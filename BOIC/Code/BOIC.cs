using BOIC.Code;
using BOIC.Code.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;

namespace BOIC.Code
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class BOIC : Game
    {
        public const int MAP_WIDTH = 800;
        public const int MAP_HEIGHT = 800;

        private const int WALL_OFFSET = 60;

        private const int HEART_GAP = 20;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public readonly List<IEntity> entities = new();
        public readonly List<IProp> props = new();
        public readonly List<IDecoration> decorations = new();
        public readonly List<IZone> zones = new();
        public readonly CollisionComponent collisionComponent;

        private Player player;
        private int heartCount = 0;

        public Player Player { get => player; }

        private Texture2D heartTexture;
        private Texture2D potionTexture;
        private Texture2D flyerTexture;
        private int heartOffset;
        public int HeartOffset { get => heartOffset; }
        private int currentHeartX;
        private int currentHeartY;

        public BOIC()
        {
            graphics = new GraphicsDeviceManager(this);
            collisionComponent = new CollisionComponent(new RectangleF(0, 0, MAP_WIDTH, MAP_HEIGHT));

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public List<Heart> initializeHearts(int numHearts)
        {
            var hearts = new List<Heart>();

            currentHeartX = HEART_GAP;
            currentHeartY = MAP_HEIGHT - heartOffset;
            for (int i = 0; i < numHearts; i++)
            {
                hearts.Add(new Heart(this, new Vector2(currentHeartX, currentHeartY), heartTexture));
                currentHeartX += heartOffset;
            }
            return hearts;
        }

        protected override void Initialize()
        {
            base.Initialize();

            graphics.PreferredBackBufferWidth = MAP_WIDTH;
            graphics.PreferredBackBufferHeight = MAP_HEIGHT;
            graphics.ApplyChanges();

            spriteBatch = new SpriteBatch(GraphicsDevice);

            player = new Player(this, new RectangleF(200, 200, 50, 50));
            entities.Add(player);
            collisionComponent.Insert(player);

            heartTexture = Content.Load<Texture2D>("images/heart");
            potionTexture = Content.Load<Texture2D>("images/potion");
            flyerTexture = Content.Load<Texture2D>("images/rightArrow");

            heartOffset = heartTexture.Width + HEART_GAP;

            List<Heart> hearts = initializeHearts(player.Hearts);
            heartCount = hearts.Count;
            decorations.AddRange(hearts);

            Follower follower = new Follower(this, new CircleF(new Point2(100, 100), 25));
            entities.Add(follower);
            collisionComponent.Insert(follower);

            follower = new Follower(this, new CircleF(new Point2(400, 400), 25));
            entities.Add(follower);
            collisionComponent.Insert(follower);

            follower = new Follower(this, new CircleF(new Point2(700, 700), 25));
            entities.Add(follower);
            collisionComponent.Insert(follower);

            RectangleF bounds = new RectangleF(new Point2(0, MAP_HEIGHT / 2), new Size2(flyerTexture.Width, flyerTexture.Height));
            Flyer flyer = new Flyer(this, bounds, flyerTexture);
            entities.Add(flyer);
            collisionComponent.Insert(flyer);

            var leftBoundary = new MapBoundary(new RectangleF(new(0, 0), new(WALL_OFFSET, MAP_HEIGHT)));
            zones.Add(leftBoundary);
            collisionComponent.Insert(leftBoundary);

            var topBoundary = new MapBoundary(new RectangleF(new(WALL_OFFSET, 0), new(MAP_WIDTH - WALL_OFFSET, WALL_OFFSET)));
            zones.Add(topBoundary);
            collisionComponent.Insert(topBoundary);

            var rightBoundary = new MapBoundary(new RectangleF(new(MAP_WIDTH - WALL_OFFSET, WALL_OFFSET), new Size2(WALL_OFFSET, MAP_HEIGHT - WALL_OFFSET)));
            zones.Add(rightBoundary);
            collisionComponent.Insert(rightBoundary);

            var bottomBoundary = new MapBoundary(new RectangleF(new(WALL_OFFSET, MAP_HEIGHT - WALL_OFFSET), new(MAP_WIDTH - 2 * WALL_OFFSET, WALL_OFFSET)));
            zones.Add(bottomBoundary);
            collisionComponent.Insert(bottomBoundary);

            //Wall newWall = new(new(500, 100), new(600, 100), new RectangleF(new(500, 100), new(100, 7)));
            //props.Add(newWall);
            //collisionComponent.Insert(newWall);
        }

        public void destroyProjectile(PlayerProjectile pj)
        {
            for (int i = 0; i < props.Count; i++)
            {
                if (ReferenceEquals(pj, props[i]))
                {
                    props.RemoveAt(i);
                }
            }
            collisionComponent.Remove(pj);
        }

        public void generateProjectile(Vector2 position, Player.Direction direction)
        {
            PlayerProjectile pj = new PlayerProjectile(this, position, direction);
            props.Add(pj);
            collisionComponent.Insert(pj);
        }

        private bool didItDrop(float probability)
        {
            return new Random().NextDouble() <= probability;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i] is IDestroyable && ((IDestroyable)entities[i]).Destroy)
                {
                    ICollisionActor ca = entities[i];

                    if (ca is Follower && didItDrop(Follower.POTION_DROP_PROBABILITY))
                    {
                        Potion potion = new Potion(new RectangleF
                        {
                            Position = ca.Bounds.Position,
                            Size = new Size2(potionTexture.Width, potionTexture.Height)
                        }, ca.Bounds.Position, potionTexture);
                        entities.Add(potion);
                        collisionComponent.Insert(potion);
                    }

                    entities.RemoveAt(i);
                    collisionComponent.Remove(ca);
                    i--;
                    continue;
                }
                entities[i].Update(gameTime);
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

            if (heartCount > 0)
            {
                if (player.Hearts < heartCount)
                {
                    for (int i = decorations.Count - 1; i >= 0; i--)
                    {
                        if (decorations[i] is Heart)
                        {
                            decorations.RemoveAt(i);
                            heartCount--;
                            currentHeartX -= heartOffset;
                            break;
                        }
                    }
                }
                else if (player.Hearts > heartCount)
                {
                    decorations.Add(new Heart(this, new Vector2(currentHeartX, currentHeartY), heartTexture));
                    currentHeartX += heartOffset;
                    heartCount++;
                }
            }
            else
            {
                Exit();
            }

            collisionComponent.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.TransparentBlack);

            spriteBatch.Begin();

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

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
