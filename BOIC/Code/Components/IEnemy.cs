using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOIC.Code.Components
{

    public interface IEnemy
    {
        public int Hp { get; set; }
        public int CollisionDamage { get; set; }
        public float PotionDropProbability { get; set; }
    }
}
