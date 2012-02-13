using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace AsteroidsGame
{
    public class Music
    {
        static Song introMusic;
        static Song[] musicList = new Song[4];
        static int track = 0;
        public static float volume = 0.4f;
        public static float fade = 0;

        public static void loadMusic(ContentManager content)
        {
            introMusic = content.Load<Song>("Music\\Techno");
            musicList[0] = content.Load<Song>("Music\\Talk");
            musicList[1] = content.Load<Song>("Music\\Breaking The Habbit");
            musicList[2] = content.Load<Song>("Music\\Square One");
            musicList[3] = content.Load<Song>("Music\\StarLight");

            MediaPlayer.Volume = volume;
        }

        public static void Update()
        {
            if (MediaPlayer.Volume >= volume)
                fade = 0;

            if (!TitleScreen.gameStarted)
            {
                if (MediaPlayer.State == MediaState.Stopped)
                    MediaPlayer.Play(introMusic);
                else if (MediaPlayer.Queue.ActiveSong != introMusic)
                    if (MediaPlayer.Volume == 0)
                        MediaPlayer.Play(introMusic);
                    else
                        fade = -0.01f;
            }

            else
            {
                if (MediaPlayer.State == MediaState.Stopped)
                    playNextTrack();
                else if (MediaPlayer.Queue.ActiveSong == introMusic)
                    if (MediaPlayer.Volume == 0)
                        MediaPlayer.Play(musicList[new Random().Next(musicList.Length)]);
                    else
                        fade = -0.01f;
            }

            if (MediaPlayer.Volume == 0)
                fade = 0.01f;

            MediaPlayer.Volume += fade;
        }

        public static void playNextTrack()
        {
            if (track == musicList.Length - 1)
                track = 0;
            else
                track++;

            MediaPlayer.Play(musicList[track]);
        }
    }
}
