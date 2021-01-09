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
    public class Flyer : IEntity, IEnemy, IDestroyable
    {
        private Room room;
        public IShapeF Bounds { get; }
        public bool Destroy { get; set; } = false;
        public int Hp { get; set; } = 3;
        public int CollisionDamage { get; set; } = 1;
        public float PotionDropProbability { get; set; }

        private Texture2D texture;
        private Rectangle srcRect;

        private Vector2 velocity = new Vector2(4, 0);
        private Random random = new Random();

        public Flyer(Room room, IShapeF bounds, Texture2D texture)
        {
            this.room = room;
            Bounds = bounds;
            this.texture = texture;
            this.srcRect = new Rectangle(0, 0, texture.Width, texture.Height);
            PotionDropProbability = 0.4f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Bounds.Position, srcRect, Color.White);
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            if(collisionInfo.Other is PlayerProjectile)
            {
                Hp -= ((PlayerProjectile)collisionInfo.Other).Damage;
            }
        }

        public void Update(GameTime gameTime)
        {
            Bounds.Position += velocity;
            if(Bounds.Position.X > BOIC.MAP_WIDTH)
            {
                Bounds.Position = new Vector2(-texture.Width, random.Next(0, BOIC.MAP_HEIGHT - texture.Height - room.Game.HeartOffset));
            }
            if (Hp <= 0)
            {
                Destroy = true;
            }
        }
    }
}
