#region

using Core.Runtime.Synchronization.Nodes;

#endregion

namespace Core.Runtime.Synchronization.Synchronizers
{
    public class LongSynchronizer : ISynchronizer<long>, IKeySynchronizer<long>
    {
        private static LongSynchronizer _instance;

        public static LongSynchronizer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LongSynchronizer();
                }

                return _instance;
            }
        }


        string IKeySynchronizer<long>.Serialize(long value)
        {
            return value.ToString();
        }


        long IKeySynchronizer<long>.Deserialize(string key)
        {
            return long.Parse(key);
        }


        Node ISynchronizer<long>.Serialize(long value)
        {
            return new ValueNode<long>(value);
        }


        long ISynchronizer<long>.Deserialize(Node node)
        {
            return ((ValueNode<long>)node).Value;
        }
    }
}
