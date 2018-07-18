using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.IO;

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
        NetPeer server;

        public Lobby()
        {
            state = LOBBY_STATE.START;

        }



        public void Update()
        {
            if(state == LOBBY_STATE.START)
            {
                if (Azul.Input.GetKeyState(Azul.AZUL_KEY.KEY_H))
                {
                    state = LOBBY_STATE.HOST;
                    GameManager.SetAsServer();

                    NetPeerConfiguration config = new NetPeerConfiguration("Connected Test");
                    config.AcceptIncomingConnections = true;
                    config.MaximumConnections = 100;
                    config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
                    config.Port = 14240;

                    //server = new NetPeer(config);
                    //server.Start();

                    MyServer.Instance.Setup(config);

                }
                

            }
            else if(state == LOBBY_STATE.HOST)
            {
               

                if(Azul.Input.GetKeyState(Azul.AZUL_KEY.KEY_SPACE))
                {
                    GameStateMessage msg1 = new GameStateMessage(SEND_TYPE.LOCAL, GAME_STATE.PLAY);
                    GameStateMessage msg2 = new GameStateMessage(SEND_TYPE.NETWORKED, GAME_STATE.PLAY);

                    InputQueue.AddToQueue(msg1);
                    InputQueue.AddToQueue(msg2);
                }

                
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
                SpriteFont SelectionMessage1 = new SpriteFont("Press H to Host", 10, 400);
                SelectionMessage1.Update();
                SelectionMessage1.Draw();

            }
            else if (state == LOBBY_STATE.HOST)
            {
                SpriteFont msg1 = new SpriteFont("You are currently Hosting", 10, 400);
                msg1.Update();
                msg1.Draw();

                NetworkInfo temp = MyServer.Instance.getNetworkInfo();

                SpriteFont msg2 = new SpriteFont("Server IP: " + temp.IPAddress + " Port: " + temp.port, 50, 350);
                msg2.Update();
                msg2.Draw();

                SpriteFont msg3 = new SpriteFont("Connected Clients", 10, 300);
                msg3.Update();
                msg3.Draw();

                List<NetConnection> connections = MyServer.Instance.getConnectionList();
                int numConnections = connections.Count;
                for(int i = 0; i < numConnections; i++)
                {
                    NetConnection c = connections[i];
                    
                    SpriteFont tmp = new SpriteFont("Client " + i + " - IP: " + c.RemoteEndPoint.Address.ToString() + " Port: " + c.RemoteEndPoint.Port, 20, 250 - (i * 25));
                    tmp.Update();
                    tmp.Draw();
                }

            }

        }



    }
}
