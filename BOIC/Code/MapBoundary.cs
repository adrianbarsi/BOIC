using BOIC.Code.Components;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOIC.Code
{
    public class MapBoundary : IZone, ICollisionActor
    {
        public IShapeF Bounds { get; }

        public MapBoundary(IShapeF Bounds)
        {
            this.Bounds = Bounds;
        }

        public void OnCollision(CollisionEventArgs collisionInfo)
        {
        }

        public void Update(GameTime gameTime)
        {
        }
    }
}
