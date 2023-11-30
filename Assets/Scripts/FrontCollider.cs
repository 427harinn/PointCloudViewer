using System.Collections.Generic;
using Share;
using UnityEngine;


public class FrontCollider : MonoBehaviour
{
    [SerializeField]
    private Point PointPrefab;

    private List<Point> collidedPoints = new List<Point>();
    public int requiredPointCount = 3;  // 何個以上の点が当たったら出力するか

    void OnTriggerEnter(Collider other)
    {
        Point pointPrefab = other.GetComponent<Point>();
        if (pointPrefab != null && !collidedPoints.Contains(pointPrefab))
        {
            collidedPoints.Add(pointPrefab);
            Debug.Log("点に当たりました：" + pointPrefab.name);

            if (collidedPoints.Count >= requiredPointCount)
            {
                Debug.Log(requiredPointCount + "個以上の点に当たりました！");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        Point pointPrefab = other.GetComponent<Point>();
        if (pointPrefab != null && collidedPoints.Contains(pointPrefab))
        {
            collidedPoints.Remove(pointPrefab);
        }
    }
}
