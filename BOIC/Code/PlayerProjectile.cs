using BOIC.Code.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BOIC.Code.Player;

namespace BOIC.Code
{
    public class PlayerProjectile : IProp, IDestroyable
    {
        private const int PROJECTILE_RADIUS = 10;
        private const int SPEED = 4;

        private BOIC game;
        private Vector2 position;
        private Direction direction;
        public IShapeF Bounds { get; }
        public bool Destroy { get; set; }
        private Vector2 velocity;

        private int damage = 1;
        public int Damage { get => damage; }


        public PlayerProjectile(BOIC game, Vector2 position, Direction direction)
        {
            this.game = game;
            this.position = position;
            this.direction = direction;

            Point2 adjustedPoint = Vector2.Zero;

            switch (direction)
            {
                case Direction.Up:
                    velocity = new Vector2(0, -4);
                    adjustedPoint = new Point2(position.X, position.Y - PROJECTILE_RADIUS);
                    break;
                case Direction.Down:
                    velocity = new Vector2(0, 4);
                    adjustedPoint = new Point2(position.X, position.Y + PROJECTILE_RADIUS);
                    break;
                case Direction.Left:
                    velocity = new Vector2(-4, 0);
                    adjustedPoint = new Point2(position.X - PROJECTILE_RADIUS, position.Y);
                    break;
                case Direction.Right:
                    velocity = new Vector2(4, 0);
                    adjustedPoint = new Point2(position.X + PROJECTILE_RADIUS, position.Y);
                    break;
            }

            Bounds = new CircleF(adjustedPoint, PROJECTILE_RADIUS);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawCircle((CircleF)Bounds, 100, Color.LightGreen);
        }

        public void Update(GameTime gameTime)
        {
            Bounds.Position += velocity;
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            Destroy = true;
        }
    }
}
