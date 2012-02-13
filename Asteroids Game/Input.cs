using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AsteroidsGame
{
    public struct Input
    {
        static KeyboardState currentKeyboard;
        static KeyboardState lastKeyboard;
        static GamePadState[] currentGamepad = new GamePadState[4];
        static GamePadState[] lastGamepad = new GamePadState[4];

        Keys key;
        Buttons btn;
        bool trigs;
        bool thumb;
        bool right;
        bool xAxis;
        bool pstv;
        int index;

        public void set(Keys k)
        {
            clear();
            key = k;
        }

        public void set(int i, Buttons b)
        {
            clear();
            index = i;
            btn = b;
        }

        public void set(int i, bool r)
        {
            clear();
            index = i;
            trigs = true;
            right = r;
        }

        public void set(int i, bool r, bool x, bool p)
        {
            clear();
            index = i;
            thumb = true;
            right = r;
            xAxis = x;
            pstv = p;
        }

        public void set(string[] s)
        {
            if (s.Length == 1)
                set((Keys)int.Parse(s[0]));
            else if (s.Length == 2)
                if (char.IsDigit(s[1][0]))
                    set(int.Parse(s[0]), (Buttons)int.Parse(s[1]));
                else
                    set(int.Parse(s[0]), bool.Parse(s[1]));
            else if (s.Length == 4)
                set(int.Parse(s[0]), bool.Parse(s[1]), bool.Parse(s[2]), bool.Parse(s[3]));
        }

        public bool set(int a)
        {
            if (currentKeyboard.IsKeyDown(Keys.Escape) || isPressed(Buttons.Back))
                return true;

            if (currentKeyboard.GetPressedKeys().Length != 0 && currentKeyboard.IsKeyUp(Keys.Enter)
                && currentKeyboard.IsKeyUp(Keys.Tab))
            {
                set(currentKeyboard.GetPressedKeys()[0]);
                return true;
            }

            for (int i = 0; i < 4; i++)
            {
                if (currentGamepad[i].ThumbSticks.Right.Length() != 0)
                {
                    if (a == 0 || a == 1)
                        set(i, true, false, a == 0);
                    else if (a == 2 || a == 3)
                        set(i, true, true, a == 2);
                    else if (Math.Abs(currentGamepad[i].ThumbSticks.Right.X)
                            > Math.Abs(currentGamepad[i].ThumbSticks.Right.Y))
                        set(i, true, true, currentGamepad[i].ThumbSticks.Right.X >= 0);
                    else
                        set(i, true, false, currentGamepad[i].ThumbSticks.Right.Y >= 0);
                    return true;
                }
                if (currentGamepad[i].ThumbSticks.Left.Length() != 0)
                {
                    if (a == 0 || a == 1)
                        set(i, false, false, a == 0);
                    else if (a == 2 || a == 3)
                        set(i, false, true, a == 2);
                    else if (Math.Abs(currentGamepad[i].ThumbSticks.Left.X)
                            > Math.Abs(currentGamepad[i].ThumbSticks.Left.Y))
                        set(i, false, true, currentGamepad[i].ThumbSticks.Left.X >= 0);
                    else
                        set(i, false, false, currentGamepad[i].ThumbSticks.Left.Y >= 0);
                    return true;
                }
                if (currentGamepad[i].Triggers.Right != 0)
                {
                    set(i, true);
                    return true;
                }
                if (currentGamepad[i].Triggers.Left != 0)
                {
                    set(i, false);
                    return true;
                }
                for (int j = 0; j <= 23; j++)
                {
                    if (j == 10)
                        j = 11;
                    if (i == 16)
                        j = 22;
                    int x = (int)Math.Pow(2, j);
                    if (currentGamepad[i].IsButtonDown(((Buttons)x)) && lastGamepad[i].IsButtonUp(((Buttons)x)))
                    {
                        set(i, (Buttons)x);
                        return true;
                    }
                }
            }

            return false;
        }

        private float getValue(bool current)
        {
            if (key != 0)
            {
                KeyboardState ks = current ? currentKeyboard : lastKeyboard;
                return ks.IsKeyDown(key) ? 1 : 0;
            }

            if (btn != 0)
            {
                GamePadState gs = current ? currentGamepad[index] : lastGamepad[index];
                return gs.IsButtonDown(btn) ? 1 : 0;
            }

            if (trigs)
            {
                GamePadTriggers tr = current ? currentGamepad[index].Triggers : lastGamepad[index].Triggers;
                return right ? tr.Right : tr.Left;
            }

            if (thumb)
            {
                GamePadThumbSticks ts = current ? currentGamepad[index].ThumbSticks : lastGamepad[index].ThumbSticks;
                Vector2 v = right ? ts.Right : ts.Left;
                float f = xAxis ? v.X : v.Y;
                if (f >= 0)
                    return pstv ? f : 0;
                else
                    return pstv ? 0 : -f;
            }

            return 0;
        }

        public float getValue()
        {
            return getValue(true);
        }

        public bool isPressed()
        {
            return getValue(true) != 0 && getValue(false) == 0;
        }

        public string getCode()
        {
            if (key != 0)
                return ((int)key).ToString();
            if (btn != 0)
                return index + " " + (int)btn;
            if (trigs)
                return index + " " + right;
            if (thumb)
                return index + " " + right + " " + xAxis + " " + pstv;
            return "";
        }

        public string getIndex()
        {
            if (key == 0)
                return " (Gamepad " + (index + 1) + ")";
            return "";
        }

        public void clear()
        {
            key = 0;
            btn = 0;
            thumb = false;
            trigs = false;
        }

        public override string ToString()
        {
            if (key != 0)
                return key + " Key";
            if (btn != 0)
                return btn + " Button";
            if (thumb)
                return (right ? "Right" : "Left") + " ThumbStick "
                    + (xAxis ? (pstv ? "right" : "left") : (pstv ? "up" : "down"));
            if (trigs)
                return (right ? "Right" : "Left") + " Trigger";
            return "none";
        }

        public static void updateStates()
        {
            currentKeyboard = Keyboard.GetState();
            for (int i = 0; i < 4; i++)
                currentGamepad[i] = GamePad.GetState((PlayerIndex)i);
        }

        public static void outdateStates()
        {
            lastKeyboard = currentKeyboard;
            for (int i = 0; i < 4; i++)
                lastGamepad[i] = currentGamepad[i];
        }

        public static bool isAnyKeyPressed()
        {
            return currentKeyboard.GetPressedKeys().Length != 0 && lastKeyboard.GetPressedKeys().Length == 0;
        }

        public static bool isDown(Keys k)
        {
            return currentKeyboard.IsKeyDown(k);
        }

        public static bool isPressed(Keys k)
        {
            return isDown(k) && lastKeyboard.IsKeyUp(k);
        }

        public static bool isPressed(Buttons b)
        {
            for (int i = 0; i < 4; i++)
                if (currentGamepad[i].IsButtonDown(b) && lastGamepad[i].IsButtonUp(b))
                    return true;
            return false;
        }

        private static bool isShiftPressed()
        {
            return currentKeyboard.IsKeyDown(Keys.RightShift) || currentKeyboard.IsKeyDown(Keys.LeftShift);
        }

        public static string getText()
        {
            string s = "";

            for (int i = 65; i <= 90; i++)
                if (isPressed((Keys)i))
                    if (isShiftPressed())
                        s += (char)i;
                    else
                        s += (char)(i + 32);

            if (isPressed(Keys.D1)) s += isShiftPressed() ? "!" : "1";
            if (isPressed(Keys.D2)) s += isShiftPressed() ? "@" : "2";
            if (isPressed(Keys.D3)) s += isShiftPressed() ? "#" : "3";
            if (isPressed(Keys.D4)) s += isShiftPressed() ? "$" : "4";
            if (isPressed(Keys.D5)) s += isShiftPressed() ? "%" : "5";
            if (isPressed(Keys.D6)) s += isShiftPressed() ? "^" : "6";
            if (isPressed(Keys.D7)) s += isShiftPressed() ? "&" : "7";
            if (isPressed(Keys.D8)) s += isShiftPressed() ? "*" : "8";
            if (isPressed(Keys.D9)) s += isShiftPressed() ? "(" : "9";
            if (isPressed(Keys.D0)) s += isShiftPressed() ? ")" : "0";

            if (isPressed(Keys.OemPeriod))          s += isShiftPressed() ? ">" : ".";
            if (isPressed(Keys.OemComma))           s += isShiftPressed() ? "<" : ",";
            if (isPressed(Keys.OemQuestion))        s += isShiftPressed() ? "?" : "/";
            if (isPressed(Keys.OemPipe))            s += isShiftPressed() ? "|" : "\\";
            if (isPressed(Keys.OemQuotes))          s += isShiftPressed() ? "\"" : "'";
            if (isPressed(Keys.OemSemicolon))       s += isShiftPressed() ? ":" : ";";
            if (isPressed(Keys.OemMinus))           s += isShiftPressed() ? "_" : "-";
            if (isPressed(Keys.OemPlus))            s += isShiftPressed() ? "+" : "=";
            if (isPressed(Keys.OemOpenBrackets))    s += isShiftPressed() ? "{" : "[";
            if (isPressed(Keys.OemCloseBrackets))   s += isShiftPressed() ? "}" : "]";

            if (currentKeyboard.IsKeyDown(Keys.Space) && lastKeyboard.IsKeyUp(Keys.Space))
                s += " ";

            return s;
        }
    }
}
