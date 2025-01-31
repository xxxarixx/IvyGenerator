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
    [SerializeField]
    VinesSystemPart2.Settings _settings;
    public override void InstallBindings()
    {
        Container.Bind<Rigidbody>().FromInstance(_rb).AsSingle();
        Container.Bind<Camera>().FromInstance(_cam);
        Container.BindInterfacesAndSelfTo<PlayerCameraSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerInputSystem>().AsSingle();
        Container.Bind<VinesSystemPart2>().FromInstance(new VinesSystemPart2(_settings)).AsSingle();
    }
}