

using Player;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Vines
{
    class Granade : IWeapon
    {
        internal Granade(Settings settings, VineController vineController)
        {
            _settings = settings;
            _vinesController = vineController;
        }
        LayerMask _vinesTarget = 1 << 0;
        // Vines
        readonly VineController _vinesController;

        Vector3 _visualizationPoint;
        [System.Serializable]
        internal class Settings
        {
            [SerializeField]
            internal int NumberOfRays = 15;
            [SerializeField]
            internal float Radius = 0.5f;
            [SerializeField]
            internal float MaxDistance;
            [SerializeField]
            internal float GizmosTime = 3f;
            [SerializeField]
            internal float BackOff = 1f;
            [SerializeField]
            internal int maxNumberOfVines = 4;
        }
        Settings _settings;
        List<RaycastHit> hitsVis = new();
        List<RaycastHit> SphereRaycast(Vector3 origin)
        {
            List<RaycastHit> hits = new();
            for (int i = 0; i < _settings.NumberOfRays; i++)
            {
                // Calculate spherical coordinates (phi and theta)
                float phi = Mathf.Acos(1 - 2 * (float)i / _settings.NumberOfRays);  // Evenly distributed from top to bottom
                float theta = Mathf.Sqrt(_settings.NumberOfRays * Mathf.PI) * phi; // Golden spiral distribution

                // Convert spherical to Cartesian coordinates
                float x = _settings.Radius * Mathf.Sin(phi) * Mathf.Cos(theta);
                float y = _settings.Radius * Mathf.Cos(phi);
                float z = _settings.Radius * Mathf.Sin(phi) * Mathf.Sin(theta);

                Vector3 rayOrigin = origin + new Vector3(x, y, z);
                Vector3 direction = new Vector3(x, y, z).normalized; // Outward direction
                if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, _settings.MaxDistance, _vinesTarget))
                    hits.Add(hit);
                Debug.DrawRay(rayOrigin, direction * _settings.MaxDistance, Color.red, _settings.GizmosTime);
            }
            return hits;
        }

        public void Shoot(Vector3 shootDirection,RaycastHit hit)
        {
            _visualizationPoint = hit.point + hit.normal * _settings.BackOff;
            hitsVis = SphereRaycast(_visualizationPoint);
            for (int i = 0; i < hitsVis.Count; i++)
            {
                RaycastHit item = hitsVis[i];
                if (item.collider == null)
                    continue;
                if (i > _settings.maxNumberOfVines)
                    break;
                _vinesController.AddVine(index: i,  
                                        shootDirection: -item.normal,
                                        shootOrigin: item.point,
                                        normal: item.normal,
                                        targetMask: _vinesTarget);
            }
            _vinesController.StartVines();
        }

        public void GizmosUpdate()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_visualizationPoint, 0.1f);

            Gizmos.color = Color.yellow;
            foreach (var item in hitsVis)
                Gizmos.DrawSphere(item.point, 0.1f);
        }
        
    }
}