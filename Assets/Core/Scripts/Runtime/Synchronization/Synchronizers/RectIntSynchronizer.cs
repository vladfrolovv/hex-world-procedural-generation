#region

using Core.Runtime.Synchronization.Nodes;
using UnityEngine;

#endregion

namespace Core.Runtime.Synchronization.Synchronizers
{
    public class RectIntSynchronizer : ISynchronizer<RectInt>
    {
        public Node Serialize(RectInt obj)
        {
            DictionaryNode dic = new DictionaryNode();
            {
                dic.SetInt("x", obj.x);
                dic.SetInt("y", obj.y);
                dic.SetInt("width", obj.width);
                dic.SetInt("height", obj.height);
            }

            return dic;
        }


        public RectInt Deserialize(Node node)
        {
            DictionaryNode dic = (DictionaryNode)node;

            return new RectInt(
                dic.GetInt("x"),
                dic.GetInt("y"),
                dic.GetInt("width"),
                dic.GetInt("height")
            );
        }
    }
}
