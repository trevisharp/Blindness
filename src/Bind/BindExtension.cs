/* Author:  Leonardo Trevisan Silio
 * Date:    02/08/2024
 */
using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace Blindness.Bind;

/// <summary>
/// A extension class to System.Linq.Expression, System.Type
/// and other types to handle expressions and reflection more easely.
/// </summary>
public static class BindExtension
{
    /// <summary>
    /// Find in properties and fields of a object by a member
    /// with type T and get your data.
    /// </summary>
    public static bool GetDataByType<T>(this object obj, out T data)
    {
        var type = obj.GetType();
        var targetType = typeof(T);

        var property = type.GetRuntimeProperties()
            .FirstOrDefault(p => p.PropertyType == targetType);
        if (property is not null)
        {
            data = (T)GetProperty(obj, property);
            return true;
        }

        var field = type.GetRuntimeFields()
            .FirstOrDefault(p => p.FieldType == targetType);
        if (field is not null)
        {
            data = (T)GetField(obj, field);
            return true;
        }

        data = default;
        return false;
    }

    /// <summary>
    /// Try get a member data value with a specific name
    /// from a specific object.
    /// </summary>
    public static bool TryGetData(
        this object obj,
        string memberName,
        out object data
    )
    {
        var type = obj.GetType();

        var property = type.GetProperty(memberName);
        if (property is not null)
        {
            data = GetProperty(obj, property); 
            return true;
        }

        var field = type.GetField(memberName);
        if (field is not null)
        {
            data = GetField(obj, field);
            return true;
        }

        data = null;
        return false;
    }

    static object GetProperty(object obj, PropertyInfo prop)
    {
        if (obj is null || prop is null)
            return null;
        var getMethod = prop.GetGetMethod();
        return getMethod.Invoke(obj, []);
    }

    static object GetField(object obj, FieldInfo field)
    {
        if (obj is null || field is null)
            return null;
        return field.GetValue(obj);
    }
}