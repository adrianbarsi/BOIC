using BOIC.Code.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOIC.Code
{
    public class Heart : IDecoration
    {
        private readonly BOIC game;
        private Vector2 position;
        private Texture2D texture;
        private Rectangle srcRect;

        public Heart(BOIC game, Vector2 position, Texture2D texture)
        {
            this.game = game;
            this.position = position;
            this.texture = texture;
            this.srcRect = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, srcRect, Color.White);
        }
    }
}
