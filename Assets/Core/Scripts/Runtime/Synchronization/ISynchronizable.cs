#region

using Core.Runtime.Synchronization.States;
using JetBrains.Annotations;

#endregion

namespace Core.Runtime.Synchronization
{
    public interface ISynchronizable
    {
        void Sync([NotNull] State state);
    }
}
