using System;
using Cysharp.Threading.Tasks;
using Share;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Device
{
    public sealed class PackedMessageSender : MonoBehaviour
    {
        //iPhoneの座標、角度と特徴点群の情報をUDP通信で送信するためのスクリプト
        [SerializeField]
        private UDPClientHolder udpClientHolder;

        [SerializeField]
        private PointCloudHolder pointCloudHolder;

        [SerializeField]
        private Transform device;

        
        private async UniTaskVoid Start()
        {
            await UniTask.Yield();

            Observable.Interval(TimeSpan.FromMilliseconds(100))
                .TakeUntilDestroy(this)
                .Subscribe(OnDevicePose);

            await UniTask.WaitUntil(() => ARSession.state == ARSessionState.SessionTracking);

            pointCloudHolder.PointChanged
                .TakeUntilDestroy(this).Subscribe(OnPointChanged);
        }

        //特徴点群情報が変更されたときに実行される。特徴点群の情報をシリアライズし、udpClientHolderを使用してUDP通信する
        private void OnPointChanged(IdentifiedPoint[] identifiedPoints)
        {
            if (identifiedPoints.Length == 0)
            {
                return;
            }
            var array = new PackedMessage.IdentifiedPointArray
            {
                Array = identifiedPoints,
                Time = DateTimeOffset.Now,
            };
            udpClientHolder.Send(array.Serialize());
        }
        //一定間隔でiPhoneの座標、角度を送信する。そしてそれをUDP通信で送信
        private void OnDevicePose(long _)
        {
            var devicePose = new PackedMessage.DevicePose
            {
                Position = device.position,
                Rotation = device.rotation,
            };
            udpClientHolder.Send(devicePose.Serialize());
        }
    }
}
