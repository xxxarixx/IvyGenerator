using UnityEngine;

namespace Ui.Config
{
    [CreateAssetMenu(menuName = "custom/config/test", fileName = "test config")]
    public class TestConfig : ScriptableObject
    {
        [SerializeField]
        int toggleAmount;

        public int ToggleAmount => toggleAmount;
    }
}
