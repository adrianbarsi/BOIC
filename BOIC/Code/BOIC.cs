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
    public class BOIC : Game
    {
        public const int MAP_WIDTH = 800;
        public const int MAP_HEIGHT = 800;

        private const int WALL_OFFSET = 60;

        private const int HEART_GAP = 20;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Player player;

        public Player Player { get => player; }

        private Textures textures;
        public Textures Textures { get => textures; }
        private List<Heart> hearts = new();
        private int heartOffset;
        public int HeartOffset { get => heartOffset; }
        private int currentHeartX;
        private int currentHeartY;

        private Room room;

        public BOIC()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public void initializeHearts(int numHearts)
        {
            currentHeartX = HEART_GAP;
            currentHeartY = MAP_HEIGHT - heartOffset;
            for (int i = 0; i < numHearts; i++)
            {
                hearts.Add(new Heart(this, new Vector2(currentHeartX, currentHeartY), textures.HeartTexture));
                currentHeartX += heartOffset;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();

            graphics.PreferredBackBufferWidth = MAP_WIDTH;
            graphics.PreferredBackBufferHeight = MAP_HEIGHT;
            graphics.ApplyChanges();

            spriteBatch = new SpriteBatch(GraphicsDevice);

            var heartTexture = Content.Load<Texture2D>("images/heart");
            var potionTexture = Content.Load<Texture2D>("images/potion");
            var flyerTexture = Content.Load<Texture2D>("images/rightArrow");

            textures = new Textures(heartTexture, potionTexture, flyerTexture);
            heartOffset = textures.HeartTexture.Width + HEART_GAP;

            var entities = new List<IEntity>();
            var props = new List<IProp>();
            var decorations = new List<IDecoration>();
            var zones = new List<IZone>();
            var collisionComponent = new CollisionComponent(new RectangleF(0, 0, MAP_WIDTH, MAP_HEIGHT));

            room = new Room(this, player, entities, props, decorations, zones, collisionComponent);

            player = new Player(room, new RectangleF(200, 200, 50, 50));
            entities.Add(player);
            collisionComponent.Insert(player);

            initializeHearts(player.Hearts);

            Follower follower = new Follower(room, new CircleF(new Point2(100, 100), 25));
            entities.Add(follower);
            collisionComponent.Insert(follower);

            follower = new Follower(room, new CircleF(new Point2(400, 400), 25));
            entities.Add(follower);
            collisionComponent.Insert(follower);

            follower = new Follower(room, new CircleF(new Point2(700, 700), 25));
            entities.Add(follower);
            collisionComponent.Insert(follower);

            RectangleF bounds = new RectangleF(new Point2(0, MAP_HEIGHT / 2), new Size2(textures.FlyerTexture.Width, textures.FlyerTexture.Height));
            Flyer flyer = new Flyer(room, bounds, textures.FlyerTexture);
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
        }

        private void updateHearts()
        {
            if(player.Hearts > 0)
            {
                if (player.Hearts < hearts.Count)
                {
                    hearts.RemoveAt(hearts.Count - 1);
                    currentHeartX -= heartOffset;
                }
                else if (player.Hearts > hearts.Count)
                {
                    hearts.Add(new Heart(this, new Vector2(currentHeartX, currentHeartY), textures.HeartTexture));
                    currentHeartX += heartOffset;
                }
            }
            else
            {
                Exit();
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            room.Update(gameTime);
            updateHearts();

            if(player.Hearts <= 0)
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.TransparentBlack);

            spriteBatch.Begin();

                room.Draw(spriteBatch);

                foreach (var heart in hearts)
                {
                    heart.Draw(spriteBatch);
                }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
