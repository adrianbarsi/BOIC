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

using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.TextureAtlases;

namespace BOIC.Code
{
    public class Door : IProp
    {
        private BOIC game;

        public IShapeF Bounds { get; set; }

        private Direction direction;
        private ParticleEffect _particleEffect;
        private Texture2D _particleTexture;

        public Door(BOIC game, RectangleF displayShape, Direction direction)
        {
            this.game = game;
            this.direction = direction;
            Bounds = new RectangleF(new Point2(displayShape.X - (displayShape.Width / 2), displayShape.Y - (displayShape.Height / 2)), new Size2(displayShape.Width, displayShape.Height));

            _particleTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            _particleTexture.SetData(new[] { Color.White });

            TextureRegion2D textureRegion = new TextureRegion2D(_particleTexture);
            _particleEffect = new ParticleEffect(autoTrigger: false)
            {
                Position = displayShape.Position,
                Emitters = new List<ParticleEmitter>
                {
                    new ParticleEmitter(textureRegion, 500, TimeSpan.FromSeconds(2.5),
                        Profile.BoxUniform(displayShape.Width, displayShape.Height))
                    {
                        Parameters = new ParticleReleaseParameters
                        {
                            Speed = new Range<float>(0f, 75f),
                            Quantity = 3,
                            Rotation = new Range<float>(-1f, 1f),
                            Scale = new Range<float>(3.0f, 4.0f)
                        },
                        Modifiers =
                        {
                            new RotationModifier {RotationRate = -2.1f},
                            new RectangleContainerModifier {Width = (int)displayShape.Width, Height = (int)displayShape.Height, RestitutionCoefficient = 0.1f},
                            new LinearGravityModifier {Direction = -Vector2.UnitY, Strength = 30f},
                        }
                    }
                }
            };
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_particleEffect);
            spriteBatch.DrawRectangle((RectangleF)Bounds, Color.Yellow, 1);
        }

        public void Update(GameTime gameTime)
        {
            _particleEffect.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
            if (collisionInfo.Other is Player)
            {
                game.changeRoom(direction);
            }
        }
    }
}
