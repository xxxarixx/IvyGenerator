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
        /// </summary>
        public Dictionary<int,Node> grid = new Dictionary<int, Node>();
        public Transform startingPosition;
        public Vector2 offset = new Vector2(1.5f, 1.5f);
        public float gridSpacing;
        public float maxHeight = 10f;
        public GridVisualization gridVisualization;
        [System.Serializable]
        public class Node
        {
            public readonly float value;
            public readonly Vector3 worldPosition;
            public Node(int time, float value, HorizontalGrid grid)
            {
                this.value = value;
                this.worldPosition = GridToWorld(time, value, grid);
            }
            public Vector3 GridToWorld(int time, float value, HorizontalGrid grid)
            {
                value = grid.maxHeight / value;
                return grid.startingPosition.position + (Vector3)grid.offset + new Vector3(time * grid.gridSpacing,value);
            }
        }
        public List<Vector3> GenerateGraph(Vector2[] graphPoints, int timeOffset, int maxRecords = int.MaxValue)
        {
            if (startingPosition == null)
                return default;
            ///Setup Veriables
            grid.Clear();
            graphPoints = graphPoints.OrderBy(p => p.x).ToArray();
            int maxRange = graphPoints.Length - 1;
            maxRecords = Mathf.Clamp(maxRecords, 0, maxRange);
            timeOffset = Mathf.Clamp(timeOffset, 0, maxRange - 1);
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
