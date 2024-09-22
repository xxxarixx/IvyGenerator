using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace HorizontalGraph
{
    [DefaultExecutionOrder(-1), ExecuteAlways]
    public class HorizontalGrid : MonoBehaviour
    {

        /// <summary>
        /// key => time; value => node on grid
        /// https://www.math.ucla.edu/~baker/149.1.02w/handouts/dd_splines.pdf
        /// </summary>
        public Dictionary<float,Node> grid = new Dictionary<float, Node>();
        public Transform startingPosition;
        public Vector2 offset = new Vector2(1.5f, 1.5f);
        public float gridSpacing;
        public float maxHeight = 10f;
        public GridVisualization gridVisualization;
        public enum SmoothingType
        {
            None,
            Bezier
        }
        [System.Serializable]
        public class Node
        {
            public readonly float value;
            public readonly float time;
            public readonly Vector2 timeXValueY;
            public readonly Vector3 worldPosition;
            public Node(float time, float value, HorizontalGrid grid)
            {
                this.value = value;
                this.time = time;
                this.timeXValueY = new Vector2(time, value);
                this.worldPosition = GridToWorld(time, value, grid);
            }
            public Vector3 GridToWorld(float time, float value, HorizontalGrid grid)
            {
                value = grid.maxHeight / value;
                return grid.startingPosition.position + (Vector3)grid.offset + new Vector3(time * grid.gridSpacing,value);
            }
        }
        public List<Vector3> GenerateGraph(Vector2[] graphPoints, int timeOffset, SmoothingType smoothingType, int maxRecords = int.MaxValue, int pointsPerCurve = 3)
        {
            if (startingPosition == null)
                return default;
            if (graphPoints.Length == 0)
                return default;
            ///Setup Veriables
            grid.Clear();
            graphPoints = graphPoints.OrderBy(p => p.x).ToArray();
            int maxRange = graphPoints.Length - 1;
            maxRecords = Mathf.Clamp(maxRecords, 0, maxRange);
            timeOffset = Mathf.Clamp(timeOffset, 0, maxRange - 1);
            pointsPerCurve = Mathf.Clamp(pointsPerCurve, 3, 100);
            ///Get selected record range
            int from = timeOffset;
            int to = Mathf.Clamp(Mathf.Clamp(graphPoints.Length, 1, maxRecords) + timeOffset,0, maxRange);
            for (int i = from; i <= to; i++)
            {
                Vector2 point = graphPoints[i];
                ///Make sure that even with applied timeOffset first point will start from first position
                point.x = point.x - timeOffset;
                if (grid.ContainsKey((int)point.x))
                    continue;
                ///Key => Time (position X), Value => Node
                grid.Add((int)point.x,new Node((int)point.x,point.y,this));
            }
            switch (smoothingType)
            {
                case SmoothingType.None:
                    return grid.Select(p => p.Value.worldPosition).ToList();
                case SmoothingType.Bezier:
                    return LinearGraphToBezier(pointsPerCurve);
                default:
                    return grid.Select(p => p.Value.worldPosition).ToList();
            }
           
        }
        private List<Vector3> LinearGraphToBezier(int pointsPerCurve = 3)
        {
            Dictionary<float, Node> newGridPopulation = new Dictionary<float, Node>();
            int nodesToSkip = 0;
            //P = (1−t)^2P1 + 2(1−t)tP2 + t^2P3
            /* foreach(KeyValuePair<float, Node> node in grid)
             {
                 if(nodesToSkip > 0)
                 {
                     nodesToSkip--;
                     continue;
                 }
                 if (!grid.ContainsKey(node.Key + 1) || !grid.ContainsKey(node.Key + 2))
                     break;
                 var p1Node = node.Value;
                 var p2Node = grid[node.Key + 1];
                 var p3Node = grid[node.Key + 2];

                 var p1 = new Vector2(p1Node.time, p1Node.value);
                 var p2 = new Vector2(p2Node.time, p2Node.value);
                 var p3 = new Vector2(p3Node.time, p3Node.value);
                 if(!newGridPopulation.ContainsKey(p1.x))
                     newGridPopulation.Add(p1.x, new Node(time: p1.x, value: p1.y, this));
                 float tRange = 1f;
                 for (float j = 1; j <= pointsPerCurve; j++)
                 {
                     float t = ((float)j * tRange) / pointsPerCurve;
                     //var p = Mathf.Pow((1 - t), 2) * p1 + 2 * (1 - t) * t * p2 + Mathf.Pow(t, 2) * p3;
                     var p = Vector2.Lerp(p1, p2, t);
                     //var p = 2 * (1 - t) * (p2 - p1) + 2 * t * (p3 - p2);
                     //var p = p2 + Mathf.Pow((1 - t), 2) * (p1 - p2) + Mathf.Pow(t, 2) * (p3 - p2);
                     if (!newGridPopulation.ContainsKey(p.x))
                         newGridPopulation.Add(p.x, new Node(time:p.x, value:p.y, this));
                 }
                 nodesToSkip = 0;
                 *//*if (!newGridPopulation.ContainsKey(p3.x))
                     newGridPopulation.Add(p3.x, new Node(time: p3.x, value: p3.y, this));*//*
             }*/
            for (int i = 1; i < 4; i++)
            {
                float tRange = 1f;
                var p1 = grid[i].timeXValueY;
                var p2 = grid[i + 1].timeXValueY;
                for (float j = 0; j <= 2; j++)
                {
                    float t = ((float)j * tRange) / 2;
                    var p = (p1+p2) / 2;
                    Debug.Log($"{i},{i+1}|p1=>{p1},p2=>{p2},av=>{p}");
                    if (!newGridPopulation.ContainsKey(p.x))
                        newGridPopulation.Add(p.x, new Node(time: p.x, value: p.y, this));
                }
            }
            grid.Clear();
            grid = newGridPopulation;
            return grid.Select(p => p.Value.worldPosition).ToList();
        }
        private void OnValidate()
        {
            if (gridVisualization == null)
                return;
            gridVisualization.VisualizeLastVisualize();
        }
    }
}