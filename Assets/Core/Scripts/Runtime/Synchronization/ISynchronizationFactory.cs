#region

using Core.Runtime.Synchronization.Nodes;

#endregion

namespace Core.Runtime.Synchronization
{
    public interface ISynchronizationFactory<T> where T : ISynchronizable
    {
        /// <summary>
        ///     Serializes data needed for creation of specified object by <see cref="Deserialize" />.
        ///     Note that this method does not serialize state of the object, it will be synchronized separately.
        /// </summary>
        Node Serialize(T obj);


        /// <summary>
        ///     Creates object based on data serialized by <see cref="Serialize" />.
        ///     Note that this method does not deserialize state of the object, it will be synchronized separately.
        /// </summary>
        T Deserialize(Node factoryNode);
    }
}
