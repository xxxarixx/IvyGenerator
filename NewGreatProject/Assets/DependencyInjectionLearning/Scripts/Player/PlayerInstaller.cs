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
    [SerializeField]
    Granade.Settings _granadeData;
    public override void InstallBindings()
    {
        // Create
        Container.Bind<PlayerWeaponHolderSystem>().FromNew().AsSingle();

        var vineController = new VineController(_settings);

        Container.Bind<IWeapon>().To<Granade>().FromInstance(new Granade(_granadeData, vineController)).AsSingle().WhenInjectedInto<PlayerWeaponHolderSystem>();

        // Player Create
        Container.Bind<VineController>().FromInstance(vineController).AsSingle();

        // Player Inject
        Container.Bind<Rigidbody>().FromInstance(_rb).AsSingle();
        Container.Bind<Camera>().FromInstance(_cam);
        Container.BindInterfacesAndSelfTo<PlayerCameraSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerInputSystem>().AsSingle();
    }
}   