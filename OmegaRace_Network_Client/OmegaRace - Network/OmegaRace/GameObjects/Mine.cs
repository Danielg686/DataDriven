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
    public enum MINE_STATE
    {
        LAYED,
        ARMED
    }

    public class Mine : GameObject
    {
        int ownerID;
        float lifeTime;
        Stopwatch lifeTimer;
        Stopwatch animTimer;
        Stopwatch armedTimer;


        Azul.Sprite mine1;
        Azul.Sprite mine2;

        bool drawMine2;


        public MINE_STATE state;
        float armDelay;

        public Mine(Azul.Rect destRect, int owner, Azul.Color color)
            : base(GAMEOBJECT_TYPE.MINE, new Azul.Rect(0, 0, 12, 12), destRect, GameObject.mineTexture, color)
        {
            PhysicBody_Data data = new PhysicBody_Data();

            data.position = new Vec2(destRect.x, destRect.y);
            data.size = new Vec2(destRect.width, destRect.height);
            //data.radius = 25f;
            data.isSensor = true;
            data.angle = 0;
            data.shape_type = PHYSICBODY_SHAPE_TYPE.DYNAMIC_BOX;

            ownerID = owner;

            lifeTime = 5.0f;

            armDelay = 2.0f;

            mine1 = pSprite;
            mine2 = new Azul.Sprite(GameObject.mineTexture2, new Azul.Rect(0, 0, 12, 12), destRect, new Azul.Color(1, 0, 0));

            drawMine2 = false;

            CreatePhysicBody(data);
            lifeTimer = new Stopwatch();
            
            animTimer = new Stopwatch();
            animTimer.Start();
            armedTimer = new Stopwatch();
            armedTimer.Start();
            

            state = MINE_STATE.LAYED;

        }

        public override void Update()
        {
            base.Update();

            switch(state)
            {
                case MINE_STATE.LAYED:
                    LayedUpdate();
                    break;
                case MINE_STATE.ARMED:
                    ArmedUpdate();
                    break;
            }

        }

        public override void Draw()
        {
            switch (state)
            {
                case MINE_STATE.LAYED:
                    LayedDraw();
                    break;
                case MINE_STATE.ARMED:
                    ArmedDraw();
                    break;
            }

        }

        private void LayedUpdate()
        {
            TimeSpan ts = armedTimer.Elapsed;

            if(ts.Seconds > armDelay)
            {
                state = MINE_STATE.ARMED;
                lifeTimer.Start();
                AudioManager.PlaySoundEvent(AUDIO_EVENT.MINE_ARMED);
            }
        }

        private void LayedDraw()
        {
            mine1.Render();
        }

        private void ArmedUpdate()
        {

            mine2.x = pScreenRect.x;
            mine2.y = pScreenRect.y;
            mine2.Update();

            TimeSpan ts = lifeTimer.Elapsed;

            if (lifeTime <= ts.Seconds)
            {
                GameManager.DestroyObject(this);
            }

            TimeSpan animTs = animTimer.Elapsed;
            if (animTs.Milliseconds > 300f)
            {
                drawMine2 = !drawMine2;

                animTimer.Restart();
            }
        }

        private void ArmedDraw()
        {
            if (drawMine2)
            {
                mine2.Render();
            }

            mine1.Render();
        }


        public override void Destroy()
        {
            AudioManager.PlaySoundEvent(AUDIO_EVENT.MISSILE_FIRE);
            ParticleSpawner.SpawnParticleEvent(PARTICLE_EVENT.EXPLOSION, this);
            base.Destroy();
            GameManager.MineDestroyed(this);
        }

        public int GetOwnerID()
        {
            return ownerID;
        }

        public void OnHit()
        {
            
            GameManager.DestroyObject(this);
        }

        public override void Accept(GameObject obj)
        {
            obj.VisitMine(this);
        }
        public override void VisitFence(Fence f)
        {
            OnHit();
        }

        public override void VisitFencePost(FencePost fp)
        {
            OnHit();
        }

        public override void VisitMissile(Missile m)
        {
        }
        public override void VisitShip(Ship s)
        {
            if (state == MINE_STATE.ARMED)
            {
                OnHit();
                s.OnHit();
            }
        }
    }
}
