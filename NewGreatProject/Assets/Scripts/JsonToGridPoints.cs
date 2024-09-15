using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HorizontalGraph.Utility
{
    public class JsonToGridPoints : MonoBehaviour
    {
        public TextAsset json;
        public GridVisualization gridVisualization;
        public PurchaseDataList gp;

        [Serializable]
        public class PurchaseData
        {
            public string date_of_purchase;
            public int quantity;
            public DateTime StringToDateTime(string format= "yyyy-MM-dd")
            {
                DateTime parsedDate;
                if (System.DateTime.TryParseExact(date_of_purchase, format, null, System.Globalization.DateTimeStyles.None, out parsedDate))
                {
                    // Parsing was successful
                    return parsedDate;
                }
                else
                {
                    // Parsing failed
                    Debug.Log("Failed to parse date: " + date_of_purchase);
                    return default;
                }
            }
        }
        [Serializable]
        public class PurchaseDataList
        {
            public PurchaseData[] purchases;
        }
        public PurchaseDataList JsonToPoints(TextAsset json)
        {
            return JsonUtility.FromJson<PurchaseDataList>(json.text);
        }
        [ContextMenu(nameof(VisualizeJson))]
        public void VisualizeJson()
        {
            gp = JsonToPoints(json);
            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i < gp.purchases.Length; i++)
            {
                PurchaseData item = gp.purchases[i];
                Debug.Log($"date:{item.StringToDateTime()} quant:{item.quantity}");
                points.Add(new Vector2(i, item.quantity));
            }
            gridVisualization.Visualize(points);
        }
    }
}
