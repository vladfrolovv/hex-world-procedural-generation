#region

using JetBrains.Annotations;

#endregion

namespace Core.Runtime.Synchronization
{
    public interface IKeySynchronizer<T>
    {
        string Serialize([NotNull] T obj);

        T Deserialize([NotNull] string key);
    }
}
