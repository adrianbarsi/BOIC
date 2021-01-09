using BOIC.Code.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOIC.Code
{
    public class Follower : IEntity, IEnemy, IDestroyable
    {
        public const float POTION_DROP_PROBABILITY = 1;

        private const float INVINCIBILITY_TIME = 0.5f;
        private readonly Room room;
        public IShapeF Bounds { get; }
        private Vector2 speed = new Vector2(1, 1);
        private Vector2 velocity;
        public int Hp { get; set; } = 3;
        private bool invincible = false;
        private float currentTime = 0;
        private float timeOfInvincibility = 0;

        public bool Destroy { get; set; }
        public int CollisionDamage { get; set; } = 1;
        public float PotionDropProbability { get; set; }

        public Follower(Room room, CircleF circleF)
        {
            this.room = room;
            Bounds = circleF;
            PotionDropProbability = 0.5f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle((CircleF)Bounds, 100, Color.Red, 3f);
        }

        public void Update(GameTime gameTime)
        {
            currentTime += gameTime.GetElapsedSeconds();

            Vector2 delta = new Vector2
            {
                X = room.Game.Player.Bounds.Position.X - Bounds.Position.X,
                Y = room.Game.Player.Bounds.Position.Y - Bounds.Position.Y
            };

            delta.Normalize();

            velocity = delta * speed * gameTime.GetElapsedSeconds() * 100;

            Bounds.Position += velocity;

            if (invincible && currentTime > timeOfInvincibility + INVINCIBILITY_TIME)
            {
                invincible = false;
            }
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            if(collisionInfo.Other is Player)
            {
                Bounds.Position -= collisionInfo.PenetrationVector + velocity;
            }
            else if (collisionInfo.Other is PlayerProjectile && !invincible)
            {
                Hp -= ((PlayerProjectile) collisionInfo.Other).Damage;
                timeOfInvincibility = currentTime;
                invincible = true;
                Bounds.Position -= collisionInfo.PenetrationVector * 2;
                if (Hp <= 0)
                {
                    Task.Delay(150).ContinueWith(t =>
                    {
                        Destroy = true;
                    });
                }
            }
            else
            {
                Bounds.Position -= collisionInfo.PenetrationVector;
            }
        }
    }
}