using UnityEngine;

namespace Player
{
    interface IWeapon
    {
        void Shoot(Vector3 shootDirection, RaycastHit hit);

        void GizmosUpdate();
    }
}
