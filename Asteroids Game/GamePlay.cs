using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AsteroidsGame
{
    public class GamePlay
    {
        public static bool isActive = false;
        public static bool gameOver = false;
        public static double time = 0;
        public static int level = 1;

        public static Player[] playerList;
        public static Asteroid[] asteroidList;

        //Visual components
        public static Model asteroidModel;
        public static Model bulletModel;
        public static Matrix[] asteroidTransforms;
        public static Matrix[] bulletTransforms;
        public static SpriteFont scoreFont;
        public static SpriteFont winFont;

        public static Random random = new Random();

        public static int localPlayers
        {
            get
            {
                int n = 0;
                for (int i = 0; i < 6; i++)
                    if (Settings.enabled[i])
                        n++;
                return n;
            }
        }

        public static void StartGame()
        {
            playerList = new Player[localPlayers];
            int tmp = 0;
            for (int i = 0; tmp < playerList.Length; i++)
                if (Settings.enabled[i])
                    playerList[tmp++] = new Player(tmp - 1, Settings.names[i], MainClass.ships[Settings.ships[i] - 1],
                        MainClass.shipTransforms[Settings.ships[i] - 1], Settings.colors[i], Controls.inputs[i]);
            Reset();
            TitleScreen.gameStarted = true;
        }

        public static void Update(GameTime gameTime)
        {
            // Plays the next Music track :)
            if (Input.isPressed(Keys.OemTilde))
                Music.playNextTrack();

            // Update Players
            for (int i = 0; i < playerList.Length; i++)
                playerList[i].Update(gameTime);

            // Update the Asteroid Position.
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
                asteroidList[i].Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            // Display Title Screen.
            if (!WinningScreen.isActive && Controls.getInput(Action.Pause))
            {
                TitleScreen.isActive = true;
                MainClass.soundBank.PlayCue("Selection_change");
                isActive = false;
            }

            // Winning Screen Input.
            if (WinningScreen.isActive && Controls.getInput(Action.Select))
            {
                if (gameOver)
                {
                    HighScores.isActive = true;
                    HighScores.newHighScoreOrder = new int[playerList.Length];
                    for (int i = 0; i < HighScores.newHighScoreOrder.Length; i++)
                        HighScores.newHighScoreOrder[i] = 10;
                }
                else
                    TitleScreen.isActive = true;
                MainClass.soundBank.PlayCue("Selection_change");
            }


            // Did you Finish all asteroids ?
            WinningScreen.isActive = true;
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                if (asteroidList[i].isActive)
                {
                    WinningScreen.isActive = false;
                    break;
                }
            }

            // Check for Game Over
            gameOver = true;
            for (int i = 0; i < playerList.Length; i++)
                if (playerList[i].life != 0)
                    gameOver = false;
            if (gameOver)
                for (int i = 0; i < asteroidList.Length; i++)
                    asteroidList[i].isActive = false;

            // Updates time.
            if (!WinningScreen.isActive)
            {
                time += gameTime.ElapsedGameTime.TotalSeconds;
            }
            for (int j = 0; j < playerList.Length; j++)
            {
                for (int i = 0; i < GameConstants.NumBullets; i++)
                    if (playerList[j].bulletList[i].startTimer)
                        playerList[j].bulletList[i].timer += gameTime.ElapsedGameTime.TotalSeconds;
                playerList[j].ship.timer += gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public static void Reset()
        {
            WinningScreen.isActive = false;
            time = 0;
            float deltaRotation = MathHelper.TwoPi / playerList.Length;

            // Reset the Players.
            for (int i = 0; i < playerList.Length; i++)
            {
                playerList[i].ship.Rotation = i * deltaRotation;
                if (playerList.Length == 1)
                    playerList[i].ship.Position = Vector3.Zero;
                else
                    playerList[i].ship.Position = 2000 * playerList[i].ship.RotationMatrix.Forward;
                playerList[i].ship.Velocity = Vector3.Zero;
                playerList[i].ship.timer = 0;
                playerList[i].bomb.isActive = false;
                if (playerList[i].life > 0)
                    playerList[i].ship.isActive = true;
                else
                    playerList[i].ship.isActive = false;
            }

            // Reset the Asteroids.
            if (level % 2 == 1)
                GameConstants.NumAsteroids += 5;
            else
                GameConstants.AsteroidSpeedAdjustment += 1;

            asteroidList = new Asteroid[GameConstants.NumAsteroids];

            float xStart;
            float yStart;
            float angle;
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                if (random.Next(2) == 0)
                {
                    xStart = -GameConstants.PlayfieldSizeX;
                    angle = (float)(random.NextDouble() * 90) + 45;
                }
                else
                {
                    xStart = GameConstants.PlayfieldSizeX;
                    angle = (float)(random.NextDouble() * 90) - 135;
                }

                yStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeY - 6250;
                asteroidList[i].position = new Vector3(xStart, yStart, 0.0f);
                asteroidList[i].direction.X = -(float)Math.Sin(MathHelper.ToRadians(angle));
                asteroidList[i].direction.Y = (float)Math.Cos(MathHelper.ToRadians(angle));
                asteroidList[i].speed = GameConstants.AsteroidMinSpeed +
                    (float)random.NextDouble() * GameConstants.AsteroidMaxSpeed;
                asteroidList[i].rotation = (float)random.NextDouble() + 0.1f;
                asteroidList[i].isActive = true;
            }
        }

        public static void Draw(GameTime gameTime)
        {
            // Draws the asteroids.
            MainClass.graphics.GraphicsDevice.BlendState = new BlendState()
            {
                AlphaSourceBlend = Blend.One,
                AlphaDestinationBlend = Blend.Zero
            };
            for (int i = 0; i < GameConstants.NumAsteroids; i++)
            {
                Matrix asteroidRotaion = Matrix.CreateRotationY(asteroidList[i].rotation
                        * (float)(gameTime.TotalGameTime.TotalSeconds + 10))
                        * Matrix.CreateRotationZ(asteroidList[i].rotation
                        * (float)(gameTime.TotalGameTime.TotalSeconds + 10));
                Matrix asteroidTransform = asteroidRotaion * Matrix.CreateTranslation(asteroidList[i].position);
                if (asteroidList[i].isActive)
                    DrawModel(asteroidModel, asteroidTransform, asteroidTransforms);
            }
            MainClass.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            for (int i = 0; i < playerList.Length; i++)
                playerList[i].Draw(gameTime);

            // Draws the Level & Time
            if (!WinningScreen.isActive)
            {
                MainClass.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                MainClass.spriteBatch.DrawString(scoreFont, "Level: " + level + "\nTime: " + (time - time % 0.01),
                    new Vector2(450, 40), Color.LightGreen);
                MainClass.spriteBatch.End();
            }

            // Draws the Players info.
            if (!WinningScreen.isActive)
            {
                MainClass.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                for (int i = 0; i < playerList.Length; i++)
                    MainClass.spriteBatch.DrawString(GamePlay.scoreFont, "Player: " + (i + 1)
                        + "\nLives: " + (playerList[i].life == 0 ? "RIP" : !playerList[i].ship.isActive ?
                        "Press " + playerList[i].controls[5] : playerList[i].life.ToString())
                        + "\nScore: " + playerList[i].score
                        + "\nSpecial Wep: " + (playerList[i].bomb.isAvailable ? "bomb" : "none")
                        + "\n------------------------",
                        new Vector2(20, 20 + i * 120), Color.LightGreen);
                MainClass.spriteBatch.End();
            }

            // Draws the Winning Screen.
            if (WinningScreen.isActive)
            {
                for (int i = 0; i < playerList.Length; i++)
                    if (playerList[i].win)
                        WinningScreen.Draw(gameTime, i);
            }
        }

        public static void DrawModel(Model model, Matrix modelTransform, Matrix[] absoluteBoneTransforms)
        {
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in model.Meshes)
            {
                //This is where the mesh orientation is set
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = absoluteBoneTransforms[mesh.ParentBone.Index] * modelTransform;
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }
    }
}
