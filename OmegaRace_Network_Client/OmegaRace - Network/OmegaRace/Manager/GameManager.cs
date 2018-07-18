using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;

namespace OmegaRace
{
    public enum GAME_STATE
    {
        LOADSCREEN,
        LOBBY,
        PLAY,
        ADD_POINT,
        GAME_OVER
    }

    public enum NETWORK_MODE
    {
        SERVER,
        CLIENT
    }

    public class GameManager 
    {
        static Azul.Texture loadScreenText = new Azul.Texture("MainMenu.tga");


        private static GameManager instance = null;
        public static GameManager Instance()
        {
            if (instance == null)
            {
                instance = new GameManager();
            }
            return instance;
        }

        // LOBBY STUFF ////////

        Lobby pLobby;

        ////////////////////

        // NETWORK STUFF ////

        NETWORK_MODE mode;
        
        ///////////
        
        List<GameObject> destroyList;
        List<GameObject> gameObjList;

        Ship player1;
        Ship player2;

        int p1Score;
        int p2Score;

        int p1Kills;
        int p2Kills;

        int p1Deaths;
        int p2Deaths;

        int p1Lives;
        int p2Lives;
        
       // bool Loading;
        bool clean;

        Azul.Sprite p1LifeDisplay;
        Azul.Sprite p2LifeDisplay;

        Azul.Sprite p2MineDisplay;
        Azul.Sprite p1MineDisplay;

        //Azul.Texture fontText32;
        Azul.Texture fontText20;
        SpriteFont p1ScoreText;
        SpriteFont p2ScoreText;
        SpriteFont p1KillText;
        SpriteFont p2KillText;
        SpriteFont p1DeathText;
        SpriteFont p2DeathText;
        SpriteFont GameOverText;

        Azul.Sprite mainMenuSprite;

        GAME_STATE state;


        private GameManager()
        {
            destroyList = new List<GameObject>();
            gameObjList = new List<GameObject>();

            mainMenuSprite = new Azul.Sprite(loadScreenText, new Azul.Rect(0, 0, 800, 480), new Azul.Rect(400, 250, 800, 500));
            
            state = GAME_STATE.LOADSCREEN;

            //fontText20 = new Azul.Texture("Consolas20pt.tga");
            //GlyphMan.AddXml("Consolas20pt.xml", fontText20);
            fontText20 = new Azul.Texture("Arial20pt.tga");
            GlyphMan.AddXml("Arial20pt.xml", fontText20);

            GameOverText = new SpriteFont("", 0, 0);

            pLobby = new Lobby();
        }

        private void FirstLoad()
        {
            LoadLevel();

            p1Score = 0;
            p2Score = 0;
            p1Lives = 3;
            p2Lives = 3;
            p1Kills = 0;
            p2Kills = 0;
            p1Deaths = 0;
            p2Deaths = 0;
        }


        public static void Update(float gameTime)
        {
            GameManager inst = Instance();

            switch (inst.state)
            {
                case GAME_STATE.LOBBY:
                    inst.pLobby.Update();
                    break;
                case GAME_STATE.LOADSCREEN:
                    inst.LoadScreenUpdate();
                    break;
                case GAME_STATE.PLAY:
                    inst.PlayScreenUpdate();
                    break;
                case GAME_STATE.ADD_POINT:
                    inst.AddPointUpdate();
                    break;
                case GAME_STATE.GAME_OVER:
                    inst.GameOverUpdate();
                    break;
            }
        }

        public static GAME_STATE GetGameState()
        {
            return instance.state;
        }

        private void LoadScreenUpdate()
        {
            mainMenuSprite.Update();

            if(InputManager.GetButtonDown(INPUTBUTTON.START) || InputManager.GetButtonDown(INPUTBUTTON.JUMP))
            {
                FirstLoad();
                state = GAME_STATE.LOBBY;
            }
        }

        private void AddPointUpdate()
        {
            if(clean == true)
            {
                if (p1Score >= 3 || p2Score >= 3)
                {
                    state = GAME_STATE.GAME_OVER;
                }
                else
                {
                    LoadLevel();
                    state = GAME_STATE.PLAY;
                }
            }
        }

        private void PlayScreenUpdate()
        {

            /*
            if (InputManager.GetButtonDown(INPUTBUTTON.P1_FIRE))
            {
                GameManager.FireMissile(player1);
            }

            if (InputManager.GetButtonDown(INPUTBUTTON.P1_FIRE2))
            {
                GameManager.LayMine(player1);
            }

            if (InputManager.GetButtonDown(INPUTBUTTON.P2_FIRE))
            {
                GameManager.FireMissile(player2);
            }

            if (InputManager.GetButtonDown(INPUTBUTTON.P2_FIRE2))
            {
                GameManager.LayMine(player2);
            }
            */
            
            


            for (int i = gameObjList.Count - 1; i >= 0; i--)
                {
                    gameObjList[i].Update();
                }
        }

