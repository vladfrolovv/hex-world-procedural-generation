#region

using System;
using Core.Runtime.Synchronization.Nodes;

#endregion

namespace Core.Runtime.Synchronization.Synchronizers
{
    internal class EnumSynchronizer<T> : ISynchronizer<T>
        where T : struct, IConvertible
    {
        public virtual Node Serialize(T obj)
        {
            return new ValueNode<int>((int)Convert.ChangeType(obj, typeof(int)));
        }


        public virtual T Deserialize(Node node)
        {
            int value = ((ValueNode<int>)node).Value;
            return (T)Enum.ToObject(typeof(T), value);
        }
    }
}
