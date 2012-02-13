using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

namespace AsteroidsGame
{
    class WaitingRoom
    {
        public static bool isActive = false;
        static int selection = 0;
        static string message = "";
        static LinkedList<string> chat = new LinkedList<string>();

        public static void Update()
        {
            if (selection == 2)
            {
                if (GamePlay.scoreFont.MeasureString(message).X < 800)
                    message += Input.getText();
                if (Input.isPressed(Keys.Back) && message.Length != 0)
                    message = message.Remove(message.Length - 1);
                if (Input.isPressed(Keys.Enter))
                {
                    LAN.writer.Write(message);
                    LAN.localGamer.SendData(LAN.writer, SendDataOptions.Chat);
                    message = "";
                }
            }
            while (LAN.localGamer.IsDataAvailable)
            {
                NetworkGamer sender;
                LAN.localGamer.ReceiveData(LAN.reader, out sender);
                chat.AddFirst(sender.Gamertag + ": " + LAN.reader.ReadString());
                if (chat.Count == 16)
                    chat.RemoveLast();
            }

            if (Controls.getInput(Action.MenuUp))
            {
                MainClass.soundBank.PlayCue("Selection_change");
                if (selection == 0)
                    selection = 2;
                else
                    selection--;
            }

            if (Controls.getInput(Action.MenuDown))
            {
                MainClass.soundBank.PlayCue("Selection_change");
                if (selection == 2)
                    selection = 0;
                else
                    selection++;
            }

            if (Controls.getInput(Action.Select))
            {
                MainClass.soundBank.PlayCue("Selection_choose");
                if (selection == 0)
                    if (LAN.session.IsHost)
                        if (LAN.session.IsEveryoneReady)
                            LAN.session.StartGame();
                        else
                        {
                            LAN.writer.Write("R?");
                            LAN.localGamer.SendData(LAN.writer, SendDataOptions.Chat);
                        }
                    else
                        LAN.localGamer.IsReady = true;
                if (selection == 1)
                {
                    LAN.session.Dispose();
                    LAN.session = null;
                    LAN.isActive = true;
                    isActive = false;
                    message = "";
                    chat = new LinkedList<string>();
                }
            }

            if (Controls.getInput(Action.Back) && selection != 2)
            {
                MainClass.soundBank.PlayCue("Selection_choose");
                LAN.session.Dispose();
                LAN.session = null;
                LAN.isActive = true;
                isActive = false;
                message = "";
                chat = new LinkedList<string>();
            }
        }

        public static void Draw()
        {
            MainClass.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            if (LAN.session.IsHost)
                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Start GAME", new Vector2(50, 200),
                    Color.Green);
            else
                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Ready", new Vector2(50, 200),
                    LAN.session.LocalGamers[0].IsReady ? Color.Red : Color.Green);
            MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Leave", new Vector2(50, 250), Color.Green);

            MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Players:", new Vector2(300, 150), Color.Green);
            LinkedListNode<string> curr = LAN.playerList.First;
            int i = 0;
            while (curr != null)
            {
                MainClass.spriteBatch.DrawString(GamePlay.scoreFont, curr.Value,
                    new Vector2(300 + i / 4 * 200, 210 + i % 4 * 25), Color.Blue);
                curr = curr.Next;
                i++;
            }

            MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Chat:", new Vector2(50, 300), Color.Green);
            MainClass.spriteBatch.DrawString(GamePlay.scoreFont, message, new Vector2(130, 320), Color.Blue);
            LinkedListNode<string> current = chat.First;
            float y = 350;
            while (current != null)
            {
                if (current.Value.EndsWith(": R?"))
                    MainClass.spriteBatch.DrawString(GamePlay.scoreFont,
                        "The Host wants to START, please click on the READY option", new Vector2(50, y), Color.Red);
                else
                    MainClass.spriteBatch.DrawString(GamePlay.scoreFont,
                        current.Value, new Vector2(50, y), Color.Blue);
                current = current.Next;
                y += 25;
            }

            if (selection == 0)
                MainClass.spriteBatch.Draw(Controls.selector, new Vector2(40, 195), Color.White);
            else if (selection == 1)
                MainClass.spriteBatch.Draw(Controls.selector, new Vector2(40, 245), Color.White);
            else
                MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "___", new Vector2(50, 310), Color.Red);

            MainClass.spriteBatch.End();
        }
    }
}
