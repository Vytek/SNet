using System.Collections.Generic;
using SNet.Core.Common;
using SNet.Core.Models;
using SNet.Core.Models.Network;
using SNet.Core.Models.Router;
using UnityEditor;
using UnityEngine;

namespace SNet.Core
{
    [AddComponentMenu("Network/SNet/SNet Manager")]
    public class SNetManager : MonoBehaviourSingleton<SNetManager>
    {
        [SerializeField] private string networkAddress = "localhost";
        [SerializeField] private ushort networkPort = 16490;
        [SerializeField] private int maxConnections = 4;
        [SerializeField] private int maxChannels = 4;

        [SerializeField] private bool connectClientOnStart = true;
        [SerializeField] private bool dontDestroyOnLoad = true;
        [SerializeField] private bool runInBackground = true;

        [SerializeField] private List<GameObject> spawnList;

        private bool _isClient;
        private bool _isServer;

        public ClientNetwork Client { get; private set; }
        public ServerNetwork Server { get; private set; }

        public bool IsServerActive => _isServer && Server != null && Server.IsActive;
        public bool IsClientActive => _isClient && Client != null && Client.IsActive;

        public static bool IsClient => Instance.IsClientActive;
        public static bool IsServer => Instance.IsServerActive;
        
        public string NetworkAddress => networkAddress;
        public ushort NetworkPort => networkPort;

        public static string SpawnMessageHeader => "";

        private void Start()
        {
            if(dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
            Application.runInBackground = runInBackground;
        }

        private void Update()
        {
            if(IsClientActive)
                Client.Update();
            if(IsServerActive)
                Server.Update();
        }

        private void OnDestroy()
        {
            if(IsClientActive)
                Client.Quit();
            if(IsServerActive)
                Server.Quit();
        }

        #region Client
        public static void StartClient()
        {
            Instance.StartClientNetwork();
        }
        
        private void StartClientNetwork()
        {
            _isClient = true;
            Client = new ClientNetwork();
            
            Client.OnConnect += data => Debug.Log($"Connected to server {data.PeerId}");
            Client.OnReceive += data => Debug.Log($"Received message from server {data.PeerId}");
            
            NetworkRouter.RegisterByChannel(ChannelType.SNetIdentity, SpawnMessageHeader, SpawnEntity);
            
            Client.Create();
            
            if(connectClientOnStart)
                Client.Connect(networkAddress, networkPort, maxChannels);
        }

        private void SpawnEntity(uint peerId, byte[] data)
        {
            var idMsg = new ObjectSpawnMessage();
            idMsg.Deserialize(data);
            var prefab = spawnList.Find(go => go.GetComponent<SNetIdentity>().AssetId == idMsg.AssetId);
            var newObj = NetworkScene.Spawn(prefab, idMsg.Position, idMsg.Rotation);
            newObj.GetComponent<SNetIdentity>().Initialize(idMsg.Id);
        }
        #endregion

        #region Server
        public static void StartServer()
        {
            Instance.StartServerNetwork();
        }

        private void StartServerNetwork()
        {
            _isServer = true;
            Server = new ServerNetwork();

            Server.OnConnect += ServerOnClientConnect;
            Server.OnDisconnect += ServerOnClientDisconnect;
            Server.OnTimeout += ServerOnClientTimeout;
            Server.OnReceive += ServerOnClientReceive;

            Server.Listen(networkAddress, networkPort, maxConnections, maxChannels);
        }

        internal virtual void ServerOnClientConnect(ServerEventData data)
        {
            Debug.Log($"Client {data.PeerId} connected with address {data.PeerIp}");
            Server.AddToFilter(data.PeerId);
        }

        internal virtual void ServerOnClientDisconnect(ServerEventData data)
        {
            
        }

        internal virtual void ServerOnClientTimeout(ServerEventData data)
        {
            
        }

        internal virtual void ServerOnClientReceive(ServerEventData data)
        {
            
        }
        #endregion
    }
}