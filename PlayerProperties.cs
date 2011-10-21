using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prequel
{
    class PlayerProperties
    {
        public float MovementSpeed;     //This is defined in terms of pixels per millisecond. That might change later though.
        public float ShotSpeed;
        public float XPosition;

        public PlayerProperties(float newMovementSpeed, float newShotSpeed, float newXPosition)
        {
            MovementSpeed = newMovementSpeed;
            ShotSpeed = newShotSpeed;
            XPosition = newXPosition;
        }
    }
}
