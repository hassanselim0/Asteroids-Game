using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace AsteroidsGame
{
    public struct Bullet
    {
        public bool isActive;

        public Vector3 position;
        public Vector3 direction;
        public float speed;
        public Vector3 velocityAdd;
        public double timer;
        public bool startTimer;

        public void Update(float delta)
        {
            position += direction * speed * GameConstants.BulletSpeedAdjustment * delta + velocityAdd;
            if (timer > GameConstants.MaxBulletTime)
            {
                isActive = false;
                startTimer = false;
                timer = 0;
            }
        }
    }
}
