using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AsteroidsGame
{
    class Settings
    {
        public static bool isActive = false;
        public static Texture2D PlayerSettings;
        public static bool[] enabled = new bool[6];
        public static string[] names = new string[6];
        public static int[] ships = new int[6];
        public static Vector3[] colors = new Vector3[6];
        static Model ship;
        static Matrix[] shipTrans;
        static Screen screen = Screen.main;
        static int selection = 0;
        static int playerSelection = 0;
        static int settingSelection = 0;
        static int colorSelection = 0;
        static bool typingName = false;

        public static void Update()
        {
            if (typingName)
            {
                if (names[playerSelection].Length < 16)
                    names[playerSelection] += Input.getText();
                if (Input.isPressed(Keys.Back) && names[playerSelection].Length != 0)
                    names[playerSelection] = names[playerSelection].Remove(names[playerSelection].Length - 1);
                if (Input.isPressed(Keys.Enter))
                {
                    MainClass.soundBank.PlayCue("Selection_choose");
                    typingName = false;
                }
                return;
            }

            if (Controls.getInput(Action.MenuUp))
            {
                MainClass.soundBank.PlayCue("Selection_change");
                switch (screen)
                {
                    case Screen.colorSetting:
                        if (colorSelection == 0)
                            colorSelection = 2;
                        else
                            colorSelection--;
                        break;
                    case Screen.playerSetting:
                        if (settingSelection == 0)
                            settingSelection = 4;
                        else
                            settingSelection--;
                        break;
                    case Screen.playerSelecting:
                        if (playerSelection == 0)
                            playerSelection = 6;
                        else
                            playerSelection--;
                        break;
                    case Screen.main:
                        if (selection == 0)
                            selection = 3;
                        else
                            selection--;
                        break;
                }
            }

            if (Controls.getInput(Action.MenuDown))
            {
                MainClass.soundBank.PlayCue("Selection_change");
                switch (screen)
                {
                    case Screen.colorSetting:
                        if (colorSelection == 2)
                            colorSelection = 0;
                        else
                            colorSelection++;
                        break;
                    case Screen.playerSetting:
                        if (settingSelection == 4)
                            settingSelection = 0;
                        else
                            settingSelection++;
                        break;
                    case Screen.playerSelecting:
                        if (playerSelection == 6)
                            playerSelection = 0;
                        else
                            playerSelection++;
                        break;
                    case Screen.main:
                        if (selection == 3)
                            selection = 0;
                        else
                            selection++;
                        break;
                }
            }

            if (Controls.getInput(Action.MenuRight))
            {
                MainClass.soundBank.PlayCue("Selection_change");
                switch (screen)
                {
                    case Screen.colorSetting:
                        if (colorSelection == 0 && colors[playerSelection].X != 1)
                            colors[playerSelection].X += 0.1f;
                        if (colorSelection == 1 && colors[playerSelection].Y != 1)
                            colors[playerSelection].Y += 0.1f;
                        if (colorSelection == 2 && colors[playerSelection].Z != 1)
                            colors[playerSelection].Z += 0.1f;
                        break;
                    case Screen.playerSetting:
                        if (settingSelection == 2)
                            if (ships[playerSelection] == MainClass.ships.Length)
                                ships[playerSelection] = 1;
                            else
                                ships[playerSelection]++;
                        break;
                }
            }

            if (Controls.getInput(Action.MenuLeft))
            {
                MainClass.soundBank.PlayCue("Selection_change");
                switch (screen)
                {
                    case Screen.colorSetting:
                        if (colorSelection == 0 && colors[playerSelection].X != 0)
                            colors[playerSelection].X -= 0.1f;
                        if (colorSelection == 1 && colors[playerSelection].Y != 0)
                            colors[playerSelection].Y -= 0.1f;
                        if (colorSelection == 2 && colors[playerSelection].Z != 0)
                            colors[playerSelection].Z -= 0.1f;
                        break;
                    case Screen.playerSetting:
                        if (settingSelection == 2)
                            if (ships[playerSelection] == 1)
                                ships[playerSelection] = MainClass.ships.Length;
                            else
                                ships[playerSelection]--;
                        break;
                }
            }

            if (Controls.getInput(Action.Select))
            {
                MainClass.soundBank.PlayCue("Selection_choose");
                switch (screen)
                {
                    case Screen.colorSetting:
                        screen = Screen.playerSetting;
                        colorSelection = 0;
                        break;
                    
                    case Screen.playerSetting:
                        switch (settingSelection)
                        {
                            case 0:
                                enabled[playerSelection] = !enabled[playerSelection];
                                break;
                            case 1:
                                typingName = true;
                                break;
                            case 3:
                                screen = Screen.colorSetting;
                                break;
                            case 4:
                                screen = Screen.playerSelecting;
                                settingSelection = 0;
                                break;
                        }
                        break;
                    
                    case Screen.playerSelecting:
                        if (playerSelection == 6)
                        {
                            screen = Screen.main;
                            playerSelection = 0;
                        }
                        else
                            screen = Screen.playerSetting;
                        break;
                    
                    case Screen.main:
                        switch (selection)
                        {
                            case 0:
                                MainClass.graphics.ToggleFullScreen();
                                break;
                            case 1:
                                if (MainClass.aspectRatio == 4f / 3f)
                                {
                                    MainClass.aspectRatio = 16f / 9f;
                                    MainClass.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                                        MathHelper.ToRadians(45.0f), MainClass.aspectRatio,
                                        GameConstants.CameraHeight - 2000.0f, GameConstants.CameraHeight + 2000.0f);
                                    MainClass.graphics.PreferredBackBufferWidth = 1280;
                                    MainClass.graphics.PreferredBackBufferHeight = 800;
                                }
                                else
                                {
                                    MainClass.aspectRatio = 4f / 3f;
                                    MainClass.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                                        MathHelper.ToRadians(45.0f), MainClass.aspectRatio,
                                        GameConstants.CameraHeight - 2000.0f, GameConstants.CameraHeight + 2000.0f);
                                    MainClass.graphics.PreferredBackBufferWidth = 1024;
                                    MainClass.graphics.PreferredBackBufferHeight = 720;
                                }
                                break;
                            case 2:
                                screen = Screen.playerSelecting;
                                break;
                            case 3:
                                TitleScreen.isActive = true;
                                isActive = false;
                                selection = 0;
                                break;
                        }
                        break;
                }
            }

            if (Controls.getInput(Action.Back))
            {
                MainClass.soundBank.PlayCue("Selection_choose");
                switch (screen)
                {
                    case Screen.colorSetting:
                        screen = Screen.playerSetting;
                        colorSelection = 0;
                        break;
                    case Screen.playerSetting:
                        screen = Screen.playerSelecting;
                        settingSelection = 0;
                        break;
                    case Screen.playerSelecting:
                        screen = Screen.main;
                        playerSelection = 0;
                        break;
                    case Screen.main:
                        TitleScreen.isActive = true;
                        isActive = false;
                        selection = 0;
                        break;
                }
            }
        }

        public static void SaveSettings()
        {
            string[] data = new string[38];

            data[0] = MainClass.graphics.IsFullScreen.ToString();
            data[1] = MainClass.graphics.PreferredBackBufferWidth.ToString();
            for (int i = 0; i < 6; i++)
            {
                data[2 + i * 6] = enabled[i].ToString();
                data[3 + i * 6] = names[i];
                data[4 + i * 6] = ships[i].ToString();
                data[5 + i * 6] = colors[i].X.ToString();
                data[6 + i * 6] = colors[i].Y.ToString();
                data[7 + i * 6] = colors[i].Z.ToString();
            }

            File.WriteAllLines("Settings.data", data);
        }

        public static void LoadSettings()
        {
            try
            {
                string[] data = File.ReadAllLines("Settings.data");

                if (data.Length != 38) throw new FileNotFoundException();

                MainClass.graphics.IsFullScreen = bool.Parse(data[0]);
                if (data[1] == "1280")
                {
                    MainClass.aspectRatio = 16f / 9f;
                    MainClass.graphics.PreferredBackBufferWidth = 1280;
                    MainClass.graphics.PreferredBackBufferHeight = 800;
                }
                else
                {
                    MainClass.aspectRatio = 4f / 3f;
                    MainClass.graphics.PreferredBackBufferWidth = 1024;
                    MainClass.graphics.PreferredBackBufferHeight = 720;
                }
                for (int i = 0; i < 6; i++)
                {
                    enabled[i] = bool.Parse(data[2 + i * 6]);
                    names[i] = data[3 + i * 6];
                    ships[i] = int.Parse(data[4 + i * 6]);
                    colors[i].X = float.Parse(data[5 + i * 6]);
                    colors[i].Y = float.Parse(data[6 + i * 6]);
                    colors[i].Z = float.Parse(data[7 + i * 6]);
                }
            }
            catch (FileNotFoundException)
            {
                ResetSettings();
            }

        }

        public static void ResetSettings()
        {
            MainClass.graphics.IsFullScreen = false;
            MainClass.aspectRatio = 4f / 3f;
            MainClass.graphics.PreferredBackBufferWidth = 1024;
            MainClass.graphics.PreferredBackBufferHeight = 720;
            enabled[0] = true;
            for (int i = 0; i < 6; i++)
            {
                enabled[i] = false;
                names[i] = "Player " + (i + 1);
                ships[i] = 1;
            }
            enabled[0] = true;
            colors[0] = Color.Green.ToVector3();
            colors[1] = Color.Orange.ToVector3();
            colors[2] = Color.Blue.ToVector3();
            colors[3] = Color.Chocolate.ToVector3();
            colors[4] = Color.Purple.ToVector3();
            colors[5] = Color.Yellow.ToVector3();
        }

        public static void Draw(GameTime gameTime)
        {
            MainClass.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Full Screen: "
                + (MainClass.graphics.IsFullScreen ? "On" : "Off"), new Vector2(50, 200), Color.Green);
            MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Resolution: " +
                MainClass.graphics.PreferredBackBufferWidth + "x" + MainClass.graphics.PreferredBackBufferHeight,
                new Vector2(50, 300), Color.Green);
            MainClass.spriteBatch.DrawString(TitleScreen.optionsFont,
                "Player Settings", new Vector2(50, 400), Color.Green);
            MainClass.spriteBatch.DrawString(TitleScreen.optionsFont,
                "Back", new Vector2(50, 500), Color.Green);

            if (screen == Screen.main || screen == Screen.playerSelecting)
                MainClass.spriteBatch.Draw(TitleScreen.selector, new Rectangle(25, 175 + 100 * selection, 400, 100),
                    Color.White);

            if (screen == Screen.playerSelecting)
            {
                for (int i = 0; i < 6; i++)
                    MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Player " + (i + 1),
                        new Vector2(500, 200 + 50 * i), Color.Green);
                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Back", new Vector2(500, 500), Color.Green);
                MainClass.spriteBatch.Draw(TitleScreen.selector, new Rectangle(475, 205 + 50 * playerSelection, 175,
                    55), Color.White);
            }

            if (screen == Screen.playerSetting || screen == Screen.colorSetting)
            {
                for (int i = 0; i < 6; i++)
                    colors[i] = new Vector3((float)Math.Round(colors[i].X, 1), (float)Math.Round(colors[i].Y, 1),
                        (float)Math.Round(colors[i].Z, 1));

                MainClass.spriteBatch.Draw(PlayerSettings, new Vector2(MainClass.graphics.PreferredBackBufferWidth / 2f,
                    MainClass.graphics.PreferredBackBufferHeight / 2f), null, Color.White, 0, new Vector2(400, 300),
                    1, SpriteEffects.None, 0);

                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, enabled[playerSelection].ToString(),
                    new Vector2(370, 200), Color.Orange);
                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, names[playerSelection].ToString(),
                    new Vector2(325, 260), Color.Orange);
                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Ship " + ships[playerSelection],
                    new Vector2(334, 330), Color.Orange);

                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, colors[playerSelection].X * 10 + "",
                    new Vector2(430, 400), Color.Red);
                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, colors[playerSelection].Y * 10 + "",
                    new Vector2(430, 437), new Color(0, 255, 0));
                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, colors[playerSelection].Z * 10 + "",
                    new Vector2(430, 474), Color.Blue);

                if (settingSelection != 4)
                {
                    string s = "";
                    if (settingSelection == 0) s = "_______";
                    else if (settingSelection == 1) s = "_____";
                    else if (settingSelection == 2) s = "____  <        >";
                    else if (settingSelection == 3) s = "_____";
                    MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, s,
                        new Vector2(160, 208 + 64 * settingSelection), Color.Orange);
                }
                else
                    MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "______",
                        new Vector2(450, 580), Color.Orange);
            }

            if (screen == Screen.colorSetting)
                for (int i = 0; i < 4; i++)
                {
                    colors[i] = new Vector3((float)Math.Round(colors[i].X, 1),
                        (float)Math.Round(colors[i].Y, 1), (float)Math.Round(colors[i].Z, 1));
                    Color tmp = Color.Black;
                    if (colorSelection == 0) tmp.R = 255;
                    else if (colorSelection == 1) tmp.G = 255;
                    else if (colorSelection == 2) tmp.B = 255;
                    MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "<     >",
                        new Vector2(400, 402 + colorSelection * 38), tmp);
                }

            MainClass.spriteBatch.End();

            if (screen == Screen.playerSetting || screen == Screen.colorSetting)
            {
                ship = MainClass.ships[ships[playerSelection] - 1];
                shipTrans = MainClass.shipTransforms[ships[playerSelection] - 1];
                
                ((BasicEffect)ship.Meshes[0].Effects[0]).DiffuseColor = colors[playerSelection];

                MainClass.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                GamePlay.DrawModel(ship, Matrix.CreateRotationX(MathHelper.Pi / 2)
                    * Matrix.CreateRotationY((float)gameTime.TotalGameTime.TotalSeconds * 1.5f)
                    * Matrix.CreateScale(1.5f) * Matrix.CreateTranslation(5700, 1200, 0), shipTrans);
            }
        }

        enum Screen
        {
            main,
            graphics,
            audio,
            playerSelecting,
            playerSetting,
            colorSetting,
        }
    }
}
