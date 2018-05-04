namespace System
{
    /// <summary>
    /// A .NET 2.0 function delegate polyfill.
    /// </summary>
    /// <typeparam name="T">The input parameter type.</typeparam>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <param name="arg">The argument.</param>
    /// <returns>The result.</returns>
    internal delegate TResult Func<in T, out TResult>(T arg);
}
