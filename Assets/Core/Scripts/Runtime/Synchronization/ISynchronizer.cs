#region

using Core.Runtime.Synchronization.Nodes;
using JetBrains.Annotations;

#endregion

namespace Core.Runtime.Synchronization
{
    public interface ISynchronizer<T>
    {
        Node Serialize([NotNull] T obj);

        T Deserialize([NotNull] Node node);
    }
}
