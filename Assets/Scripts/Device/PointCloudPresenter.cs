using System.Collections.Generic;
using System.Linq;
using Share;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Device
{
    public sealed class PointCloudPresenter : MonoBehaviour
    {
        //iPhoneの実行画面で、特徴点群を表示する。
        [SerializeField]
        private Point pointPrefab;

        [SerializeField]
        private PointCloudHolder pointCloudHolder;

        private PointViewPool pointViewPool;
        private readonly Dictionary<ulong, Point> _points = new Dictionary<ulong, Point>();
        public int Count => _points.Count;

        private void Start()
        {
            pointViewPool = new PointViewPool(pointPrefab, this.transform);
            pointViewPool.PreloadAsync(100, 10).Subscribe();
            pointCloudHolder.PointChanged
                .TakeUntilDestroy(this).Subscribe(OnChanged);
        }

        //特徴点が変更されたときに呼び出される。
        //現在の特徴点を一時的なリストに保持し、それらが新しい特徴点に存在しない場合非表示にする。
        //新しい特徴点は表示する。
        private void OnChanged(IdentifiedPoint[] identifiedPoints)
        {
            var reserved = _points.Keys.ToArray();

            foreach (var l in reserved)
            {
                if (identifiedPoints.Any(point => point.Identify == l))
                {
                    continue;
                }
                pointViewPool.Return(_points[l]);
                _points.Remove(l);
            }

            foreach (var identifiedPoint in identifiedPoints)
            {
                if (_points.TryGetValue(identifiedPoint.Identify, out var target))
                {
                    target.transform.position = identifiedPoint.Position;
                    continue;
                }
                var ts = pointViewPool.Rent();
                ts.transform.position = identifiedPoint.Position;
                _points[identifiedPoint.Identify] = ts;
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (pointCloudHolder == null)
            {
                pointCloudHolder = FindObjectOfType<PointCloudHolder>();
            }
        }
#endif
    }
}