        public static void RecieveMessage(DataMessage msg)
        {
            if (instance.state == GAME_STATE.LOBBY)
            {
                switch (msg.type)
                {
                    case DataMessage_Type.GAME_STATE:
                        instance.ProcessGameStateMessage(msg as GameStateMessage);
                        break;
                }
            }
            else if (instance.state == GAME_STATE.PLAY)
            {
                 switch (msg.type)
                {
                    case DataMessage_Type.PLAYER_INPUT:
                        PlayerInputMessage pMsg = msg as PlayerInputMessage;
                        instance.ProcessPlayerInputMessage(pMsg);
                        break;
                    case DataMessage_Type.PLAYER_UPDATE:
                        PlayerUpdateMessage updateMsg = msg as PlayerUpdateMessage;
                        instance.ProcessPlayerUpdateMessage(updateMsg);
                        break;
                    case DataMessage_Type.FENCE_HIT:
                        FenceHitMessage fenceMsg = msg as FenceHitMessage;
                        instance.ProcessFenceHitMessage(fenceMsg);
                        break;
                    case DataMessage_Type.MISSLE_MINE:
                        MissileMineMessage MMMsg = msg as MissileMineMessage;
                        instance.ProcessMissileMineMessage(MMMsg);
                        break;
                    case DataMessage_Type.ROTATION:
                        RotationMessage rMsg = msg as RotationMessage;
                        instance.ProcessRotationMessage(rMsg);
                        break;
                    case DataMessage_Type.MISSILEUPDATE:
                        MissileUpdateMessage MMsg = msg as MissileUpdateMessage;
                        instance.ProcessMissileUpdateMessage(MMsg);
                        break;
                    case DataMessage_Type.GAME_OVER:
                        GAMEOVERMESSAGE gomsg = msg as GAMEOVERMESSAGE;
                        instance.ProcessGameOverMessage(gomsg);
                        break;
                    default:
                        break;

                }
            }


        }

        void ProcessGameOverMessage(GAMEOVERMESSAGE msg)
        {
            state = GAME_STATE.GAME_OVER;
            DestroyAll();
        }

        void ProcessMissileUpdateMessage(MissileUpdateMessage msg)
        {
            for (int i = gameObjList.Count - 1; i >= 0; i--)
            {
                if (gameObjList[i] is Missile)
                {
                    Missile temp = gameObjList[i] as Missile;

                    if (temp.GetMissileID() == msg.MissileID)
                    {
                        temp.UpdatePos(msg.x_pos,msg.y_pos);
                        break;
                    }
                }
            }

        }

        void ProcessPlayerUpdateMessage(PlayerUpdateMessage msg)
        {
            if(msg.playerID == 1)
            {
                player1.SetPixelPosition(msg.x, msg.y);
                
            }
            else if (msg.playerID == 2)
            {
                player2.SetPixelPosition(msg.x, msg.y);
            }
        }

        void ProcessGameStateMessage(GameStateMessage msg)
        {
            state = GAME_STATE.PLAY;
        }

        void ProcessPlayerInputMessage(PlayerInputMessage msg)
        {
            if (msg.playerID == 1)
            {
                player1.Rotate(msg.horzInput);
                player1.Move(msg.vertInput);
                if (msg.fireMissile)
                {
                    
                    GameManager.FireMissile(player1);
                }
            }
            else if (msg.playerID == 2)
            {
                player2.Rotate(msg.horzInput);
                player2.Move(msg.vertInput);
                if(msg.fireMissile)
                {
                    GameManager.FireMissile(player2);
                }
            }
        }

        void ProcessFenceHitMessage(FenceHitMessage msg)
        {
            for (int i = gameObjList.Count - 1; i >= 0; i--)
            {
                if(gameObjList[i] is Fence)
                {
                    Fence temp = gameObjList[i] as Fence;

                    if (temp.GetFenceID() == msg.fence_id)
                    {
                        temp.OnHit();
                        break;
                    }
                }
            }
        }

        void ProcessMissileMineMessage(MissileMineMessage msg)
        {
            if(msg.PlayerID == 1)
            {
                if(msg.pMissile)
                {
                    Vec2 t = new Vec2(msg.direction_x, msg.direction_y);
                    GameManager.FireMissile(player1, t);
                }
                if(msg.Mine)
                {
                    GameManager.LayMine(player1);
                }
            }
            else if(msg.PlayerID == 2)
            {
                if (msg.pMissile)
                {
                    Vec2 t = new Vec2(msg.direction_x, msg.direction_y);
                    GameManager.FireMissile(player2, t);
                }
                if (msg.Mine)
                {
                    GameManager.LayMine(player2);
                }
            }
        }

