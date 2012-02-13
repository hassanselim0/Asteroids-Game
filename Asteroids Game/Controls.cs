using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AsteroidsGame
{
    public class Controls
    {
        public static bool isActive = false;
        public static Texture2D selector;
        static Vector2 selectorPosition;
        static int selection = 0;
        static int player = 0;

        public static Input[][] inputs;
        static bool changeKey;
        static bool menu = true;

        public static void Update()
        {
            // Changes the Controls.
            if (changeKey)
            {
                changeKey = !inputs[player][selection].set(selection);
                if (!changeKey)
                    MainClass.soundBank.PlayCue("contols_change");
                return;
            }

            // Positions the selector according to the selection.
            if (menu)
                selectorPosition = new Vector2((player == 7 ? 420 : 30), 201 + player * 59);

            // Determines what to do when you select a choice.
            if (getInput(Action.Select))
            {
                MainClass.soundBank.PlayCue("Selection_choose");
                if (menu)
                {
                    if (player == 7)
                    {
                        TitleScreen.isActive = true;
                        isActive = false;
                        selection = 0;
                        player = 0;
                    }
                    else
                    {
                        menu = false;
                        selection = 0;
                    }
                }
                else if (selection == 7)
                    menu = true;
                else
                {
                    changeKey = true;
                    return;
                }
            }
            if (getInput(Action.Back))
            {
                MainClass.soundBank.PlayCue("Selection_choose");
                if (menu)
                {
                    TitleScreen.isActive = true;
                    isActive = false;
                    selection = 0;
                    player = 0;
                }
                else
                    menu = true;
            }

            // Move the selector.
            if (getInput(Action.MenuDown))
            {
                if (menu)
                    if (player == 5)
                        player = 7;
                    else
                        player += 1;
                else
                    selection += 1;
                MainClass.soundBank.PlayCue("Selection_change");
            }
            if (getInput(Action.MenuUp))
            {
                if (menu)
                    if (player == 7)
                        player = 5;
                    else
                        player -= 1;
                else
                    selection -= 1;
                MainClass.soundBank.PlayCue("Selection_change");
            }
            if (selection == -1)
                selection = 7;
            if (selection == 8)
                selection = 0;
            if (player == 8)
                player = 0;
            if (player == -1)
                player = 7;
        }

        public static void SaveControls()
        {
            string[] data = new string[42];

            for (int i = 0; i < inputs.Length; i++)
                for (int j = 0; j < inputs[i].Length; j++)
                    data[i * 7 + j] = inputs[i][j].getCode();

            File.WriteAllLines("Controls.data", data);
        }

        public static void LoadControls()
        {
            inputs = new Input[6][];
            for (int i = 0; i < 6; i++)
                inputs[i] = new Input[7];

            try
            {
                string[] data = File.ReadAllLines("Controls.data");

                if (data.Length != 42) throw new FileNotFoundException();

                for (int i = 0; i < inputs.Length; i++)
                    for (int j = 0; j < inputs[i].Length; j++)
                        inputs[i][j].set(data[i * 7 + j].Split(' '));
            }
            catch (FileNotFoundException)
            {
                ResetControls();
            }
        }

        public static void ResetControls()
        {
            inputs[0][0].set(Keys.Up);
            inputs[0][1].set(Keys.Down);
            inputs[0][2].set(Keys.Right);
            inputs[0][3].set(Keys.Left);
            inputs[0][4].set(Keys.Space);
            inputs[0][5].set(Keys.RightShift);
            inputs[0][6].set(Keys.B);
            inputs[1][0].set(Keys.W);
            inputs[1][1].set(Keys.S);
            inputs[1][2].set(Keys.D);
            inputs[1][3].set(Keys.A);
            inputs[1][4].set(Keys.T);
            inputs[1][5].set(Keys.Y);
            inputs[1][6].set(Keys.G);
            ResetPadControls();
        }

        private static void ResetPadControls()
        {
            for (int i = 2; i < 6; i++)
            {
                inputs[i][0].set(i - 2, true);
                inputs[i][1].set(i - 2, false);
                inputs[i][2].set(i - 2, false, true, true);
                inputs[i][3].set(i - 2, false, true, false);
                inputs[i][4].set(i - 2, Buttons.A);
                inputs[i][5].set(i - 2, Buttons.X);
                inputs[i][6].set(i - 2, Buttons.B);
            }
        }

        public static bool getInput(Action a)
        {
            switch (a)
            {
                case Action.MenuUp:
                    return Input.isPressed(Keys.Up)
                        || Input.isPressed(Buttons.RightThumbstickUp)
                        || Input.isPressed(Buttons.LeftThumbstickUp)
                        || Input.isPressed(Buttons.DPadUp);

                case Action.MenuDown:
                    return Input.isPressed(Keys.Down)
                        || Input.isPressed(Buttons.RightThumbstickDown)
                        || Input.isPressed(Buttons.LeftThumbstickDown)
                        || Input.isPressed(Buttons.DPadDown);

                case Action.MenuRight:
                    return Input.isPressed(Keys.Right)
                        || Input.isPressed(Buttons.RightThumbstickRight)
                        || Input.isPressed(Buttons.LeftThumbstickRight)
                        || Input.isPressed(Buttons.DPadRight);

                case Action.MenuLeft:
                    return Input.isPressed(Keys.Left)
                        || Input.isPressed(Buttons.RightThumbstickLeft)
                        || Input.isPressed(Buttons.LeftThumbstickLeft)
                        || Input.isPressed(Buttons.DPadLeft);

                case Action.Select:
                    return Input.isPressed(Keys.Enter)
                        || Input.isPressed(Buttons.A)
                        || Input.isPressed(Buttons.Start)
                        || Input.isPressed(Buttons.RightStick)
                        || Input.isPressed(Buttons.LeftStick);

                case Action.Back:
                    return Input.isPressed(Keys.Escape)
                        || Input.isPressed(Keys.Back)
                        || Input.isPressed(Buttons.Back)
                        || Input.isPressed(Buttons.B);

                case Action.Pause:
                    return Input.isPressed(Keys.Enter)
                        || Input.isPressed(Keys.Escape)
                        || Input.isPressed(Buttons.Start)
                        || Input.isPressed(Buttons.Back);
            }
            return false;
        }

        public static void Draw()
        {
            MainClass.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            if (menu)
            {
                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont,
                    "Player 1\nPlayer 2\nPlayer 3\nPlayer 4\nPlayer 5\nPlayer 6", new Vector2(50, 200), Color.Green);
                MainClass.spriteBatch.Draw(selector, selectorPosition, Color.White);
            }
            else
            {
                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont,
                    "Accelerate:\nDecelerate:\nRotate Right:\nRotate Left:\nShoot:\nWarp to Center:\nSpecial Weapon:",
                    new Vector2(50, 200), Color.Green);
                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Player " + (player + 1),
                    new Vector2(450, 150), Color.Blue);

                string tmp = "";
                for (int i = 0; i < 7; i++)
                    tmp += (changeKey && selection == i ? "Press a Key/Button ..."
                        : (inputs[player][i] + inputs[player][i].getIndex())) + "\n";
                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, tmp, new Vector2(456, 200), Color.Green);

                if (selection == 7)
                    MainClass.spriteBatch.Draw(selector, new Vector2(420, 615), Color.White);
                else
                    MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "__________",
                        new Vector2(50, 215 + selection * 59), Color.Red);
            }

            MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Back", new Vector2(482, 625), Color.Green);

            MainClass.spriteBatch.End();
        }
    }

    public enum Action
    {
        MenuUp,
        MenuDown,
        MenuRight,
        MenuLeft,
        Select,
        Back,
        Pause,
    }
}
