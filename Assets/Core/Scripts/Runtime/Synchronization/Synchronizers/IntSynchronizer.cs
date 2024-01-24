#region

using Core.Runtime.Synchronization.Nodes;

#endregion

namespace Core.Runtime.Synchronization.Synchronizers
{
    public class IntSynchronizer : ISynchronizer<int>, IKeySynchronizer<int>
    {
        private static IntSynchronizer _instance;

        public static IntSynchronizer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new IntSynchronizer();
                }

                return _instance;
            }
        }


        string IKeySynchronizer<int>.Serialize(int value)
        {
            return value.ToString();
        }


        int IKeySynchronizer<int>.Deserialize(string key)
        {
            return int.Parse(key);
        }


        Node ISynchronizer<int>.Serialize(int value)
        {
            return new ValueNode<int>(value);
        }


        int ISynchronizer<int>.Deserialize(Node node)
        {
            return ((ValueNode<int>)node).Value;
        }
    }
}
