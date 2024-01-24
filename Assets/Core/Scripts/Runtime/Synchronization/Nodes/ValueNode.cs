#region

using System;
using JetBrains.Annotations;

#endregion

namespace Core.Runtime.Synchronization.Nodes
{
    public class ValueNode<T> : Node
    {
        private T _value;


        public ValueNode([NotNull] T value)
        {
            Value = value;
        }


        public T Value
        {
            get => _value;

            [NotNull]
            set
            {
                CheckNull(value);
                CheckType(value.GetType(), this);
                _value = value;
            }
        }


        public static void CheckType(Node context)
        {
            CheckType(typeof(T), context);
        }


        private static void CheckType(Type t, Node context)
        {
            if (t != typeof(bool) &&
                t != typeof(int) &&
                t != typeof(float) &&
                t != typeof(string) &&
                t != typeof(long) &&
                t != typeof(ulong))
            {
                throw new InvalidOperationException($"Type '{t.Name}' is not supported.");
            }
        }


        private void CheckNull([NotNull] object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "Null value is not supported.");
            }
        }
    }
}
