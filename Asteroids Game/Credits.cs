using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AsteroidsGame
{
    class Credits
    {
        public static bool isActive = false;
        public static Texture2D me;
        public static SpriteFont creditsFont;

        public static void Update()
        {
            if (Controls.getInput(Action.Select))
                System.Diagnostics.Process.Start("http://www.hassanselim.me/");
            else if (Input.isAnyKeyPressed())
            {
                MainClass.soundBank.PlayCue("Selection_choose");
                TitleScreen.isActive = true;
                isActive = false;
            }
        }

        public static void Draw()
        {
            MainClass.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            MainClass.spriteBatch.DrawString(creditsFont, "Programmed by: Hassan Aly Mahmoud Selim\na GUC Engineering student.\n"
                + "Programming Language:\nMicrosoft Visual C# and XNA Game Studio\n"
                + "Audio, 3D Models & Textures: XNA SpaceWars Starter Kit.\n"
                + "Some models where edited with XSI 6 Mod Tool & Wings 3D\n"
                + "All images where made using Microsoft Expression Design\n"
                + "More games coming soon \"en sha2 2allah\"\n"
                + "Check for updates on http://www.hassanselim.me/",
                new Vector2(30, 100), Color.GreenYellow);
            MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Press \"Space\" to open Website or any other key to go Back",
                new Vector2(10, 600), Color.Red);
            MainClass.spriteBatch.Draw(me, new Vector2(700, 100), Color.White);

            MainClass.spriteBatch.End();
        }
    }
}
