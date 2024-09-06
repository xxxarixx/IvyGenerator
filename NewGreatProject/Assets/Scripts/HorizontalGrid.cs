using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Grid
{
    public class HorizontalGrid : MonoBehaviour
    {
        public static HorizontalGrid instance { get; private set; }

        /// <summary>
        /// key => time; value => node on grid
        /// </summary>
        public Dictionary<float,Node> grid = new Dictionary<float,Node>();
        public Transform startingPosition;
        [Range(1,100)]
        public int timeStamp = 20;
        [System.Serializable]
        public class Node
        {
            public float value;
        }
    }
}
