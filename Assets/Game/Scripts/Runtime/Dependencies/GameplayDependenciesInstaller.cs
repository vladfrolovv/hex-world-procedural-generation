#region

using Game.Runtime.Cameras;
using UnityEngine;
using Zenject;

#endregion

namespace Game.Runtime.Dependencies
{
    public class GameplayDependenciesInstaller : MonoInstaller
    {
        [SerializeField] private CameraInstaller _cameraInstaller;

        public override void InstallBindings()
        {
            BindInstance(_cameraInstaller);
        }


        private void BindInstance<T>(T instance)
        {
            Container.Bind<T>().FromInstance(instance).AsSingle();
        }
    }
}
