#region

using Core.Runtime.Synchronization.Nodes;
using UnityEngine;

#endregion

namespace Core.Runtime.Synchronization.Synchronizers
{
    public class Vector2Synchronizer : ISynchronizer<Vector2>
    {
        public Node Serialize(Vector2 obj)
        {
            DictionaryNode dic = new DictionaryNode();
            {
                dic.SetFloat("x", obj.x);
                dic.SetFloat("y", obj.y);
            }

            return dic;
        }


        public Vector2 Deserialize(Node node)
        {
            DictionaryNode dic = (DictionaryNode)node;

            return new Vector2
            {
                x = dic.GetFloat("x"),
                y = dic.GetFloat("y")
            };
        }
    }
}
