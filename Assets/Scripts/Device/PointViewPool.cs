using Share;
using UniRx.Toolkit;
using UnityEngine;

namespace Device
{
    public sealed class PointViewPool : ObjectPool<Point>
    {
        //特徴点群の各特徴点を視覚的に表現するためのオブジェクトプールを管理する
        private readonly Point _pointPrefab = default;
        private readonly Transform _parentTs = default;

        public PointViewPool(Point pointPrefab, Transform parentTs)
        {
            _pointPrefab = pointPrefab;
            _parentTs = parentTs;
        }

        protected override Point CreateInstance()
        {
            return GameObject.Instantiate(_pointPrefab, _parentTs);
        }
    }
}
