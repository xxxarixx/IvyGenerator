using Ui.Config;
using Ui.Model;
using Ui.View;
using UnityEngine;

namespace Ui.Controller
{
    internal class TestController
    {
        readonly TestConfig _config;

        readonly Transform _toggleContainer;

        readonly TestView _togglePrefabView;

        readonly static TestModel _testModel = new();

        internal TestController(Transform toggleContainer, TestConfig config, TestView togglePrefabView)
        {
            _toggleContainer = toggleContainer;
            _config = config;
            _togglePrefabView = togglePrefabView;

            Initialize();
        }

        internal static void ToggleStateChanged(bool active, int id) => _testModel.Update(active,id);

        void Initialize()
        {

            // Generate map
            for (int i = 0; i < _config.ToggleAmount; i++)
            {
                _togglePrefabView.CreateToggleCopy(toggleContainer: _toggleContainer,
                                                   initialValue: true,
                                                   text: $"toggle{i}");
                _testModel.Update(active: true,
                                  id: i);
            }
        }
    }
}
