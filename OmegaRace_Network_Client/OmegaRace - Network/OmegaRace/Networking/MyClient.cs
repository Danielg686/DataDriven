using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Lidgren.Network;
using System.Net;
using System.Runtime.InteropServices;
using System.IO;

namespace OmegaRace
{
    class MyClient
    {
        public string IPaddress = null;
        

        private static MyClient instance;
        public static MyClient Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MyClient();
                }
                return instance;
            }
        }

        NetClient client;
        bool pConnected;
        NetworkInfo myInfo;
        NetworkInfo connectedServerInfo;

        public MyClient()
        {
            //client = new NetClient(config);
            //client.Start();
            //isConnected = false;
           
        }

        public void Setup(NetPeerConfiguration config)
        {
            client = new NetClient(config);
            client.Start();
            pConnected = false;


        }

        public void Update()
        {
            if (client != null)
            {
                ReadInData();

                if (client.ConnectionStatus == NetConnectionStatus.Connected)
                {
                    pConnected = true;
                }

                if (pConnected == true)
                {

                    SendData();
                    
                }
                else
                {
                    AttemptConnection();
                }
            }
        }

        public bool isConnected()
        {
            return pConnected;
        }

        void SendData()
        {
            /*
            ClientMessage pInput = new ClientMessage();
            pInput.horzVal = InputManager.GetAxis(INPUTAXIS.HORIZONTAL_P1);
            pInput.vertVal = InputManager.GetAxis(INPUTAXIS.VERTICAL_P1);

            NetOutgoingMessage om = client.CreateMessage();

            om.Write(pInput.Serialize());

            client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);

            client.FlushSendQueue();
            */
        }

        public void SendMessageToServer(DataMessage msg)
        {
            if (pConnected)
            {
                NetworkMessage sm = new NetworkMessage();


                // Temporary
                switch (msg.type)
                {
                   
                    case DataMessage_Type.PLAYER_INPUT:
                        PlayerInputMessage playerMsg = msg as PlayerInputMessage;
                        sm.Set_Data(playerMsg);
                        break;
                }

                NetOutgoingMessage om = client.CreateMessage();

                om.Write(sm.Serialize());
                client.SendMessage(om, NetDeliveryMethod.ReliableOrdered);

                //client.RawSend()

                client.FlushSendQueue();
            }
        }

        void ReadInData()
        {
            NetworkMessage sm = new NetworkMessage();

            NetIncomingMessage im;
            while ((im = client.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        byte[] msg = im.ReadBytes(im.LengthBytes);
                        sm = NetworkMessage.Deserialize(msg);

                        OutputQueue.AddToQueue(sm.GetData());

                        break;
                    case NetIncomingMessageType.DiscoveryResponse:
                        Debug.WriteLine("Found server at " + im.SenderEndPoint + " name: " + im.ReadString());
                        client.Connect(im.SenderEndPoint);
                        break;
                    case NetIncomingMessageType.UnconnectedData:
                        Debug.WriteLine("Received from " + im.SenderEndPoint + ": " + im.ReadString() + Environment.NewLine);
                        break;
                }
                client.Recycle(im);
            }
        }


        public NetworkInfo getClientInfo()
        {
            return myInfo;
        }
        public NetworkInfo getConnectedServerInfo()
        {
            return connectedServerInfo;
        }

        public void SetIPAddress(String s)
        {
            instance.IPaddress = s;
        }

        public bool AttemptConnection()
        {
            // Attempt Connection
            IPEndPoint ep;

            if (IPaddress != "")
            {
                ep = NetUtility.Resolve(IPaddress, 14240);

            }
            else
            {
                ep = NetUtility.Resolve("localhost", 14240);
            }

            if (client.GetConnection(ep) == null)
            {
                
                    client.DiscoverKnownPeer(ep);

                    connectedServerInfo.IPAddress = ep.ToString();
                    connectedServerInfo.port = ep.Port;

                    myInfo.IPAddress = client.Configuration.BroadcastAddress.ToString();
                    myInfo.port = client.Port;

                //if (client.ConnectionStatus != NetConnectionStatus.Disconnected)
                //{
                    //pConnected = true;
               // }
            }
            else
            {
                Debug.WriteLine("You're already connected idiot");
            }

            return pConnected;
        }

    }
}