        void ProcessRotationMessage(RotationMessage msg)
        {
            if (msg.playerID == 1)
            {
                player1.SetAng(msg.input_Ang);
            }
            else if (msg.playerID == 2)
            {
                player2.SetAng(msg.input_Ang);
            }
        }
        public void GameOverUpdate()
        {
            if (p1Score > p2Score)
            {
                GameOverText = new SpriteFont("P1 WINS", 400, 250);
            }
            else
            {
                GameOverText = new SpriteFont("P2 WINS", 400, 250);
            }
            GameOverText.Update();

            if (InputManager.GetButtonDown(INPUTBUTTON.JUMP))
            {
                state = GAME_STATE.LOADSCREEN;
            }
        }

        


        public static void Draw()
        {
            GameManager inst = Instance();

            switch (inst.state)
            {
                case GAME_STATE.LOBBY:
                    inst.pLobby.Draw();
                    break;
                case GAME_STATE.LOADSCREEN:
                    inst.LoadScreenDraw();
                    break;
                case GAME_STATE.PLAY:
                    inst.PlayScreenDraw();
                    break;
                case GAME_STATE.GAME_OVER:
                    inst.GameOverDraw();
                    break;
            }
        }

        private void LoadScreenDraw()
        {
            mainMenuSprite.Render();
        }

        private void PlayScreenDraw()
        {
            player1.Draw();
            player2.Draw();

            for (int i = 0; i < gameObjList.Count; i++)
            {
                gameObjList[i].Draw();
            }

            DisplayHUD();
        }

        private void GameOverDraw()
        {
            GameOverText.Draw();
        }


        public static void PlayerKilled(Ship s)
        {
            Instance().pPlayerKilled(s);
        }

        // Clean up this mess
        void pPlayerKilled(Ship shipKilled)
        {

            // Player 1 is Killed
            if(player1.getID() == shipKilled.getID())
            {
                p1Lives--;
                p1Deaths++;
                p2Kills++;

                if(p1Lives < 0)
                {
                    p2Score++;
                    p1Lives = 3;
                    p2Lives = 3;
                    DestroyAll();
                    state = GAME_STATE.ADD_POINT;
                }
                else
                {
                    if(player2.GetPixelPosition().Y > 250)
                    {
                        player1.Respawn(new Vec2(400, 100));
                    }
                    else
                    {
                        player1.Respawn(new Vec2(400, 400));
                    }
                }
            }
            // Player 2 is Killed
            else if (player2.getID() == shipKilled.getID())
            {
                p2Lives--;
                p2Deaths++;
                p1Kills++;

                if(p2Lives < 0)
                {
                    p2Score++;
                    p1Lives = 3;
                    p2Lives = 3;
                    DestroyAll();
                    state = GAME_STATE.ADD_POINT;
                }
                else
                {
                    if (player1.GetPixelPosition().Y > 250)
                    {
                        player2.Respawn(new Vec2(400, 100));
                    }
                    else
                    {
                        player2.Respawn(new Vec2(400, 400));
                    }
                }
            }
        }
        

        public static void MineDestroyed(Mine m)
        {
            GameManager inst = Instance();

            if(m.GetOwnerID() == inst.player1.getID())
            {
                inst.player1.GiveMine();
            }
            else if(m.GetOwnerID() == inst.player2.getID())
            {
                inst.player2.GiveMine();
                
            }

            AudioManager.PlaySoundEvent(AUDIO_EVENT.MINE_DESPAWN);

        }

        public static void MissileDestroyed(Missile m)
        {
            GameManager inst = Instance();

            if (m.GetOwnerID() == inst.player1.getID())
            {
                inst.player1.GiveMissile();
            }
            else if (m.GetOwnerID() == inst.player2.getID())
            {
                inst.player2.GiveMissile();
            }
        }

        public static void FireMissile(Ship ship, Vec2 direction)
        {
            if (ship.UseMissile())
            {
                ship.Update();
                Vec2 pos = ship.GetPixelPosition();
               // Vec2 direction = ship.GetHeading();
                Missile m = new Missile(new Azul.Rect(pos.X, pos.Y, 20, 5), ship.getID(), direction, ship.getColor());
                Instance().gameObjList.Add(m);
                AudioManager.PlaySoundEvent(AUDIO_EVENT.MISSILE_FIRE);
            }
        }

