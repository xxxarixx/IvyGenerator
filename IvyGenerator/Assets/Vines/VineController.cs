using System.Collections.Generic;
using UnityEngine;

namespace Vines
{
    class VineController
    {
        readonly Dictionary<int, VinesSystemPart2> _vines = new();
        VinesSystemPart2.Settings _settings;
        internal VineController(VinesSystemPart2.Settings settings) => _settings = settings;

        internal void AddVine(int index,Vector3 shootDirection, Vector3 shootOrigin, Vector3 normal, LayerMask targetMask)
        {
            if (_vines.ContainsKey(index))
                _vines[index].ChangeValues(shootDirection, shootOrigin, normal, targetMask);
            else
                _vines.Add(index,new VinesSystemPart2(_settings, shootDirection, shootOrigin, normal, targetMask));
        }

        internal void Update()
        {
            foreach (VinesSystemPart2 vine in _vines.Values)
                vine.Update();
        }
        internal void GizmosDebug()
        {
            foreach (VinesSystemPart2 vine in _vines.Values)
                vine.GizmosDebug();
        }

        internal void StartVines()
        {
            foreach (VinesSystemPart2 vine in _vines.Values)
                vine.Invoke();
        }
    }
}
