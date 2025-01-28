using Player;
using UnityEngine;
using Vines;
using Zenject;

class PlayerInstaller : MonoInstaller<PlayerInstaller>
{
    [SerializeField]
    Camera _cam;
    [SerializeField]
    Rigidbody _rb;
    public override void InstallBindings()
    {
        Container.Bind<Rigidbody>().FromInstance(_rb).AsSingle();
        Container.Bind<Camera>().FromInstance(_cam);
        Container.BindInterfacesAndSelfTo<PlayerCameraSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerInputSystem>().AsSingle();
        Container.Bind<VinesSystem>().ToSelf().AsSingle();
    }
}