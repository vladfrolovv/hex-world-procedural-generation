#region

using Zenject;

#endregion

namespace Game.Runtime.Dependencies
{
    public class GameplayDependenciesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
        }


        private void BindInstance<T>(T instance)
        {
            Container.Bind<T>().FromInstance(instance).AsSingle();
        }
    }
}
