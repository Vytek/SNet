using System;
using SNet.Core.Models.Router;
using UnityEngine.Events;

namespace SNet.Core.Events
{
    public class SNetEvent<T> : SNetEntity
    {
        public UnityEvent<T> clientReceiveCallback;
        public UnityEvent<T> serverReceiveCallback;
        
        protected new void Awake()
        {
            if (IsClient && clientReceiveCallback != null)
            {
                // TODO Change to NetworkRouter.RegisterClientCallback(identity.Id, clientEventCallbacks);
                NetworkRouter.Register(ChannelType.Base, HeaderType.Base, ((id, value) => clientReceiveCallback?.Invoke((T)value)), typeof(T));
            }

            if (IsServer && serverReceiveCallback != null)
            {
                // TODO Change to NetworkRouter.RegisterServerCallback(identity.Id, serverEventCallbacks);
            }
        }

        public override void ServerBroadcast(byte[] data)
        {
            //var obj = (TObj) data;
            //var serializable = (TSerializableObj) new TSerializableObj().ConvertFrom(obj);
            //ServerBroadcastSerializable(serializable);
        }

        public override void OnServerReceive(byte[] data)
        {
            //var serializable = (TSerializableObj) data;
            //var obj = serializable.ConvertTo();
            //clientEventCallbacks?.Invoke(obj);
        }

        public override void ServerSend(object target, byte[] data)
        {
            throw new NotImplementedException();
        }

        public override void OnClientReceive(byte[] data)
        {
            throw new NotImplementedException();
        }

        public override void ClientSend(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}