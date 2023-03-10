namespace PboSpy.Modules.ConfigExplorer.Utils;

static class StackExtensions
{
    /// <summary>
    /// Return the top element of stack or null if the Stack is empty.
    /// </summary>
    public static T PeekOrDefault<T>(this Stack<T> stack)
    {
        return stack.Count == 0 ? default : stack.Peek();
    }

    /// <summary>
    /// Push all children of a given node in reverse order into the
    /// <seealso cref="Stack{T}"/> <paramref name="stack"/>.
    /// 
    /// Use this to traverse the tree from left to right.
    public static void PushReversed<T>(this Stack<T> stack, IEnumerable<T> list)
    {
        foreach (var l in list.ToArray().Reverse())
        {
            stack.Push(l);
        }
    }
}
