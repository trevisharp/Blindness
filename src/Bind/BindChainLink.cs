/* Author:  Leonardo Trevisan Silio
 * Date:    06/08/2024
 */
namespace Blindness.Bind;

/// <summary>
/// Base type for all chain links of a chain of responsability
/// of Binding algorithm that analyzes Expressions and make the
/// data binding.
/// </summary>
public abstract class BindChainLink
{
    /// <summary>
    /// The next chain linked executed if the current chainlink do
    /// not can handle the a request.
    /// </summary>
    public BindChainLink Next { get; set; }

    /// <summary>
    /// Test if the chainlink can handle the request and
    /// handle it. If the chainlink handle with success
    /// returns true, otherside false.
    /// </summary>
    protected abstract bool TryHandle(BindingArgs args, out BindingResult result);

    /// <summary>
    /// Handle the request. If any chainlink handle with
    /// success returns true, otherside false.
    /// </summary>
    public bool Handle(BindingArgs args)
    {
        if (TryHandle(args, out _))
            return true;
        
        if (Next is null)
            return false;
        
        return Next.Handle(args);
    }

    /// <summary>
    /// Handle the request. If any chainlink handle with
    /// success returns true, otherside false.
    /// </summary>
    public bool Handle(BindingArgs args, out BindingResult result)
    {
        if (TryHandle(args, out result))
            return true;
        
        if (Next is null)
            return false;
        
        return Next.Handle(args);
    }
}