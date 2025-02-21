using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
namespace GraphItAll
{
    public class Test : MonoBehaviour
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
        public Vector3 offset;
        public float rad;
        private void OnDrawGizmos()
        {
            var a = this.a + offset;
            var b = this.b + offset;
            var c = this.c + offset;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(a,rad);
            Gizmos.DrawSphere(b, rad);
            Gizmos.DrawSphere(c, rad);
            Gizmos.DrawLine(a,b);
            Gizmos.DrawLine(b, c);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere((a + b) / 2, rad);
            Gizmos.color = Color.blue;
            var pointsPerCurve = 5;
            float tRange = 1f;
            List<Vector3> points = new List<Vector3>();
            for (float j = 0; j <= pointsPerCurve; j++)
            {
                float t = ((float)j * tRange) / pointsPerCurve;
                var p = Mathf.Pow((1 - t), 2) * a + 2 * (1 - t) * t * b + Mathf.Pow(t, 2) * c;
                Gizmos.DrawSphere(p, rad);
                if(points.Count > 0) 
                    Gizmos.DrawLine(p, points[points.Count - 1]);
                points.Add(p);
            }

        }
    }
}