        public static void FireMissile(Ship ship)
        {
            if (ship.UseMissile())
            {
                ship.Update();
                Vec2 pos = ship.GetPixelPosition();
                Vec2 direction = ship.GetHeading();
                Missile m = new Missile(new Azul.Rect(pos.X, pos.Y, 20, 5), ship.getID(), direction, ship.getColor());
                Instance().gameObjList.Add(m);
                AudioManager.PlaySoundEvent(AUDIO_EVENT.MISSILE_FIRE);
            }
        }

        public static void LayMine(Ship ship)
        {
            if (ship.UseMine())
            {
                ship.Update();
                GameManager inst = Instance();
                Vec2 pos = ship.GetPixelPosition();

                Mine m = new Mine(new Azul.Rect(pos.X, pos.Y, 20, 20), ship.getID(), ship.getColor());
                Instance().gameObjList.Add(m);
                AudioManager.PlaySoundEvent(AUDIO_EVENT.MINE_LAYED);
            }
            
        }

        public static void AddGameObject(GameObject obj)
        {
            Instance().gameObjList.Add(obj);
        }

        public static void CleanUp()
        {
            foreach (GameObject obj in Instance().destroyList)
            {
                obj.Destroy();
            }

            Instance().destroyList.Clear();
            Instance().clean = true;
        }


        public void DestroyAll()
        {
            foreach(GameObject obj in gameObjList)
            {
                destroyList.Add(obj);
            }
            gameObjList.Clear();
            clean = false;
        }
            
        public static void DestroyObject(GameObject obj)
        {
            obj.setAlive(false);
            Instance().gameObjList.Remove(obj);

            Instance().destroyList.Add(obj);
            Instance().clean = false;
        }


        void DisplayHUD()
        {
            
          //  pLobby.Update();

            p1ScoreText = new SpriteFont(p1Score + "", 380, 285);
            p1ScoreText.Update();
            p1ScoreText.Draw();

            p2ScoreText = new SpriteFont(p2Score + "", 420, 285);
            p2ScoreText.Update();
            p2ScoreText.Draw();

            p1KillText = new SpriteFont("P1  KI LLS: " + p1Kills, 245, 220);
            p1KillText.Update();
            p1KillText.Draw();

            p2KillText = new SpriteFont("P2  KI LLS: " + p2Kills, 455, 220);
            p2KillText.Update();
            p2KillText.Draw();

            p1DeathText = new SpriteFont("P1  DEATHS: " + p1Deaths, 245, 200);
            p1DeathText.Update();
            p1DeathText.Draw();

            p2DeathText = new SpriteFont("P2  DEATHS: " + p2Deaths, 455, 200);
            p2DeathText.Update();
            p2DeathText.Draw();

            for (int i = 0; i < p1Lives; i++)
            {
                p1LifeDisplay.x = 265 + (i * 30);
                p1LifeDisplay.y = 285;
                p1LifeDisplay.Update();
                p1LifeDisplay.Render();
            }
            for (int i = 0; i < p2Lives; i++)
            {
                p2LifeDisplay.x = 475 + (i * 30);
                p2LifeDisplay.y = 285;
                p2LifeDisplay.Update();
                p2LifeDisplay.Render();
            }

            int p1MineCount = player1.MineCount();
            for (int i = 0; i < p1MineCount; i++)
            {
                p1MineDisplay.x = 235 + (i * 30);
                p1MineDisplay.y = 250;
                p1MineDisplay.Update();
                p1MineDisplay.Render();
            }
            int p2MineCount = player2.MineCount();
            for (int i = 0; i < p2MineCount; i++)
            {
                p2MineDisplay.x = 445 + (i * 30);
                p2MineDisplay.y = 250;
                p2MineDisplay.Update();
                p2MineDisplay.Render();
            }

        }
        

