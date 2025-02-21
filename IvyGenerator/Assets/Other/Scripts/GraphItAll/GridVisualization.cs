using System.Collections.Generic;
using UnityEngine;
namespace GraphItAll.HorizontalGraph
{
    [RequireComponent(typeof(HorizontalGrid))]
    public class GridVisualization : MonoBehaviour
    {
        private HorizontalGrid grid;
        public Color pointColor = Color.white;
        public Color linesColor = Color.gray;
        public Color gridColor = Color.gray;
        public Color viewPortColor = Color.green;
        public HorizontalGrid.SmoothingType smoothing = HorizontalGrid.SmoothingType.Bezier;
        [Range(3,50)]
        public int pointsPerCurve = 4;
        [Range(0.01f,1f)]
        public float gizSize;
        private List<Vector3> vis = new List<Vector3>();
        public int maxRecordsToShow;
        [Range(0,100)]
        public int timeOffset;
        private List<Vector2> lastPoints = new List<Vector2>();
        public void Visualize(List<Vector2> points)
        {
            //if(lastPoints.Count != points.Count)
            vis = grid.GenerateGraph(points.ToArray(), timeOffset, smoothing, maxRecordsToShow,pointsPerCurve: pointsPerCurve);
            lastPoints = points;
        }
        public void VisualizeLastVisualize()
        {
            Visualize(lastPoints);
        }
        private void OnValidate()
        {
            grid = GetComponent<HorizontalGrid>();
            VisualizeLastVisualize();
        }
        private void OnDrawGizmos()
        {
            if (grid == null)
                OnValidate();
            DrawGraphViewPort();
            if (vis  == null|| vis.Count == 0 || lastPoints.Count == 0)
                return;
            DrawPoints();
        }
        private void DrawPoints()
        {
            for (int i = -1; i < vis.Count; i++)
            {
                if(i != -1)
                {
                    Vector3 worldPosition = vis[i];
                    Gizmos.color = pointColor;
                    Gizmos.DrawSphere(worldPosition, gizSize);
                }

                Gizmos.color = pointColor;
                Vector3 startPos = i == -1 ? grid.startingPosition.position : vis[i];
                Vector3 endPos = i >= vis.Count - 1 ? vis[i] : vis[i + 1];
                Gizmos.DrawLine(startPos, endPos);
            }
        }
        private void DrawGraphViewPort()
        {
            float vpWidth = maxRecordsToShow * grid.gridSpacing;
            float vpHeight = grid.maxHeight;
            Vector3 viewPortCenter = grid.startingPosition.position + (Vector3)grid.offset + new Vector3(vpWidth / 2, vpHeight / 2);
            float gridWidth = vpWidth / maxRecordsToShow;
            //grid
            if (vis != null && vis.Count > 0)
            {
                Gizmos.color = gridColor;
                for (int i = 0; i < Mathf.Clamp(vis.Count, 0, maxRecordsToShow); i++)
                {
                    Vector3 worldPosition = vis[i];
                    Gizmos.DrawWireCube(new Vector3(worldPosition.x + gridWidth / 2, viewPortCenter.y), new Vector3(gridWidth, vpHeight));
                }
            }

            //viewPort
            Gizmos.color = viewPortColor;
            Gizmos.DrawWireCube(viewPortCenter, new Vector3(vpWidth,vpHeight));

        }
    }
}
