using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace AsteroidsGame
{
    public struct Asteroid
    {
        public bool isActive;

        public Vector3 position;
        public Vector3 direction;
        public float speed;
        public float rotation;

        public void Update(float delta)
        {
            position += direction * speed * GameConstants.AsteroidSpeedAdjustment * delta;
            if (position.X > GameConstants.PlayfieldSizeX)
                position.X -= 2 * GameConstants.PlayfieldSizeX;
            if (position.X < -GameConstants.PlayfieldSizeX)
                position.X += 2 * GameConstants.PlayfieldSizeX;
            if (position.Y > GameConstants.PlayfieldSizeY)
                position.Y -= 2 * GameConstants.PlayfieldSizeY;
            if (position.Y < -GameConstants.PlayfieldSizeY)
                position.Y += 2 * GameConstants.PlayfieldSizeY;
        }
    }
}
