using UnityEngine;
namespace Player
{
    [CreateAssetMenu(menuName = "Custom/Player/Stats", fileName = "Player Stats")]
    internal class PlayerStatsConfig : ScriptableObject
    {
        [SerializeField]
        internal float Speed;

    }
}
