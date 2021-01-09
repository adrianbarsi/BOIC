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
    public class Room
    {
        private BOIC game;
        public BOIC Game { get => game; }
        private Player player;
        private List<IEntity> entities = new();
        private List<IProp> props = new();
        private List<IDecoration> decorations = new();
        private List<IZone> zones = new();
        private CollisionComponent collisionComponent;
        private bool complete = false;
        public bool Complete { get => complete; }

        public Room(BOIC game, Player player, List<IEntity> entities, List<IProp> props, List<IDecoration> decorations, List<IZone> zones, CollisionComponent collisionComponent)
        {
            this.game = game;
            this.player = player;
            this.entities = entities;
            this.props = props;
            this.decorations = decorations;
            this.zones = zones;
            this.collisionComponent = collisionComponent;
        }

        public void generateProjectile(Vector2 position, Direction direction)
        {
            PlayerProjectile pj = new PlayerProjectile(position, direction);
            props.Add(pj);
            collisionComponent.Insert(pj);
        }

        private IConsumable dropConsumable(IEnemy enemy, Vector2 position)
        {
            if (new Random().NextDouble() <= enemy.PotionDropProbability)
            {
                return new Potion(new RectangleF
                {
                    Position = position,
                    Size = new Size2(game.Textures.PotionTexture.Width, game.Textures.PotionTexture.Height)
                }, position, game.Textures.PotionTexture);
            }
            else
            {
                return null;
            }
        }

        public void Update(GameTime gameTime)
        {
            int enemyCount = 0;
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i] is IDestroyable && ((IDestroyable)entities[i]).Destroy)
                {
                    if(entities[i] is IEnemy)
                    {
                        IEnemy enemy = (IEnemy)entities[i];
                        Vector2 enemyPosition = ((ICollisionActor)enemy).Bounds.Position;
                        IConsumable enemyDrop = dropConsumable(enemy, enemyPosition);
                        if(enemyDrop != null)
                        {
                            entities.Add(enemyDrop);
                            collisionComponent.Insert(enemyDrop);
                        }
                    }
                    ICollisionActor ca = entities[i];
                    entities.RemoveAt(i);
                    collisionComponent.Remove(ca);
                    i--;
                    continue;
                }
                if(entities[i] is IEnemy)
                {
                    enemyCount++;
                }
                entities[i].Update(gameTime);
            }
            if(enemyCount == 0)
            {
                complete = true;
            }

            for (int i = 0; i < props.Count; i++)
            {
                if (props[i] is IDestroyable && ((IDestroyable)props[i]).Destroy)
                {
                    ICollisionActor ca = props[i];
                    props.RemoveAt(i);
                    collisionComponent.Remove(ca);
                    i--;
                    continue;
                }
                props[i].Update(gameTime);
            }

            for (int i = 0; i < zones.Count; i++)
            {
                zones[i].Update(gameTime);
            }

            collisionComponent.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (IEntity entity in entities)
            {
                entity.Draw(spriteBatch);
            }

            foreach (IDecoration decoration in decorations)
            {
                decoration.Draw(spriteBatch);
            }

            foreach (IProp prop in props)
            {
                prop.Draw(spriteBatch);
            }
        }
    }
}
