using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AsteroidsGame
{
    public class Ship
    {
        // Is the ship Alive ?
        public bool isActive = true;

        public double timer = 0;

        //Position of the model in world space
        public Vector3 Position = Vector3.Zero;

        //Velocity of the model, applied each frame to the model's position
        public Vector3 Velocity = Vector3.Zero;

        public Matrix RotationMatrix = Matrix.CreateRotationX(MathHelper.PiOver2);
        public float rotation = 0.0f;
        public float RotationAdd;
        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                float newVal = value;
                while (newVal >= MathHelper.TwoPi)
                {
                    newVal -= MathHelper.TwoPi;
                }
                while (newVal < 0)
                {
                    newVal += MathHelper.TwoPi;
                }

                rotation = newVal;
                RotationMatrix = Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateRotationZ(rotation);
            }
        }

        public void Update()
        {
            if (!WinningScreen.isActive && isActive)
            {
                // Add velocity to the current position.
                Position += Velocity;

                // Add RotaionAdd to Rotation.
                Rotation -= RotationAdd;

                // Bleed off velocity over time.
                Velocity *= 0.98f;

                // Limit Rotation Speed.
                RotationAdd *= 0.9f;
            }
            else
            {
                if (!isActive)
                {
                    Position = Vector3.Zero;
                    Rotation = 0;
                }
                Velocity = Vector3.Zero;
                RotationAdd = 0;
            }
        }
    }
}