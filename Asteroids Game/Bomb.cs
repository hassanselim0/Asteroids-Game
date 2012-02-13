using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsteroidsGame
{
    public class Bomb
    {
        public bool isActive = false;
        public bool isAvailable = false;
        public int index;
        public static Model explosionModel;
        public static Matrix[] explosionTransforms;
        public Vector3 position;
        public float scale;
        public float alpha;
        public double timer;

        public Bomb()
        {
        }

        public void Deploy(Vector3 pos, int i)
        {
            isActive = true;
            isAvailable = false;
            index = i;
            position = pos;
            scale = 0.5f;
            alpha = 1;
            timer = 1;
        }

        public void Update(GameTime gameTime)
        {
            if (timer > 0)
            {
                timer -= gameTime.ElapsedGameTime.TotalSeconds;
                scale += 0.12f;
                alpha -= 0.016f;
            }
            else
                isActive = false;

            BoundingSphere bombSphere = new BoundingSphere(position, explosionModel.Meshes[0].BoundingSphere.Radius * scale);
            for (int i = 0; i < GamePlay.asteroidList.Length; i++)
            {
                if (GamePlay.asteroidList[i].isActive)
                {
                    BoundingSphere asteroidSphere = new BoundingSphere(GamePlay.asteroidList[i].position,
                        GamePlay.asteroidModel.Meshes[0].BoundingSphere.Radius * 0.95f);
                    if (asteroidSphere.Intersects(bombSphere))
                    {
                        //destroy asteroid
                        MainClass.soundBank.PlayCue("explosion2");
                        GamePlay.asteroidList[i].isActive = false;
                        GamePlay.playerList[index].score += GameConstants.KillBonus;
                        return; //exit the loop
                    }
                }
            }
        }

        public void Draw()
        {
            MainClass.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            ((BasicEffect)explosionModel.Meshes[0].Effects[0]).Alpha = alpha;
            GamePlay.DrawModel(explosionModel, Matrix.CreateRotationX(MathHelper.PiOver2)
                * Matrix.CreateScale(scale) * Matrix.CreateTranslation(position), explosionTransforms);

            MainClass.graphics.GraphicsDevice.BlendState = BlendState.Opaque;
        }
    }
}
