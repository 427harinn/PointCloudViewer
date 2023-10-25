using System;
using System.Collections.Generic;
using Share;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Device
{
    public sealed class PointCloudHolder : MonoBehaviour
    {
        //特徴点を管理、特徴店に変更があると識別された特徴店を生成する
        private const int IntervalMsec = 100;

        [SerializeField]
        private ARPointCloudManager pointCloudManager = default;

        private Transform _cameraTs = null;
        //特徴点が変更されたときに通知を受け取る
        public IObservable<IdentifiedPoint[]> PointChanged =>
            Observable.FromEvent<ARPointCloudChangedEventArgs>(
                    h => pointCloudManager.pointCloudsChanged += h,
                    h => pointCloudManager.pointCloudsChanged -= h
                )
                .ThrottleFirst(TimeSpan.FromMilliseconds(IntervalMsec))
                .Select(args => CurrentPoints());

        private readonly List<IdentifiedPoint> _cacheIdentifiedPoint = new List<IdentifiedPoint>();

        public void Update()
        {
            Debug.Log(_cacheIdentifiedPoint.Count);
            foreach (var point in _cacheIdentifiedPoint)
            {
                Debug.Log("特徴点"+$"Identify: {point.Identify}, Position: {point.Position}, Confidence: {point.Confidence}");
            }
        }
        //特徴点の情報を収集し(位置、識別情報、信頼度、デバイスのカメラ位置と回転情報)、識別された特徴店の配列を生成する
        public IdentifiedPoint[] CurrentPoints()
        {
            
            _cacheIdentifiedPoint.Clear();

            if (_cameraTs == null)
            {
                _cameraTs = Camera.main?.transform;
            }
            var cameraPos = _cameraTs?.position ?? Vector3.zero;
            var cameraRot = _cameraTs?.rotation ?? Quaternion.identity;

            var trackable = pointCloudManager.trackables;
            foreach (var pointCloud in trackable)
            {
                
                if (!pointCloud.identifiers.HasValue || !pointCloud.positions.HasValue /*|| !pointCloud.confidenceValues.HasValue*/)
                {
                    Debug.Log("1です" + pointCloud.identifiers.HasValue);
                    Debug.Log("2です"+pointCloud.positions.HasValue);
                    //Debug.Log("3です"+pointCloud.confidenceValues.HasValue);
                    continue;
                }
                var identifiers = pointCloud.identifiers.Value;
                var position = pointCloud.positions.Value;
                //var confidence = pointCloud.confidenceValues.Value;

                for (var count = 0; count < identifiers.Length; count++)
                {
                    _cacheIdentifiedPoint.Add(new IdentifiedPoint
                    {
                        Identify = identifiers[count],
                        Position = position[count],
                        //Confidence = confidence[count],
                        CameraPosition = cameraPos,
                        CameraRotation = cameraRot,
                    });
                }
            }
            return _cacheIdentifiedPoint.ToArray();
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (pointCloudManager == null)
            {
                pointCloudManager = FindObjectOfType<ARPointCloudManager>();
            }
        }
#endif
    }
}
