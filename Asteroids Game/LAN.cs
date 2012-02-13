using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;

namespace AsteroidsGame
{
    class LAN
    {
        public static bool isActive = false;
        static int selection = 0;
        static AvailableNetworkSessionCollection availableSessions;

        public static NetworkSession session;
        public static PacketReader reader = new PacketReader();
        public static PacketWriter writer = new PacketWriter();
        public static LocalNetworkGamer localGamer;
        public static LinkedList<string> playerList = new LinkedList<string>();

        public static void Update()
        {
            if (Controls.getInput(Action.MenuUp))
            {
                MainClass.soundBank.PlayCue("Selection_change");
                if (availableSessions != null && availableSessions.Count != 0)
                    if (selection < 2)
                        selection = availableSessions.Count + 1;
                    else
                        if (selection == 2)
                            selection = 0;
                        else
                            selection--;
            }

            if (Controls.getInput(Action.MenuDown))
            {
                MainClass.soundBank.PlayCue("Selection_change");
                if (availableSessions != null && availableSessions.Count != 0)
                    if (selection == availableSessions.Count + 1)
                        selection = 0;
                    else
                        if (selection == 0)
                            selection = 2;
                        else
                            selection++;
            }

            if (Controls.getInput(Action.MenuRight) || Controls.getInput(Action.MenuLeft))
            {
                MainClass.soundBank.PlayCue("Selection_change");
                if (selection == 0)
                    selection = 1;
                else if (selection == 1)
                    selection = 0;
            }

            if (Controls.getInput(Action.Select))
            {
                MainClass.soundBank.PlayCue("Selection_choose");

                if (selection == 0)
                    try
                    {
                        availableSessions = NetworkSession.Find(NetworkSessionType.SystemLink, 1, null);
                    }
                    catch (GamerPrivilegeException)
                    {
                        Guide.ShowSignIn(1, false);
                    }
                else if (selection == 1)
                {
                    session = NetworkSession.Create(NetworkSessionType.SystemLink, 1, 12);
                    session.AllowHostMigration = true;
                    SetupSession();
                    localGamer.IsReady = true;
                    WaitingRoom.isActive = true;
                    isActive = false;
                }
                else
                {
                    session = NetworkSession.Join(availableSessions[selection - 2]);
                    SetupSession();
                    WaitingRoom.isActive = true;
                    isActive = false;
                }
            }

            if(Controls.getInput(Action.Back))
            {
                MainClass.soundBank.PlayCue("Selection_choose");
                TitleScreen.isActive = true;
                isActive = false;
                selection = 0;
            }
        }

        static void SetupSession()
        {
            session.GameStarted += new EventHandler<GameStartedEventArgs>(StartGame);
            session.GamerJoined += new EventHandler<GamerJoinedEventArgs>(GamerJoined);
            localGamer = session.LocalGamers[0];

            while (!localGamer.IsDataAvailable) ;

            NetworkGamer sender;
            localGamer.ReceiveData(reader, out sender);


            //foreach (NetworkGamer gamer in session.AllGamers)
            //    if (!gamer.IsLocal)
            //        while ((gamer.Tag as LinkedList<string>).Count != 0)
            //        {
            //            playerList.AddLast((gamer.Tag as LinkedList<string>).First.Value);
            //            (gamer.Tag as LinkedList<string>).RemoveFirst();
            //        }

            //LinkedList<string> tag = new LinkedList<string>();
            //int i = playerList.Count;
            //for (int j = 0; j < Settings.names.Length; j++)
            //    if (Settings.enabled[j])
            //    {
            //        tag.AddLast((i < 10 ? "0" : "") + i + "- " + Settings.names[j]);
            //        i++;
            //    }
            //localGamer.Tag = tag;

            //while (tag.Count != 0)
            //{
            //    playerList.AddLast(tag.First.Value);
            //    tag.RemoveFirst();
            //}
        }

        static void GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            //if (!e.Gamer.IsLocal)
            //    while ((e.Gamer.Tag as LinkedList<string>).Count != 0)
            //    {
            //        playerList.AddLast((e.Gamer.Tag as LinkedList<string>).First.Value);
            //        (e.Gamer.Tag as LinkedList<string>).RemoveFirst();
            //    }
        }

        static void StartGame(object sender, GameStartedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Game Started");
        }

        public static void Draw()
        {
            MainClass.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Refresh", new Vector2(50, 150), Color.Green);
            MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Host", new Vector2(400, 150), Color.Green);
            MainClass.spriteBatch.DrawString(TitleScreen.optionsFont, "Available Games:", new Vector2(50, 200),
                Color.Green);

            if (availableSessions == null || availableSessions.Count == 0)
                MainClass.spriteBatch.DrawString(GamePlay.scoreFont, "There are no LAN Games\nTry Refreshing the list",
                    new Vector2(50, 250), Color.Blue);
            else
                for (int i = 0; i < availableSessions.Count; i++)
                    MainClass.spriteBatch.DrawString(GamePlay.scoreFont, availableSessions[i].HostGamertag,
                        new Vector2(50, 250 + i * 50), Color.Blue);

            Vector2 selectorPos;
            if (selection == 0)
                selectorPos = new Vector2(40, 145);
            else if (selection == 1)
                selectorPos = new Vector2(390, 145);
            else
                selectorPos = new Vector2(40, 140 + selection * 50);
            MainClass.spriteBatch.Draw(Controls.selector, selectorPos, Color.White);

            MainClass.spriteBatch.End();
        }
    }
}
