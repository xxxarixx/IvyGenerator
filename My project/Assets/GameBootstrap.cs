using Ui.Config;
using Ui.Controller;
using Ui.View;
using UnityEngine;

class GameBootstrap : MonoBehaviour
{
    [Header("Config")]

    [SerializeField]
    Transform _toggleContainer;

    [SerializeField]
    TestConfig _config;

    [SerializeField]
    TestView _togglePrefab;

    TestController _testController;

    void Start() => _testController = new(_toggleContainer, _config, _togglePrefab);
}
