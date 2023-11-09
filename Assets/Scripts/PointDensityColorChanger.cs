using System.Collections.Generic;
using UnityEngine;

namespace Viewer
{
    public class PointDensityColorChanger : MonoBehaviour
    {
        [SerializeField]
        private PointRoot pointRoot; // PointRoot スクリプトへの参照

        [SerializeField]
        private float radius = 1f; // 密度計算の半径

        [SerializeField]
        private float lowDensityThreshold = 800f;
        [SerializeField]
        private float mediumDensityThreshold = 1000f;

        private void Update()
        {
            // すべての特徴点をループして密度に応じて色を変更
            foreach (var pointSet in pointRoot.PointSets)
            {
                var points = pointSet.Points; // 特徴点リスト

                for (int i = 0; i < points.Length; i++)
                {
                    float density = CalculateDensity(points, i);

                    Debug.Log(density);
                    // 密度に応じて色を変更
                    Color color = GetColorByDensity(density);
                    points[i].ChangeColor((identifiedPoint) => color);
                }
            }
        }

        private float CalculateDensity(Share.Point[] points, int targetIndex)
        {
            float density = 0f;

            // ターゲットとなる特徴点
            Share.Point targetPoint = points[targetIndex];

            for (int i = 0; i < points.Length; i++)
            {
                if (i != targetIndex)
                {
                    // ターゲットとなる特徴点からの距離を計算
                    float distance = Vector3.SqrMagnitude(targetPoint.transform.position - points[i].transform.position);

                    if (distance < radius * radius)
                    {
                        density += 1.0f;
                    }
                }
            }

            // 特徴点の密度を半径で割って正規化
            density /= radius;

            return density;
        }

        private Color GetColorByDensity(float density)
        {
            if (density < lowDensityThreshold)
            {
                return Color.green;
            }
            else if (density < mediumDensityThreshold)
            {
                return Color.yellow;
            }
            else
            {
                return Color.red;
            }
        }
    }
}
