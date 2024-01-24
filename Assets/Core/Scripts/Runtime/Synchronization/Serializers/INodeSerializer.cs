#region

using System.IO;
using Core.Runtime.Synchronization.Nodes;

#endregion

namespace Core.Runtime.Synchronization.Serializers
{
    public interface INodeSerializer
    {
        void Serialize(Stream stream, Node node);

        Node Deserialize(Stream stream);
    }
}
