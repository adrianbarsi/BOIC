using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Input;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using System;
using BOIC.Code.Components;
using BOIC.Code.Utility;

namespace BOIC.Code
{
    public class Player : IEntity
    {
        private const int PROJECTILE_OFFSET = 10;
        private const int INVINCIBILITY_TIME = 2;
        private const float SHOOTING_COOLDOWN_TIME = 1f;

        private readonly Room room;
        public IShapeF Bounds { get; }

        private Vector2 verticalSpeed = new Vector2(0, 4);
        private Vector2 horizontalSpeed = new Vector2(4, 0);
        private int hearts = 10;
        public int Hearts { get => hearts; }
        private bool invincible = false;
        private float currentTime = 0;
        private float timeOfInvincibility = 0;
        private bool shootingCooldown = false;
        private float timeOfShooting = 0;

        public Player(Room room, RectangleF rectangleF)
        {
            this.room = room;
            Bounds = rectangleF;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle((RectangleF)Bounds, invincible ? Color.Blue : Color.LightGreen, 3f, 1f);
        }

        public void Update(GameTime gameTime)
        {
            currentTime += gameTime.GetElapsedSeconds();

            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.IsKeyDown(Keys.W))
            {
                Bounds.Position -= verticalSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                Bounds.Position += verticalSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                Bounds.Position -= horizontalSpeed;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                Bounds.Position += horizontalSpeed;
            }

            if (shootingCooldown && currentTime > timeOfShooting + SHOOTING_COOLDOWN_TIME)
            {
                shootingCooldown = false;
            }
            else if (!shootingCooldown &&
                (keyboardState.IsKeyDown(Keys.Up) ||
                keyboardState.IsKeyDown(Keys.Down) ||
                keyboardState.IsKeyDown(Keys.Left) ||
                keyboardState.IsKeyDown(Keys.Right))
            )
            {
                RectangleF r = (RectangleF)Bounds;
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    room.generateProjectile(new Vector2(r.X + (r.Width / 2), r.Y - PROJECTILE_OFFSET), Direction.Up);
                }
                else if (keyboardState.IsKeyDown(Keys.Down))
                {
                    room.generateProjectile(new Vector2(r.X + (r.Width / 2), r.Y + r.Height + PROJECTILE_OFFSET + 5), Direction.Down);
                }
                else if (keyboardState.IsKeyDown(Keys.Left))
                {
                    room.generateProjectile(new Vector2(r.X - PROJECTILE_OFFSET, r.Y + (r.Height / 2)), Direction.Left);
                }
                else if (keyboardState.IsKeyDown(Keys.Right))
                {
                    room.generateProjectile(new Vector2(r.X + r.Width + PROJECTILE_OFFSET, r.Y + (r.Height / 2)), Direction.Right);
                }
                shootingCooldown = true;
                timeOfShooting = currentTime;
            }

            if (invincible && currentTime > timeOfInvincibility + INVINCIBILITY_TIME)
            {
                invincible = false;
            }
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            if (collisionInfo.Other is IEnemy && !invincible)
            {
                hearts -= ((IEnemy)collisionInfo.Other).CollisionDamage;
                timeOfInvincibility = currentTime;
                invincible = true;
                Bounds.Position -= collisionInfo.PenetrationVector;
            }
            else if (collisionInfo.Other is IConsumable)
            {
                if (collisionInfo.Other is Potion)
                {
                    hearts++;
                }
            }
            else
            {
                Bounds.Position -= collisionInfo.PenetrationVector;
            }
        }
    }
}
