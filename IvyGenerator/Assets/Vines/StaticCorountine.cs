using System.Collections;
using UnityEngine;

namespace Vines
{
    class StaticCorountine : MonoBehaviour
    {
        internal static StaticCorountine instance;
        void Awake() { instance = this; }
        internal static void StartStaticCoruntine(IEnumerator routine) =>
            instance.StartCoroutine(routine);
        internal static void StopStaticCoruntine(IEnumerator routine) =>
          instance.StopCoroutine(routine);
    }
}
