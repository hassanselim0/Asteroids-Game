using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AsteroidsGame
{
    public class Intro
    {
        public static Texture2D XavierProductions;
        public static Texture2D logo2;
        static Color fade1 = new Color(0, 0.8f, 0.2f, 0);
        static Color fade2 = new Color(255, 255, 255, 0);
        static Color fade3 = new Color(255, 255, 255, 0);
        
        public static Asteroid[] logoAsteroids = new Asteroid[20];

        public static void Update(GameTime gameTime)
        {
            if (Input.isAnyKeyPressed() || Controls.getInput(Action.Select) || Controls.getInput(Action.Back))
            {
                TitleScreen.isActive = true;
            }

            if (gameTime.TotalGameTime.TotalSeconds > 14)
                TitleScreen.isActive = true;

            if (gameTime.TotalGameTime.TotalSeconds > 9.15f && gameTime.TotalGameTime.TotalSeconds < 14)
                for (int i = 0; i < 20; i++)
                    logoAsteroids[i].position += logoAsteroids[i].direction * logoAsteroids[i].speed
                        * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public static void ResetLogoAsteroids()
        {
            Random random = new Random();
            float angle;
            for (int i = 0; i < 20; i++)
            {
                angle = (float)(random.NextDouble() * 360);
                logoAsteroids[i].direction.X = -(float)Math.Sin(MathHelper.ToRadians(angle));
                logoAsteroids[i].direction.Y = (float)Math.Cos(MathHelper.ToRadians(angle));
                logoAsteroids[i].position = logoAsteroids[i].direction * 3000;
                logoAsteroids[i].speed = 2600 + (float)random.NextDouble() * 3000;
                logoAsteroids[i].rotation = (float)random.NextDouble() + 0.1f;
            }
        }

        public static void Draw(GameTime gameTime)
        {
            MainClass.spriteBatch.Begin(SpriteSortMode.Immediate,
                new BlendState() { ColorSourceBlend = Blend.SourceAlpha, ColorDestinationBlend = Blend.InverseSourceAlpha });

            if (gameTime.TotalGameTime.TotalSeconds > 0 && gameTime.TotalGameTime.TotalSeconds < 4)
            {
                if (gameTime.TotalGameTime.TotalSeconds < 2 && fade1.A < 252)
                    fade1.A += 4;
                if (gameTime.TotalGameTime.TotalSeconds > 2.8f && fade1.A > 0)
                    fade1.A -= 4;

                MainClass.spriteBatch.DrawString(GamePlay.scoreFont, "                     This game was made by:\n" +
                                                                    "                 Hassan Aly Mahmoud Selim\n" +
                                                                    "                        a GUC DMET student",
                                                                    new Vector2(300, 300), fade1);
            }

            if (gameTime.TotalGameTime.TotalSeconds > 4 && gameTime.TotalGameTime.TotalSeconds < 8)
            {
                if (gameTime.TotalGameTime.TotalSeconds < 6 && fade2.A < 252)
                    fade2.A += 4;
                if (gameTime.TotalGameTime.TotalSeconds > 6.8f && fade2.A > 0)
                    fade2.A -= 4;

                MainClass.spriteBatch.Draw(XavierProductions, new Vector2(262, 294), fade2);
            }

            if (gameTime.TotalGameTime.TotalSeconds > 9.1f && gameTime.TotalGameTime.TotalSeconds < 14)
            {
                if (gameTime.TotalGameTime.TotalSeconds < 10 && fade3.A < 252)
                    fade3.A += 3;
                if (gameTime.TotalGameTime.TotalSeconds > 11.8f && fade3.A > 0)
                    fade3.A -= 3;

                MainClass.spriteBatch.Draw(logo2, new Vector2(312, 290), fade3);
            }
            
            MainClass.spriteBatch.End();

            MainClass.graphics.GraphicsDevice.BlendState = new BlendState()
            {
                AlphaSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.Zero,
            };
            if (gameTime.TotalGameTime.TotalSeconds > 9.1f && gameTime.TotalGameTime.TotalSeconds < 14)
                for (int i = 0; i < 20; i++)
                {
                    Matrix asteroidRotaion = Matrix.CreateRotationY(logoAsteroids[i].rotation
                            * (float)(gameTime.TotalGameTime.TotalSeconds + 10))
                            * Matrix.CreateRotationZ(logoAsteroids[i].rotation
                            * (float)(gameTime.TotalGameTime.TotalSeconds + 10));
                    Matrix asteroidTransform = asteroidRotaion * Matrix.CreateTranslation(logoAsteroids[i].position);
                    GamePlay.DrawModel(GamePlay.asteroidModel, asteroidTransform, GamePlay.asteroidTransforms);
                }
            MainClass.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }
    }
}
