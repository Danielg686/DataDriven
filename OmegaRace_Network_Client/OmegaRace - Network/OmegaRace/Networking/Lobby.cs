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
    class Lobby
    {
        enum LOBBY_STATE
        {
            START,
            HOST,
            CLIENT
        }

        LOBBY_STATE state;
        NetClient client;
        bool isConnected;

        public Lobby()
        {
            state = LOBBY_STATE.START;
            
        }


        public void Update()
        {
            
            if(state == LOBBY_STATE.START)
            {
                if(Azul.Input.GetKeyState(Azul.AZUL_KEY.KEY_J))
                {
                    state = LOBBY_STATE.CLIENT;

                   // GameManager.SetAsClient();

                    NetPeerConfiguration config = new NetPeerConfiguration("Connected Test");
                    config.AutoFlushSendQueue = false;
                    config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

                    MyClient.Instance.Setup(config);
                    //client = new NetClient(config);
                    //client.Start();
                    //isConnected = false;
                }

            }
            
            else if(state == LOBBY_STATE.CLIENT)
            {
                MyClient.Instance.Update();
                
            }



        }

        public void Draw()
        {
            // Welcome Message
            SpriteFont LobbyWelcomeText = new SpriteFont("Welcome To Lobby", 10, 450);
            LobbyWelcomeText.Update();
            LobbyWelcomeText.Draw();

            if (state == LOBBY_STATE.START)
            {
                SpriteFont SelectionMessage1 = new SpriteFont("Press J to Join", 10, 400);
                SelectionMessage1.Update();
                SelectionMessage1.Draw();
  
            }

            else if(state == LOBBY_STATE.CLIENT)
            {
                if(MyClient.Instance.isConnected() == false)
                {
                    SpriteFont msg1 = new SpriteFont("Attempting To Join", 10, 400);
                    msg1.Update();
                    msg1.Draw();
                }
                else
                {
                    SpriteFont msg1 = new SpriteFont("Connected To Server", 10, 400);
                    msg1.Update();
                    msg1.Draw();

                    NetworkInfo temp = MyClient.Instance.getConnectedServerInfo();

                    //SpriteFont msg2 = new SpriteFont("Server IP: " + temp.IPAddress + " Port: " + temp.port, 50, 350);
                    //msg2.Update();
                    //msg2.Draw();
                    

                }
                

                
            }

        }

        void AttemptConnection()
        {
            // Attempt Connection

            IPEndPoint ep = NetUtility.Resolve("localhost", 14240);

            if (client.GetConnection(ep) == null)
            {
                client.DiscoverKnownPeer(ep);
                isConnected = true;
            }
            else
            {
                Debug.WriteLine("You're already connected idiot");
            }
        }



    }
}
