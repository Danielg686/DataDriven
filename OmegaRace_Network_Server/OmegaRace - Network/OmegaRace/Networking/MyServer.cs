using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using System.Net;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace OmegaRace
{
    struct NetworkInfo
    {
        public string IPAddress;
        public int port;
    }

    class MyServer
    {
        private static MyServer instance;
        public static MyServer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MyServer();
                }
                return instance;
            }
        }

        NetPeer server;
        NetworkInfo networkInfo;

        private MyServer()
        {
        }

        public void Setup(NetPeerConfiguration config)
        {
            server = new NetPeer(config);
            server.Start();

            networkInfo = new NetworkInfo();
            networkInfo.IPAddress = server.Configuration.BroadcastAddress.ToString();
            networkInfo.port = server.Port;
        }

        public void Update()
        {

            if (server != null)
            {
                ReadInData();

                SendData();
            }
        }

        public List<NetConnection> getConnectionList()
        {
            return server.Connections;
        }

        public NetworkInfo getNetworkInfo()
        {
            return networkInfo;
        }

        void ReadInData()
        {
            NetworkMessage client_input = new NetworkMessage();

            NetIncomingMessage im;
            while ((im = server.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        byte[] msg = im.ReadBytes(im.LengthBytes);
                        client_input = NetworkMessage.Deserialize(msg);

                        OutputQueue.AddToQueue(client_input.GetData());

                        Debug.WriteLine(msg);
                        break;
                    case NetIncomingMessageType.DiscoveryRequest:
                        // Create a response
                        NetOutgoingMessage om = server.CreateMessage();
                        om.Write("Connecting to DOG server");
                        server.SendDiscoveryResponse(om, im.SenderEndPoint);
                        break;

                    case NetIncomingMessageType.UnconnectedData:
                        Debug.WriteLine("Received from " + im.SenderEndPoint + ": " + im.ReadString() + Environment.NewLine);
                        break;
                }
                server.Recycle(im);
            }
        }

        void SendData()
        {

            if (server.ConnectionsCount > 0)
            {
                /*
                ServerMessage sm = new ServerMessage();
                sm.p1_x = 0;
                sm.p1_y = 0;
                sm.p2_x = 0;
                sm.p2_y = 0;

                NetOutgoingMessage om = server.CreateMessage();

                om.Write(sm.Serialize());
                server.SendMessage(om, server.Connections, NetDeliveryMethod.ReliableOrdered, 4);

                server.FlushSendQueue();
                */
            }

        }

        public void SendMessageToClient(DataMessage msg)
        {
            if (server.ConnectionsCount > 0)
            {
                NetworkMessage sm = new NetworkMessage();

                
                // Temporary
                switch(msg.type)
                {
                    case DataMessage_Type.GAME_STATE:
                        GameStateMessage gameStateMsg = msg as GameStateMessage;
                        sm.Set_Data(gameStateMsg);
                        break;
                    case DataMessage_Type.PLAYER_UPDATE:
                        PlayerUpdateMessage playerMsg = msg as PlayerUpdateMessage;
                        sm.Set_Data(playerMsg);
                        break;
                    case DataMessage_Type.FENCE_HIT:
                        FenceHitMessage fMsg = msg as FenceHitMessage;
                        sm.Set_Data(fMsg);
                        break;
                    case DataMessage_Type.MISSLE_MINE:
                        MissileMineMessage MMMsg = msg as MissileMineMessage;
                        sm.Set_Data(msg);
                        break;
                    case DataMessage_Type.ROTATION:
                        RotationMessage rMsg = msg as RotationMessage;
                        sm.Set_Data(msg);
                        break;
                    case DataMessage_Type.MISSILEUPDATE:
                        MissileUpdateMessage tmsg = msg as MissileUpdateMessage;
                        sm.Set_Data(msg);
                        break;
                    case DataMessage_Type.GAME_OVER:
                        GAMEOVERMESSAGE gomsg = msg as GAMEOVERMESSAGE;
                        sm.Set_Data(msg);
                        break;

                    default:
                        break;
                }

                NetOutgoingMessage om = server.CreateMessage();

                om.Write(sm.Serialize());
                server.SendMessage(om, server.Connections, NetDeliveryMethod.ReliableOrdered, 4);

                server.FlushSendQueue();
            }

        }

    }
}
