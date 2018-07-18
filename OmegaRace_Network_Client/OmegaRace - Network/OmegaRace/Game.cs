using System;
using System.Diagnostics;
using Lidgren.Network;
using System.Net;
using System.Runtime.InteropServices;
using System.IO;


namespace OmegaRace
{
    class NetworkGame : Azul.Game
    {
        float prevTime;


        int a;

        //-----------------------------------------------------------------------------
        // Game::Initialize()
        //		Allows the engine to perform any initialization it needs to before 
        //      starting to run.  This is where it can query for any required services 
        //      and load any non-graphic related content. 
        //-----------------------------------------------------------------------------
        public override void Initialize()
        {
            // Game Window Device setup
            this.SetWindowName("Omega Race");
            this.SetWidthHeight(800, 500);
            this.SetClearColor(0.2f, 0.2f, 0.2f, 1.0f);

            NetPeerConfiguration config = new NetPeerConfiguration("OmegaRaceConnection");
            config.AcceptIncomingConnections = true;
            config.MaximumConnections = 10;
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = 14240;

            
        }

        //-----------------------------------------------------------------------------
        // Game::LoadContent()
        //		Allows you to load all content needed for your engine,
        //	    such as objects, graphics, etc.
        //-----------------------------------------------------------------------------
        public override void LoadContent()
        {
            InputQueue.Instance();
            OutputQueue.Instance();

            PhysicWorld.Instance();
            GameManager.Instance();
            ParticleSpawner.Instance();
            AudioManager.Instance();


            prevTime = GetTime();

            a =33;
        }

        //-----------------------------------------------------------------------------
        // Game::Update()
        //      Called once per frame, update data, tranformations, etc
        //      Use this function to control process order
        //      Input, AI, Physics, Animation, and Graphics
        //-----------------------------------------------------------------------------

       // static int number = 0;
        public override void Update()
        {
            float curTime = GetTime();
            float gameElapsedTime = curTime - prevTime;

            PhysicWorld.Update(gameElapsedTime);

            InputManager.Update();
            CheckForInput();

            GameManager.Update(gameElapsedTime);

            MyClient.Instance.Update();

            InputQueue.Process();
            
            OutputQueue.Process();

            GameManager.CleanUp();

            prevTime = curTime;

             
        }

        //-----------------------------------------------------------------------------
        // Game::Draw()
        //		This function is called once per frame
        //	    Use this for draw graphics to the screen.
        //      Only do rendering here
        //-----------------------------------------------------------------------------
        public override void Draw()
        {
            GameManager.Draw();

            //debug.Draw();
            
        }

        //-----------------------------------------------------------------------------
        // Game::UnLoadContent()
        //       unload content (resources loaded above)
        //       unload all content that was loaded before the Engine Loop started
        //-----------------------------------------------------------------------------
        public override void UnLoadContent()
        {
        }

        void CheckForInput()
        {
            if (GameManager.GetGameState() == GAME_STATE.PLAY)
            {
                int p1_H = InputManager.GetAxis(INPUTAXIS.HORIZONTAL_P1);
                int p1_V = InputManager.GetAxis(INPUTAXIS.VERTICAL_P1);
                bool p1_Missile = InputManager.GetButtonDown(INPUTBUTTON.P1_FIRE);


                //PlayerInputMessage p1msg = new PlayerInputMessage(SEND_TYPE.NETWORKED, 1, p1_H, p1_V, p1_Missile);

                //InputQueue.AddToQueue(p1msg);

                int p2_H = InputManager.GetAxis(INPUTAXIS.HORIZONTAL_P2);
                int p2_V = InputManager.GetAxis(INPUTAXIS.VERTICAL_P2);
                bool p2_Missile = InputManager.GetButtonDown(INPUTBUTTON.P2_FIRE);
                bool p2_Mine = InputManager.GetButtonDown(INPUTBUTTON.P2_FIRE2);

                PlayerInputMessage p2msg = new PlayerInputMessage(SEND_TYPE.NETWORKED, 2, p2_H, p2_V, p2_Missile, p2_Mine);

                InputQueue.AddToQueue(p2msg);

            }

        }

    }
}

