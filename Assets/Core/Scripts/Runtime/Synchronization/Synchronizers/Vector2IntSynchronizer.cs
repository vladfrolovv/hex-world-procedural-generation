#region

using Core.Runtime.Synchronization.Nodes;
using UnityEngine;

#endregion

namespace Core.Runtime.Synchronization.Synchronizers
{
    public class Vector2IntSynchronizer : ISynchronizer<Vector2Int>
    {
        public Node Serialize(Vector2Int obj)
        {
            DictionaryNode dic = new DictionaryNode();
            {
                dic.SetInt("x", obj.x);
                dic.SetInt("y", obj.y);
            }

            return dic;
        }


        public Vector2Int Deserialize(Node node)
        {
            DictionaryNode dic = (DictionaryNode)node;

            return new Vector2Int
            {
                x = dic.GetInt("x"),
                y = dic.GetInt("y")
            };
        }
    }
}
