using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prequel
{
    class EnemyProperties
    {
        public int XCount;
        public int YCount;

        public float YMovementRange;
        public float InitialX;
        public float InitialY;
        public float XInterval;
        public float YInterval;
        public float YMovementInterval;
        public float ShotSpeed;

        public EnemyProperties(int newXCount, int newYCount, float newYMovementRange, float newInitialX, float newInitialY, float newXInterval, float newYInterval, float newYMovementInterval, float newShotSpeed)
        {
            XCount = newXCount;
            YCount = newYCount;
            YMovementRange = newYMovementRange;
            InitialX = newInitialX;
            InitialY = newInitialY;
            XInterval = newXInterval;
            YInterval = newYInterval;
            YMovementInterval = newYMovementInterval;
            ShotSpeed = newShotSpeed;
        }
    }
}
