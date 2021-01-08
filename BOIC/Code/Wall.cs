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
    public class Wall : IProp
    {
        public const int LINE_WIDTH = 7;

        private Vector2 point1;
        private Vector2 point2;
        public IShapeF Bounds { get; }

        public Wall(Vector2 point1, Vector2 point2, IShapeF Bounds)
        {
            this.point1 = point1;
            this.point2 = point2;
            this.Bounds = Bounds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawLine(point1, point2, Color.Red, LINE_WIDTH);
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
        }
        public void Update(GameTime gameTime)
        {
        }
    }
}
