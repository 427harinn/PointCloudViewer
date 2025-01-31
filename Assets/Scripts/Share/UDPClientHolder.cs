using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Share
{
    public sealed class UDPClientHolder : MonoBehaviour
    {
        private UdpClient _udpClient = null;
        public const int Port = 3210;
        public string Address { get; private set; }

        private readonly Subject<byte[]> _receive = new Subject<byte[]>();
        public IObservable<byte[]> ObservableReceive => _receive;

        private void Awake()
        {
            Address = GetAddress();

            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, Port))
            {
                EnableBroadcast = true
            };

            var token = this.GetCancellationTokenOnDestroy();
            ReceiveLoop(token).Forget();
            PingLoop(token).Forget();
        }

        private void OnDestroy()
        {
            _udpClient?.Close();
            _udpClient = null;
        }

        public readonly HashSet<IPEndPoint> SendTarget = new HashSet<IPEndPoint>();

        public void Send(byte[] data)
        {
            foreach (var ipEndPoint in SendTarget)
            {
                _udpClient.SendAsync(data, data.Length, ipEndPoint);
            }
        }

        private async UniTaskVoid ReceiveLoop(CancellationToken token)
        {
            await UniTask.SwitchToThreadPool();

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                try
                {
                    var receiveData = await _udpClient.ReceiveAsync();

                    if (receiveData.RemoteEndPoint.ToString().StartsWith(Address))
                    {
                        continue;
                    }

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    if (BufferPing.SequenceEqual(receiveData.Buffer))
                    {
                        SendTarget.Add(receiveData.RemoteEndPoint);
                        continue;
                    }

                    _receive?.OnNext(receiveData.Buffer);
                }
                catch (Exception e)
                {
                    if (!(e is ObjectDisposedException))
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }

        private const int PingIntervalSec = 1;
        private const int BufferPingLength = 4;
        private static readonly byte[] BufferPing = new byte[BufferPingLength];

        private async UniTaskVoid PingLoop(CancellationToken token)
        {
            await UniTask.SwitchToThreadPool();

            var target = new IPEndPoint(IPAddress.Broadcast, Port);

            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(PingIntervalSec), cancellationToken: token);
                if (token.IsCancellationRequested)
                {
                    break;
                }
                await _udpClient.SendAsync(BufferPing, BufferPingLength, target);
            }
        }

        public static string GetAddress()
        {
            return Dns.GetHostAddresses(Dns.GetHostName())
                .Select(ipAddress => ipAddress.ToString())
                //.FirstOrDefault(s => s.StartsWith("192.168"));
                .FirstOrDefault(s => s.StartsWith("133.17"));
        }
    }
}
