using System;
using UnityEngine;
using UnityEngine.Events;

namespace SNet.Core.Events
{
    [Serializable] public class ColorEvent : UnityEvent<Color> { }

    public class SNetColorEvent : SNetEvent
    {
        public ColorEvent clientReceiveCallback;

        protected override void Setup()
        {
            clientReceive += OnClientReceive;
        }

        public void ServerBroadcast(Color color)
        {
            var arr = SNetColorSerializer.Serialize(color);
            ServerBroadcastSerializable(arr);
        }

        private void OnClientReceive(uint peerId, byte[] data)
        {
            var color = SNetColorSerializer.Deserialize(data);
            clientReceiveCallback?.Invoke(color);
        }
    }
}