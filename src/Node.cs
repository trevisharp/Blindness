/* Author:  Leonardo Trevisan Silio
 * Date:    17/07/2024
 */
using System;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Blindness;

using States;
using Internal;
using Concurrency;
using Concurrency.Elements;

/// <summary>
/// A base class for all concrete nodes.
/// Used by code generator.
/// </summary>
public abstract class Node : IAsyncElement
{
    int signalCount = 0;
    readonly AutoResetEvent signal = new(false);
    readonly List<(Func<bool> pred, Action act)> whenList = [];

    public Binding Bind { get; set; }
    public IAsyncModel Model { get; set; }
    public int MemoryLocation { get; set; } = Memory.Null;

    public event Action<IAsyncElement, SignalArgs> OnSignal;

    internal void LoadDependencies()
    {
        var deps = FindDeps();
        if (deps is null)
            return;
        
        var parameters = deps.GetParameters();
        var objs = new object[parameters.Length];
        
        for (int i = 0; i < objs.Length; i++)
        {
            var param = parameters[i];
            var type = param.ParameterType;
            objs[i] = DependencySystem
                .Current.GetConcrete(type);
        }

        deps.Invoke(this, objs);
    }

    /// <summary>
    /// Field used to early stop Node.
    /// </summary>
    protected bool Running { get; set; } = true;
    protected internal virtual void OnLoad() { }
    protected internal virtual void OnRun() { }
    
    public void Run()
    {
        this.Running = true;
        OnRun();
        RunWhenList();

        if (signalCount == 0)
            return;
        signal.Set();
    }

    public void Wait()
    {
        signalCount++;
        signal.WaitOne();
        signalCount--;
    }

    public void Stop()
        => Running = false;
    
    public void When(
        Func<bool> condition,
        Action action
    )
    {
        if (condition is null || action is null)
            return;
        
        whenList.Add((condition, action));
    }

    public void On(
        Expression<Func<bool>> condition,
        Action<bool> action
    )
    {
        EventElement eventElement = new(
            Model, action, condition.Compile()
        );

        AddEvents(condition, eventElement);

        Model.Run(eventElement);
    }

    void AddEvents(
        Expression exp,
        EventElement eventObj
    )
    {
        List<EventMatch> matches = new();
        AddEvents(exp, eventObj, matches);

        var uniqueMatches = matches
            .Where(m => m.Field is not null)
            .DistinctBy(m => m.Field);
        
        foreach (var match in uniqueMatches)
            AddEvent(match);
    }

    void AddEvent(
        EventMatch match
    )
    {
        var binding = GetBinding(match.Parent);

        binding.AddEvent(
            match.Field,
            match.EventObject
        );
    }

    void RunWhenList()
    {
        foreach (var (pred, act) in whenList)
        {
            if (pred())
                act();
        }
    }

    MethodInfo FindDeps()
        => FindMethod("Deps");

    MethodInfo FindMethod(string name, Type type = null)
    {
        type ??= GetType();

        var method = type.GetRuntimeMethods()
            .FirstOrDefault(p => p.Name == name);
        
        return method;
    }

    PropertyInfo FindProperty(string name, Type type = null)
    {
        type ??= GetType();
        
        var prop = type.GetRuntimeProperties()
            .FirstOrDefault(p => p.Name == name);
        
        return prop;
    }

    static Binding GetBinding(object type)
    {
        if (type is null)
            return null;

        if (type is not Node node)
            return null;

        return node.Bind;
    }
    
    static void AddEvents(
        Expression exp,
        EventElement eventObj,
        List<EventMatch> capturedEvents
    )
    {
        switch (exp.NodeType)
        {
            case ExpressionType.Lambda:
                var lambdaExp = exp as LambdaExpression;
                AddEvents(lambdaExp.Body, eventObj, capturedEvents);
                break;
            
            case ExpressionType.AndAlso:
            case ExpressionType.OrElse:
            case ExpressionType.Add:
            case ExpressionType.Subtract:
            case ExpressionType.Multiply:
            case ExpressionType.Divide:
            case ExpressionType.And:
            case ExpressionType.Or:
            case ExpressionType.Equal:
            case ExpressionType.GreaterThan:
            case ExpressionType.GreaterThanOrEqual:
            case ExpressionType.LessThan:
            case ExpressionType.LessThanOrEqual:
                var binExp = exp as BinaryExpression;
                AddEvents(binExp.Left, eventObj, capturedEvents);
                AddEvents(binExp.Right, eventObj, capturedEvents);
                break;

            case ExpressionType.MemberAccess:
                var memberExp = exp as MemberExpression;

                var propExp = memberExp.Expression as ConstantExpression;
                if (propExp is not null)
                {
                    capturedEvents.Add(new(propExp.Value, memberExp.Member as PropertyInfo, eventObj));
                    break;
                }

                AddEvents(memberExp.Expression, eventObj, capturedEvents);
                break;
        }
    }
}