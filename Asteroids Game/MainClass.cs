using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

namespace AsteroidsGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainClass : Microsoft.Xna.Framework.Game
    {
        bool doneSaving = false;

        //Camera and View information
        public static GraphicsDeviceManager graphics;
        public static float aspectRatio;
        Vector3 cameraPosition = new Vector3(0.0f, 0.0f, GameConstants.CameraHeight);
        public static Matrix projectionMatrix;
        static Matrix viewMatrix;

        //Audio components
        public static SoundBank soundBank;
        AudioEngine audioEngine;
        WaveBank waveBank;

        //Visual components
        public static SpriteBatch spriteBatch;
        public static Texture2D logo;
        Texture2D stars;

        //The ships.
        public static Model[] ships = new Model[3];
        public static Matrix[][] shipTransforms = new Matrix[3][];
        
        public MainClass()
        {
            Content.RootDirectory = "Content";
            
            Components.Add(new GamerServicesComponent(this));
            graphics = new GraphicsDeviceManager(this);
            Exiting += new EventHandler<EventArgs>(SaveState);
            
            HighScores.LoadHighScores();
            Controls.LoadControls();
            Settings.LoadSettings();
            Intro.ResetLogoAsteroids();
            graphics.PreferMultiSampling = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>

        protected override void Initialize()
        {
            audioEngine = new AudioEngine("Content\\Audio\\MyGameAudio.xgs");
            waveBank = new WaveBank(audioEngine, "Content\\Audio\\Wave Bank.xwb");
            soundBank = new SoundBank(audioEngine, "Content\\Audio\\Sound Bank.xsb");

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio,
                GameConstants.CameraHeight - 2000.0f, GameConstants.CameraHeight + 2000.0f);
            viewMatrix = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>

        protected override void LoadContent()
        {
            GamePlay.bulletModel = Content.Load<Model>("Models/pea_proj");
            GamePlay.bulletTransforms = SetupEffectDefaults(GamePlay.bulletModel);

            Bomb.explosionModel = Content.Load<Model>("Models/bomb explosion");
            Bomb.explosionTransforms = SetupEffectDefaults(Bomb.explosionModel);

            PowerUp.bombModel = Content.Load<Model>("Models/bomb");
            PowerUp.bombTransforms = SetupEffectDefaults(PowerUp.bombModel);

            GamePlay.asteroidModel = Content.Load<Model>("Models/asteroid1");
            GamePlay.asteroidTransforms = SetupEffectDefaults(GamePlay.asteroidModel);
            for (int i = 0; i < ships.Length; i++)
            {
                ships[i] = Content.Load<Model>("Models/ship" + (i + 1));
                shipTransforms[i] = SetupEffectDefaults(ships[i]);
            }

            stars = Content.Load<Texture2D>("Images/B1_stars");
            logo = Content.Load<Texture2D>("Images/Logo");
            Intro.XavierProductions = Content.Load<Texture2D>("Images/HassanSelim");
            Intro.logo2 = Content.Load<Texture2D>("Images/Logo2");
            TitleScreen.selector = Content.Load<Texture2D>("Images/selection");
            Controls.selector = Content.Load<Texture2D>("Images/selection2");
            TitleScreen.ExitWarningY = Content.Load<Texture2D>("Images/ExitWarningY");
            TitleScreen.ExitWarningN = Content.Load<Texture2D>("Images/ExitWarningN");
            Settings.PlayerSettings = Content.Load<Texture2D>("Images/PlayerSettings");
            Credits.me = Content.Load<Texture2D>("Images/me");
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            GamePlay.scoreFont = Content.Load<SpriteFont>("Fonts/Copperplate Gothic Bold");
            GamePlay.winFont = Content.Load<SpriteFont>("Fonts/Freestyle Script");
            TitleScreen.optionsFont = Content.Load<SpriteFont>("Fonts/Boopee");
            TitleScreen.optionsFont.LineSpacing = 59;
            Credits.creditsFont = Content.Load<SpriteFont>("Fonts/Credits");

            Music.loadMusic(Content);
        }

        public static Matrix[] SetupEffectDefaults(Model myModel)
        {
            Matrix[] absoluteTransforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(absoluteTransforms);

            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = projectionMatrix;
                    effect.View = viewMatrix;
                }
            }
            return absoluteTransforms;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (TitleScreen.exit)
                this.Exit();

            // Update audioEngine.
            audioEngine.Update();
            if ((!GamePlay.isActive || WinningScreen.isActive) && GamePlay.playerList != null)
                for (int i = 0; i < GamePlay.playerList.Length; i++)
                    if (GamePlay.playerList[i].engineSound != null && GamePlay.playerList[i].engineSound.IsPlaying)
                        GamePlay.playerList[i].engineSound.Pause();

            Music.Update();

            if (LAN.session != null)
                LAN.session.Update();

            if (!Guide.IsVisible)
                Input.updateStates();

            //Switches to Full Screen.
            if (Input.isPressed(Keys.Tab))
                graphics.ToggleFullScreen();

            if (HighScores.isActive)
                HighScores.Update(gameTime);

            else if (TitleScreen.isActive)
                TitleScreen.Update(Content);

            else if (LAN.isActive)
                LAN.Update();

            else if (WaitingRoom.isActive)
                WaitingRoom.Update();

            else if (Controls.isActive)
                Controls.Update();

            else if (Settings.isActive)
                Settings.Update();

            else if (Credits.isActive)
                Credits.Update();

            else if (GamePlay.isActive)
                GamePlay.Update(gameTime);

            else if (gameTime.TotalGameTime.TotalSeconds < 14.1)
                Intro.Update(gameTime);

            Input.outdateStates();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Draws the Background and Logo and Version.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(stars, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width,
                graphics.GraphicsDevice.Viewport.Height), Color.White);
            if (HighScores.isActive || TitleScreen.isActive || Controls.isActive || Settings.isActive || LAN.isActive
                || WaitingRoom.isActive)
            {
                spriteBatch.Draw(logo, new Vector2(300, 40), Color.White);
                spriteBatch.DrawString(GamePlay.scoreFont, "v 1.3", new Vector2(730, 100), Color.Red, 0,
                    new Vector2(0), 1, SpriteEffects.None, 0);
            }
            spriteBatch.End();

            if (HighScores.isActive)
                HighScores.Draw(gameTime);

            else if (TitleScreen.isActive)
                TitleScreen.Draw();

            else if (LAN.isActive)
                LAN.Draw();

            else if (WaitingRoom.isActive)
                WaitingRoom.Draw();

            else if (Controls.isActive)
                Controls.Draw();

            else if (Settings.isActive)
                Settings.Draw(gameTime);

            else if (Credits.isActive)
                Credits.Draw();

            else if (GamePlay.isActive)
                GamePlay.Draw(gameTime);

            else if (gameTime.TotalGameTime.TotalSeconds < 14.2)
                Intro.Draw(gameTime);

            //Draws "Press "Tab" to Toggle Full Screen".
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.DrawString(GamePlay.scoreFont, "Press \"Tab\" to Toggle Full Screen", new Vector2(325, 20),
                Color.Red);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void SaveState(object sender, EventArgs e)
        {
            if (!doneSaving)
            {
                HighScores.SaveHighScores();
                Controls.SaveControls();
                Settings.SaveSettings();
                doneSaving = true;
            }
        }
    }
}
