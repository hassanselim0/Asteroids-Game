using System;
using System.Collections.Generic;
using System.Text;

namespace AsteroidsGame
{
    class GameConstants
    {
        //camera constants.
        public const float CameraHeight = 25000.0f;
        public const float PlayfieldSizeX = 16000f;
        public const float PlayfieldSizeY = 12500f;
        //asteroid constants.
        public static int NumAsteroids = 0;
        public const float AsteroidMinSpeed = 300.0f;
        public const float AsteroidMaxSpeed = 600.0f;
        public static int AsteroidSpeedAdjustment = 6;
        // Bullet Constants.
        public const int NumBullets = 5;
        public const int MaxBulletTime = 2;
        public const float BulletSpeedAdjustment = 150.0f;
        // Scoring Constants.
        public const int ShotPenalty = 1;
        public const int WarpPenalty = 50;
        public const int KillBonus = 25;
    }
}
