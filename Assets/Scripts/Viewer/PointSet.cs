using System.Collections.Generic;
using Share;
using UnityEngine;

namespace Viewer
{
    public sealed class PointSet : MonoBehaviour
    {
        public const string DateFormat = "yyyyMMdd_HHmmssff";

        [SerializeField]
        private Point pointPrefab;

        [SerializeField]
        public Point[] Points;

        public PackedMessage.IdentifiedPointArray IdentifiedPointArray { get; private set; }

        public void Init(PackedMessage.IdentifiedPointArray identifiedPointsArray)
        {
            //Debug.Log("aaaa");
            this.IdentifiedPointArray = identifiedPointsArray;
            this.gameObject.name = identifiedPointsArray.Time.ToString(DateFormat);

            var pointsList = new List<Point>();
            var parentTs = this.transform;
            foreach (var identifiedPoint in identifiedPointsArray.Array)
            {
                var p = GameObject.Instantiate(pointPrefab, identifiedPoint.Position, Quaternion.identity, parentTs);
                p.IdentifiedPoint = identifiedPoint;
                p.gameObject.name = identifiedPoint.Identify.ToString();
                pointsList.Add(p);
            }
            
            Points = pointsList.ToArray();
        }

        public void ChangeColor(Color newColor)
        {
            foreach (var point in Points)
            {
                point.GetComponent<Renderer>().material.color = newColor;
            }
        }
    }
}
