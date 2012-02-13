using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsteroidsGame
{
    public class PowerUp
    {
        public bool isActive = false;
        public Matrix transformationMatrix;
        public Vector3 position;
        public float rotationX;
        public float rotationZ;
        public static Model bombModel;
        public static Matrix[] bombTransforms;

        public PowerUp()
        {
        }

        public void Initialize(Vector3 pos)
        {
            position = pos;
            rotationX = 0;
            rotationZ = 0;
            isActive = true;
        }

        public void Update(GameTime gameTime, Vector3 shipPosition)
        {
            rotationX += 0.15f;
            rotationZ += 0.01f;
            transformationMatrix = Matrix.CreateRotationX(rotationX) * Matrix.CreateRotationZ(rotationZ) * Matrix.CreateTranslation(position);
            if ((position - shipPosition).Length() > 2200)
                position -= (position - shipPosition) / 100;

            BoundingSphere bombSphere = new BoundingSphere(position,
                bombModel.Meshes[0].BoundingSphere.Radius);
            for (int i = 0; i < GamePlay.playerList.Length; i++)
                if (GamePlay.playerList[i].ship.isActive)
                {
                    BoundingSphere shipSphere = new BoundingSphere(GamePlay.playerList[i].ship.Position,
                        GamePlay.playerList[i].shipModel.Meshes[0].BoundingSphere.Radius * 0.8f);
                    if (shipSphere.Intersects(bombSphere))
                    {
                        MainClass.soundBank.PlayCue("powerUp");
                        isActive = false;
                        GamePlay.playerList[i].bomb.isAvailable = true;
                        return; //exit the loop
                    }
                }
        }

        public void Draw(Vector3 color)
        {
            ((BasicEffect)PowerUp.bombModel.Meshes[0].Effects[0]).DiffuseColor = color;
            ((BasicEffect)PowerUp.bombModel.Meshes[1].Effects[0]).DiffuseColor = color;
            GamePlay.DrawModel(bombModel, transformationMatrix, bombTransforms);
        }
    }
}
