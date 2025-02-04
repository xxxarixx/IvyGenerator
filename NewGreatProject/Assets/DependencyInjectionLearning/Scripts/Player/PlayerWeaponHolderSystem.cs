using UnityEngine;
using Zenject;

namespace Player
{ 
    class PlayerWeaponHolderSystem
    {
        [Inject] IWeapon _currentWeapon;  // Inject the current weapon
        internal void SetWeapon(IWeapon weapon)
        {
            _currentWeapon = weapon;
        }

        internal void Fire(Vector3 shootDirection, RaycastHit hit)
        {
            if (_currentWeapon != null)
            {
                _currentWeapon.Shoot(shootDirection,hit);
            }
            else
            {
                Debug.LogWarning("No weapon equipped!");
            }
        }

        internal void Gizmos()
        {
            if (_currentWeapon != null)
            {
                _currentWeapon.GizmosUpdate();
            }
            else
            {
                Debug.LogWarning("No weapon equipped!");
            }
        }
    }
}

