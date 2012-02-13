using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AsteroidsGame
{
    class HighScores
    {
        public static bool isActive = false;
        public static bool newHighScore = false;
        static string newHighScoreName = "";
        static int cursorPos = 0;
        public static int[] newHighScoreOrder;
        static int player = 0;
        static string[] names = new string[10];
        static int[] levels = new int[10];
        static int[] scores = new int[10];
        static int timer = 0;
        static int screen = 1;

        public static void Update(GameTime gameTime)
        {
            // Input for Viewing the Highscores
            if (!newHighScore)
            {
                if (Input.isAnyKeyPressed())
                {
                    MainClass.soundBank.PlayCue("Selection_choose");
                    if (screen == 1)
                        screen = 2;
                    else
                    {
                        TitleScreen.isActive = true;
                        isActive = false;
                        screen = 1;
                    }
                }
            }

            // Checking for a new highscore and storing it's ranking.
            if (GamePlay.gameOver && WinningScreen.isActive && !TitleScreen.isActive)
                for (int i = newHighScoreOrder.Length == 1 ? 0 : 5; i < (newHighScoreOrder.Length == 1 ? 5 : 10); i++)
                    if (GamePlay.playerList[player].score > scores[i])
                    {
                        newHighScore = true;
                        newHighScoreOrder[player] = i;
                        break;
                    }
                    else
                        newHighScore = false;

            // Text Input
            if (!TitleScreen.isActive && newHighScore)
            {
                if (Input.isAnyKeyPressed())
                {
                    timer += 1;
                    if (timer > 10)
                        timer = 0;
                }
                else
                    timer = 0;

                if (timer == 1)
                {
                    if (Input.isDown(Keys.Back) && cursorPos != 0)
                        newHighScoreName = newHighScoreName.Remove(--cursorPos, 1);
                    if (Input.isDown(Keys.Left) && cursorPos != 0)
                        cursorPos--;
                    if (Input.isDown(Keys.Right) && cursorPos != newHighScoreName.Length)
                        cursorPos++;
                }

                newHighScoreName = newHighScoreName.Insert(cursorPos, Input.getText());
                cursorPos += Input.getText().Length;

                // HighScore Recording
                if (Input.isPressed(Keys.Enter))
                {
                    for (int i = newHighScoreOrder.Length == 1 ? 4 : 9; i >= newHighScoreOrder[player]; i--)
                        if (i == newHighScoreOrder[player])
                        {
                            names[i] = newHighScoreName;
                            scores[i] = GamePlay.playerList[player].score;
                            levels[i] = GamePlay.level;
                        }
                        else if (i != 0 && i != 5)
                        {
                            names[i] = names[i - 1];
                            scores[i] = scores[i - 1];
                            levels[i] = levels[i - 1];
                            for (int j = 0; j < newHighScoreOrder.Length; j++)
                                if (newHighScoreOrder[j] == i - 1 && j != player)
                                    newHighScoreOrder[j]++;
                        }
                    newHighScore = false;
                    newHighScoreName = "";
                    cursorPos = 0;
                    MainClass.soundBank.PlayCue("Selection_choose");
                    if (player == newHighScoreOrder.Length - 1)
                    {
                        player = 0;
                        TitleScreen.isActive = true;
                    }
                    else
                        player++;
                }
            }
        }

        public static void SaveHighScores()
        {
            string[] data = new string[30];

            for (int i = 0; i < 5; i++)
                for (int j = 0; j < names[i].Length; j++)
                {
                    data[i] += (char)(names[i].ToCharArray()[j] + 1);
                }
            for (int i = 5; i < 10; i++)
                data[i] = (levels[i - 5] / 4.0 + 10).ToString();
            for (int i = 10; i < 15; i++)
                data[i] = (scores[i - 10] / 4.0 + 10).ToString();

            for (int i = 15; i < 20; i++)
                for (int j = 0; j < names[i - 10].Length; j++)
                {
                    data[i] += (char)(names[i - 10].ToCharArray()[j] + 1);
                }
            for (int i = 20; i < 25; i++)
                data[i] = (levels[i - 15] / 4.0 + 10).ToString();
            for (int i = 25; i < 30; i++)
                data[i] = (scores[i - 20] / 4.0 + 10).ToString();

            File.WriteAllLines("HighScores.data", data);
        }

        public static void LoadHighScores()
        {
            try
            {
                string[]  data = File.ReadAllLines("HighScores.data");

                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < data[i].Length; j++)
                    {
                        names[i] += (char)(data[i].ToCharArray()[j] - 1);
                    }
                    for (int j = 0; j < 5; j++)
                        scores[i] = (int)(float.Parse(data[j + 10]) * 4 - 40);
                }

                if (data.Length == 10)
                    throw new FileNotFoundException();

                if (data.Length == 15)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        names[i + 5] = "None";
                        levels[i] = (int)(float.Parse(data[i + 5]) * 4 - 40);
                        levels[i + 5] = 0;
                        scores[i + 5] = 0;
                    }
                }
                else
                {
                    for (int i = 5; i < 10; i++)
                        levels[i - 5] = (int)(float.Parse(data[i]) * 4 - 40);
                    for (int i = 10; i < 15; i++)
                        scores[i - 10] = (int)(float.Parse(data[i]) * 4 - 40);
                    for (int i = 15; i < 20; i++)
                        for (int j = 0; j < data[i].Length; j++)
                            names[i - 10] += (char)(data[i].ToCharArray()[j] - 1);
                    for (int i = 20; i < 25; i++)
                        levels[i - 15] = (int)(float.Parse(data[i]) * 4 - 40);
                    for (int i = 25; i < 30; i++)
                        scores[i - 20] = (int)(float.Parse(data[i]) * 4 - 40);
                }

                for (int i = 0; i < names.Length; i++)
                    if (names[i] == null)
                        names[i] = "";
            }
            catch (FileNotFoundException)
            {
                ResetHighScores();
            }

        }

        public static void ResetHighScores()
        {
            for (int i = 0; i < 10; i++)
            {
                names[i] = "None";
                levels[i] = 0;
                if (i < 5)
                    scores[i] = 5 - i;
                else
                    scores[i] = 10 - i;
            }
        }

        public static void Draw(GameTime gameTime)
        {
            if (WinningScreen.isActive && newHighScore)
            {
                MainClass.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                MainClass.spriteBatch.DrawString(GamePlay.winFont, "Player " + (player + 1) + " made a High Score !",
                    new Vector2(0, 192), Color.Gold);
                MainClass.spriteBatch.DrawString(GamePlay.winFont, "Enter Your Name:",
                    new Vector2(0, 292), Color.Gold);
                float tmp = newHighScoreName == "" ? 0
                    : TitleScreen.optionsFont.MeasureString(newHighScoreName.Length == cursorPos ? newHighScoreName
                    : newHighScoreName.Remove(cursorPos)).X;
                if (gameTime.TotalGameTime.TotalSeconds % 1 < 0.6)
                    MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "_", new Vector2(650 + tmp, 344),
                        Color.Green);
                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, newHighScoreName, new Vector2(650, 342),
                        Color.Green);
                MainClass.spriteBatch.End();
            }
            else
            {
                MainClass.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont,
                    (screen == 1 ? "Single" : "Multi") + "Player Mode", new Vector2(400, 150), Color.Red);
                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Name", new Vector2(100, 200), Color.Blue);
                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Level", new Vector2(800, 200), Color.Blue);
                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Score", new Vector2(900, 200), Color.Blue);
                for (int i = 0; i < 5; i++)
                {
                    Color temp = Color.Gold;
                    for (int j = 0; GamePlay.gameOver && j < newHighScoreOrder.Length; j++)
                        if (newHighScoreOrder[j] == (screen == 1 ? i : i + 5))
                            temp = Color.Red;
                    MainClass.spriteBatch.DrawString(TitleScreen.optionsFont,
                        names[screen == 1 ? i : i + 5], new Vector2(100, 250 + i * 50), temp);
                    MainClass.spriteBatch.DrawString(TitleScreen.optionsFont,
                        levels[screen == 1 ? i : i + 5].ToString(), new Vector2(800, 250 + i * 50), temp);
                    MainClass.spriteBatch.DrawString(TitleScreen.optionsFont,
                        scores[screen == 1 ? i : i + 5].ToString(), new Vector2(900, 250 + i * 50), temp);
                }
                MainClass.spriteBatch.End();
            }
        }
    }
}
