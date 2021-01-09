using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOIC.Code
{
    public class Textures
    {
        public Texture2D HeartTexture;
        public Texture2D PotionTexture;
        public Texture2D FlyerTexture;
        public Textures(Texture2D heartTexture, Texture2D potionTexture, Texture2D flyerTexture)
        {
            HeartTexture = heartTexture;
            PotionTexture = potionTexture;
            FlyerTexture = flyerTexture;
        }
    }
}