        void LoadLevel()
        {
            p1LifeDisplay = new Azul.Sprite(GameObject.shipTexture,
            new Azul.Rect(0, 0, 32, 32), new Azul.Rect(0, 0, 30, 30), new Azul.Color(0, 1, 0));
            p1LifeDisplay.angle = 90 * PhysicWorld.MATH_PI_180;
            p2LifeDisplay = new Azul.Sprite(GameObject.shipTexture,
            new Azul.Rect(0, 0, 32, 32), new Azul.Rect(0, 0, 30, 30), new Azul.Color(0, 0.5f, 1));
            p2LifeDisplay.angle = 90 * PhysicWorld.MATH_PI_180;

            p1MineDisplay = new Azul.Sprite(GameObject.mineTexture,
            new Azul.Rect(0, 0, 12, 12), new Azul.Rect(0, 0, 20, 20), new Azul.Color(0, 1, 0));
            p2MineDisplay = new Azul.Sprite(GameObject.mineTexture,
            new Azul.Rect(0, 0, 12, 12), new Azul.Rect(0, 0, 20, 20), new Azul.Color(0, 0.5f, 1.0f));
            
            player1 = new Ship(new Azul.Rect(400, 100, 30, 30), new Azul.Color(0, 1, 0));
            player2 = new Ship(new Azul.Rect(400, 400, 30, 30), new Azul.Color(0, 0.5f, 1));
            
            gameObjList.Add(player1);
            gameObjList.Add(player2);

            
            
            // OutsideBox
            gameObjList.Add(new FencePost(new Azul.Rect(5, 5, 10, 10)));
            gameObjList.Add(new FencePost(new Azul.Rect(200, 5, 10, 10)));
            gameObjList.Add(new FencePost(new Azul.Rect(400, 5, 10, 10)));
            gameObjList.Add(new FencePost(new Azul.Rect(600, 5, 10, 10)));
            gameObjList.Add(new FencePost(new Azul.Rect(800 - 5, 5, 10, 10)));

            gameObjList.Add(new FencePost(new Azul.Rect(0 + 5, 495, 10, 10)));
            gameObjList.Add(new FencePost(new Azul.Rect(200, 495, 10, 10)));
            gameObjList.Add(new FencePost(new Azul.Rect(400, 495, 10, 10)));
            gameObjList.Add(new FencePost(new Azul.Rect(600, 495, 10, 10)));
            gameObjList.Add(new FencePost(new Azul.Rect(800 - 5, 495, 10, 10)));

            gameObjList.Add(new FencePost(new Azul.Rect(5, 250, 10, 10)));
            gameObjList.Add(new FencePost(new Azul.Rect(795, 250, 10, 10)));

            // InsideBox
            
            gameObjList.Add(new FencePost(new Azul.Rect(200, 170, 10, 10)));
            gameObjList.Add(new FencePost(new Azul.Rect(400, 170, 10, 10)));
            gameObjList.Add(new FencePost(new Azul.Rect(600, 170, 10, 10)));
            gameObjList.Add(new FencePost(new Azul.Rect(200, 330, 10, 10)));
            gameObjList.Add(new FencePost(new Azul.Rect(400, 330, 10, 10)));
            gameObjList.Add(new FencePost(new Azul.Rect(600, 330, 10, 10)));
            

            // Fence OutsideBox
            
            gameObjList.Add(new Fence(new Azul.Rect(100, 5, 8, 200), 90));
            gameObjList.Add(new Fence(new Azul.Rect(300, 5, 8, 200), 90));
            gameObjList.Add(new Fence(new Azul.Rect(500, 5, 8, 200), 90));
            gameObjList.Add(new Fence(new Azul.Rect(700, 5, 8, 200), 90));

            gameObjList.Add(new Fence(new Azul.Rect(100, 495, 8, 200), 90));
            gameObjList.Add(new Fence(new Azul.Rect(300, 495, 8, 200), 90));
            gameObjList.Add(new Fence(new Azul.Rect(500, 495, 8, 200), 90));
            gameObjList.Add(new Fence(new Azul.Rect(700, 495, 8, 200), 90));

            gameObjList.Add(new Fence(new Azul.Rect(5, 125, 8, 250), 0));
            gameObjList.Add(new Fence(new Azul.Rect(5, 375, 8, 250), 0));
            gameObjList.Add(new Fence(new Azul.Rect(795, 125, 8, 250), 0));
            gameObjList.Add(new Fence(new Azul.Rect(795, 375, 8, 250), 0));

            // Fence InsideBox
            gameObjList.Add(new Fence(new Azul.Rect(300, 170, 10, 200), 90));
             gameObjList.Add(new Fence(new Azul.Rect(500, 170, 10, 200), 90));
             gameObjList.Add(new Fence(new Azul.Rect(300, 330, 10, 200), 90));
             gameObjList.Add(new Fence(new Azul.Rect(500, 330, 10, 200), 90));

             gameObjList.Add(new Fence(new Azul.Rect(200, 250, 10, 160), 0));
             gameObjList.Add(new Fence(new Azul.Rect(600, 250, 10, 160), 0));

        }


        // NETWORK STUFF ////////////

        public static void SetAsServer()
        {
            instance.mode = NETWORK_MODE.SERVER;


        }

        public static void SetAsClient()
        {
            instance.mode = NETWORK_MODE.CLIENT;

        }


        //////////

    }
}
