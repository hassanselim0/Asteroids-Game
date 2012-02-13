using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsteroidsGame
{
    class WinningScreen
    {
        public static bool isActive = false;
        public static string draw = "";
        public static Vector2 winPosition = new Vector2(256, 192);

        public static void Draw(GameTime gameTime, int playerIndex)
        {
            // Gets the players who are DRAW.
            draw = "";
            int max = 0;
            for (int i = 0; i < GamePlay.playerList.Length;i++ )
                if (GamePlay.playerList[i].score > max)
                    max = GamePlay.playerList[i].score;
            for (int i = 0; i < GamePlay.playerList.Length; i++)
                if (GamePlay.playerList[i].score == max)
                    draw += (GamePlay.playerList[i].index + 1) + " & ";

            MainClass.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            if (GamePlay.playerList.Length != 1)
            {
                if (GamePlay.gameOver)
                    if (draw.Length > 4)
                        MainClass.spriteBatch.DrawString(GamePlay.winFont, "It's a Draw between players: \n"
                            + draw.TrimEnd(new char[] { ' ', '&', ' ' }), winPosition - new Vector2(240, 150), Color.Gold);
                    else
                        MainClass.spriteBatch.DrawString(GamePlay.winFont, "Player " + (playerIndex + 1) + " Wins !!!", winPosition,
                        Color.Gold);
                else
                    MainClass.spriteBatch.DrawString(GamePlay.winFont, "Level Complete", winPosition, Color.Gold);

            }
            else
            {
                if (GamePlay.gameOver)
                    MainClass.spriteBatch.DrawString(GamePlay.winFont, "Game Over", winPosition, Color.Red);
                else
                    MainClass.spriteBatch.DrawString(GamePlay.winFont, "You Win !!!", winPosition, Color.Gold);
            }

            MainClass.spriteBatch.DrawString(GamePlay.winFont, "Score: " + GamePlay.playerList[playerIndex].score,
                winPosition + new Vector2(150, 110), Color.Goldenrod);

            if (GamePlay.gameOver)
                MainClass.spriteBatch.DrawString(GamePlay.winFont, "Last LVL: " + GamePlay.level,
                    winPosition + new Vector2(150, 210), Color.Goldenrod);
            else
                MainClass.spriteBatch.DrawString(GamePlay.winFont, "Time: " + (GamePlay.time - GamePlay.time % 0.01) + " sec",
                    winPosition + new Vector2(150, 210), Color.Goldenrod);

            if (gameTime.TotalGameTime.TotalSeconds % 0.5 < 0.35)
                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Press \"Enter\"", new Vector2(350, 550), Color.Red);

            MainClass.spriteBatch.End();

            if (draw.Length < 5)
            {
                GamePlay.playerList[playerIndex].ship.isActive = true;
                GamePlay.playerList[playerIndex].ship.Position = new Vector3(-5000, -1000, 0);
                GamePlay.playerList[playerIndex].ship.Rotation = 0.0f;
                GamePlay.playerList[playerIndex].ship.RotationMatrix = Matrix.CreateRotationX((float)MathHelper.PiOver2)
                    * Matrix.CreateRotationY((float)(MathHelper.Pi / 8 * Math.Sin(gameTime.TotalGameTime.TotalSeconds * 2))
                    + MathHelper.Pi / 16);
            }
            for (int i = 0; i < GamePlay.playerList.Length;i++ )
                if (GamePlay.playerList[i].index != playerIndex || draw.Length > 4)
                    GamePlay.playerList[i].ship.isActive = false;
        }
    }
}
