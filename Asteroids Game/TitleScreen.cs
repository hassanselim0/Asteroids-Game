using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AsteroidsGame
{
    class TitleScreen
    {
        public static bool isActive = false;
        public static bool gameStarted = false;
        public static bool exitWarning = false;
        public static bool exit = false;
        public static Texture2D selector;
        public static Texture2D ExitWarningY;
        public static Texture2D ExitWarningN;
        public static SpriteFont optionsFont;
        static string[] options = new string[7];
        static Color[] optionColor = new Color[7];
        static int selection = 1;
        static int exitWarningSelection = 2;
        static SpriteBatch spriteBatch = new SpriteBatch(MainClass.graphics.GraphicsDevice);

        public static void Update(ContentManager Content)
        {
            if (Controls.getInput(Action.MenuDown) && !exitWarning)
            {
                selection += 1;
                MainClass.soundBank.PlayCue("Selection_change");
            }
            if (Controls.getInput(Action.MenuUp) && !exitWarning)
            {
                selection -= 1;
                MainClass.soundBank.PlayCue("Selection_change");
            }
            if (selection == 0)
                selection = 7;
            if (selection == 8)
                selection = 1;

            if (Controls.getInput(Action.MenuRight) && exitWarning)
            {
                exitWarningSelection += 1;
                MainClass.soundBank.PlayCue("Selection_change");
            }
            if (Controls.getInput(Action.MenuLeft) && exitWarning)
            {
                exitWarningSelection -= 1;
                MainClass.soundBank.PlayCue("Selection_change");
            }
            if (exitWarningSelection == 0)
                exitWarningSelection = 1;
            if (exitWarningSelection == 3)
                exitWarningSelection = 2;

            if (Controls.getInput(Action.Select) && !exitWarning)
            {
                MainClass.soundBank.PlayCue("Selection_choose");
                switch (selection)
                {
                    case 1:
                        if (!gameStarted)  //Start New Game.
                        {
                            GamePlay.StartGame();
                            GamePlay.Reset();
                            gameStarted = true;
                        }
                        if (WinningScreen.isActive && !GamePlay.gameOver)  //Proceed to next Level.
                        {
                            GamePlay.level += 1;
                            GamePlay.Reset();
                        }
                        if (!GamePlay.gameOver)
                        {
                            GamePlay.isActive = true;
                            isActive = false;
                        }
                        break;
                    case 2:
                        if (gameStarted)  //Restart the Game.
                        {
                            gameStarted = false;
                            GamePlay.level = 1;
                            GameConstants.NumAsteroids = 0;
                            GameConstants.AsteroidSpeedAdjustment = 6;
                            for (int i = 0; i < GamePlay.playerList.Length; i++)
                            {
                                GamePlay.playerList[i].life = 5;
                                GamePlay.playerList[i].score = 0;
                            }
                            GamePlay.gameOver = false;
                            HighScores.newHighScore = false;
                            WinningScreen.isActive = false;
                        }
                        else  //Opens the Lobby to Host/Join a LAN Game.
                        {
                            LAN.isActive = true;
                            isActive = false;
                        }
                        selection = 1;
                        break;
                    case 3:  //Open HighScores Page.
                        HighScores.isActive = true;
                        break;
                    case 4:  //Open Controls Page.
                        Controls.isActive = true;
                        isActive = false;
                        break;
                    case 5:  //Opens Settings Page.
                        Settings.isActive = true;
                        isActive = false;
                        break;
                    case 6:  //Shows the Credits.
                        Credits.isActive = true;
                        isActive = false;
                        break;
                    case 7:  //Shows the Exit Warning.
                        exitWarning = true;
                        break;
                }
            }
            else if (Controls.getInput(Action.Select) && exitWarning)
            {
                MainClass.soundBank.PlayCue("Selection_choose");
                if (exitWarningSelection == 1)
                    exit = true;
                else
                    exitWarning = false;
            }
            if (Controls.getInput(Action.Back))
            {
                MainClass.soundBank.PlayCue("Selection_choose");
                exitWarning = true;
            }
        }

        public static void Draw()
        {
            for (int i = 0; i < optionColor.Length; i++)
                optionColor[i] = Color.Green;
            if (gameStarted)
                if (WinningScreen.isActive)
                {
                    options[0] = "Next Level";
                    if (GamePlay.gameOver)
                        optionColor[0] = Color.Gray;
                }
                else
                    options[0] = "Resume";
            else
                options[0] = "Start Game !";
            if (gameStarted)
                options[1] = "Restart Game";
            else
                options[1] = "Play on LAN";
            options[2] = "High Scores";
            options[3] = "Controls";
            options[4] = "Settings";
            options[5] = "Credits";
            options[6] = "Exit";

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            spriteBatch.Draw(selector, new Vector2(400, 340), Color.White);
            spriteBatch.DrawString(optionsFont, options[selection == 1 ? 6 : selection - 2], new Vector2(420, 300),
                optionColor[selection == 1 ? 6 : selection - 2], 0, new Vector2(0, 36), 0.6f, SpriteEffects.None, 0);
            spriteBatch.DrawString(optionsFont, options[selection - 1], new Vector2(420, 400), optionColor[selection - 1], 0,
                new Vector2(0, 36), 1.4f, SpriteEffects.None, 0);
            spriteBatch.DrawString(optionsFont, options[selection == 7 ? 0 : selection], new Vector2(420, 500),
                optionColor[selection == 7 ? 0 : selection], 0, new Vector2(0, 36), 0.5f, SpriteEffects.None, 0);
            
            if (exitWarning && exitWarningSelection == 1)
                spriteBatch.Draw(ExitWarningY, new Vector2(212, 309), Color.White);
            else if (exitWarning && exitWarningSelection == 2)
                spriteBatch.Draw(ExitWarningN, new Vector2(212, 309), Color.White);
            
            spriteBatch.End();
        }
    }
}
