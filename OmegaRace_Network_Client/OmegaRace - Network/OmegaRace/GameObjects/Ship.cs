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
    public class Ship : GameObject
    {
        float maxSpeed;
        float maxForce;
        float rotateSpeed;
        Vec2 heading;

        Azul.Color shipColor;

        int mineCount;
        int missileCount;

        Vec2 respawnPos;
        bool respawning;

        public Ship(Azul.Rect screenRect, Azul.Color color)
            : base (GAMEOBJECT_TYPE.SHIP, new Azul.Rect(0, 0, 32, 32), screenRect, GameObject.shipTexture, color)
        {
            PhysicBody_Data data = new PhysicBody_Data();

            data.position = new Vec2(screenRect.x, screenRect.y);
            data.size = new Vec2(screenRect.width, screenRect.height);
            data.active = true;
            data.angle = 0;
            data.shape_type = PHYSICBODY_SHAPE_TYPE.SHIP_MANIFOLD;
            //data.isSensor = true;
            CreatePhysicBody(data);

            // maxSpeed is m/s
            maxSpeed = 150.0f;

            maxForce = 20f;
            rotateSpeed = 5.0f;
            heading = new Vec2((float)System.Math.Cos(pBody.GetAngleDegs()), (float)System.Math.Sin(pBody.GetAngleDegs()));

            mineCount = 5;
            missileCount = 3;
            shipColor = color;

            respawnPos = new Vec2(screenRect.x, screenRect.y) ;
        }

        public override void Update()
        {
            if (respawning == false)
            {
                base.Update();
                LimitSpeed();
                UpdateDirection();
            }
            else
            {
                pBody.SetPhysicalPosition(respawnPos);
                respawning = false;
            }
            
        }


        public override void Draw()
        {
            base.Draw();
        }

        public Azul.Color getColor()
        {
            return shipColor;
        }

        public void Move(int vertInput)
        {
            if(vertInput < 0)
            {
                vertInput = 0;
            }
            pBody.ApplyForce(heading * vertInput * maxForce, GetPixelPosition());
        }

        public void Rotate(int horzInput)
        {
            pBody.SetAngularVelocity(0);
            pBody.SetAngle(pBody.GetAngleDegs() + (horzInput * -rotateSpeed));
        }


        

        public void SetAngle(float angle)
        {

            pBody.SetAngle(angle);
            this.UpdateDirection();
        }

        public void LimitSpeed()
        {
            Vec2 shipVel = pBody.GetPhysicalVelocity();
            float magnitude = shipVel.Length();

            if(magnitude > maxSpeed)
            {
                shipVel.Normalize();
                shipVel *= maxSpeed;
                pBody.GetBody().SetLinearVelocity(shipVel);
            }


        }

        public bool UseMine()
        {
            bool output = false;

            if (mineCount > 0)
            {
                mineCount--;
                output = true;
            }
            return output;
        }

        public bool UseMissile()
        {
            bool output = false;

            if (missileCount > 0)
            {
                missileCount--;
                output = true;
            }
            return output;
        }

        public int MissileCount()
        {
            return missileCount;
        }

        public void GiveMissile()
        {
            if (missileCount < 3)
            {
                missileCount++;
            }
        }

        public int MineCount()
        {
            return mineCount;
        }

        public void GiveMine()
        {
            if (mineCount < 5)
            {
                mineCount++;
            }
        }

        public void Respawn(Vec2 v)
        {
            respawning = true;
            respawnPos = v;
        }

        void UpdateDirection()
        {
            heading = new Vec2((float)System.Math.Cos(pSprite.angle), (float)System.Math.Sin(pSprite.angle));
        }

        public Vec2 GetHeading()
        {
            return heading;
        }

        public void SetAng(float x)
        {
            pSprite.angle = x;
            
        }

        public void OnHit()
        {
            GameManager.PlayerKilled(this);
        }

        public override void Accept(GameObject obj)
        {
            obj.VisitShip(this);
        }

        public override void VisitMissile(Missile m)
        {
            if (m.GetOwnerID() != getID())
            {
                m.OnHit();
                OnHit();
            }
        }
        public override void VisitMine(Mine m)
        {
            if (m.state == MINE_STATE.ARMED)
            {
                m.OnHit();
                OnHit();
            }
        }

        public override void VisitFence(Fence f)
        {
            f.OnHit();
        }
    }
}
