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
    public class Potion : IConsumable
    {
        public IShapeF Bounds { get; }

        private Texture2D texture;
        private Vector2 position;
        private Rectangle srcRect;

        public bool Destroy { get; set; } = false;

        public Potion(IShapeF bounds, Vector2 position, Texture2D texture)
        {
            Bounds = bounds;
            this.texture = texture;
            this.position = position;
            this.srcRect = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, srcRect, Color.White);
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            if(collisionInfo.Other is Player)
            {
                Destroy = true;
            }
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
