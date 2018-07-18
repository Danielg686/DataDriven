﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box2DX.Dynamics;
using Box2DX.Collision;
using Box2DX.Common;

namespace OmegaRace
{
    public enum GAMEOBJECT_TYPE
    {
        NULL,
        SHIP,
        MISSILE,
        FENCEPOST,
        FENCE,
        MINE,
        PARTICLE
    }

    public class GameObject : Visitor
    {
        
        public static Azul.Texture shipTexture = new Azul.Texture("PlayerShip.tga");
        public static Azul.Texture missileTexture = new Azul.Texture("Missile.tga");
        public static Azul.Texture mineTexture = new Azul.Texture("Mine.tga");
        public static Azul.Texture mineTexture2 = new Azul.Texture("Mine2.tga");
        public static Azul.Texture fenceTexture = new Azul.Texture("FenceTall1.tga");
        public static Azul.Texture fencePostTexture = new Azul.Texture("FencePost.tga");
        
        public GAMEOBJECT_TYPE type;

        protected Azul.Sprite pSprite;
        protected Azul.Rect pScreenRect;
        protected Azul.Color color;
        protected float angle;
        protected bool updatebody;

        protected PhysicBody pBody;
        
        static int IDNUM;
        int id;
        
        bool alive;
        

        public GameObject(GAMEOBJECT_TYPE _type, Azul.Rect textureRect, Azul.Rect screenRect, Azul.Texture text, Azul.Color c)
        {
            type = _type;
            color = c;
            pSprite = new Azul.Sprite(text, textureRect, screenRect, color);
            pScreenRect = screenRect;
            id = IDNUM++;
            alive = true;
        }

        public int getID()
        {
            return id;
        }

        public Azul.Rect getDestRect()
        {
            return pScreenRect;
        }

        public virtual void Update()
        {
           // if (updatebody)
           // {
            //    if (pBody != null)
           //     {
            //        pushPhysics();
            //    }

            //    pSprite.x = pScreenRect.x;
           //     pSprite.y = pScreenRect.y;
            //}
            pSprite.Update();
        }

        public void SetPixelPosition(float _x, float _y)
        {
            pSprite.x = _x;
            pSprite.y = _y;
        }


        public bool isAlive()
        {
            return alive;
        }
        public void setAlive(bool b)
        {
            alive = b;
        }

        public void CreatePhysicBody(PhysicBody_Data _data)
        {
            pBody = new PhysicBody(_data, this);
        }

        public virtual void Draw()
        {
            // Hack to fix texture loss bug
            //pSprite.SwapTexture(texture);
            
            pSprite.Render();
        }

        public Vec2 GetPixelPosition()
        {
            return new Vec2(pSprite.x, pSprite.y);
        }

        public void SetPixelVelocity(Vec2 v)
        {
            pBody.SetPixelVelocity(v);
        }

        public Vec2 GetPixelVelocity()
        {
            return pBody.GetPhysicalVelocity() * PhysicWorld.METERSTOPIXELS;
        }

        public Vec2 GetPhysicalVelocity()
        {
            return pBody.GetPhysicalVelocity();
        }

        public float GetAngle_Deg()
        {
            return pBody.GetAngleDegs();
        }
        public float GetAngle_Rad()
        {
            return pBody.GetAngleRads();
        }


        void pushPhysics()
        {
            Vec2 bodyPos = pBody.GetPixelPosition();
            pSprite.angle = pBody.GetAngleRads();

            pScreenRect.x = bodyPos.X;
            pScreenRect.y = bodyPos.Y;
        }

        public virtual void Destroy()
        {
            pSprite = null;

            if (pBody != null)
            {
                Body b = pBody.GetBody();
                if (b != null)
                {
                    PhysicWorld.GetWorld().DestroyBody(pBody.GetBody());
                }
            }
            pBody = null;

        }

    }
}
