using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MusicPlayer.Helpers;
using MusicPlayer.Models;
using Newtonsoft.Json;
using Uri = Android.Net.Uri;

namespace MusicPlayer
{
    [Activity(Label = "Album Activity")]
    public class AlbumActivity : Activity
    {
        private const string MusicDataFileName = "musicData.json";
        private Album _album;
        private Song _currentSong;

        private static readonly MediaPlayer Player = new MediaPlayer();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Album);
            // Create your application here

            var startPause = FindViewById<Button>(Resource.Id.startpause);
            var nextSong = FindViewById<Button>(Resource.Id.nextSong);
            var previousSong = FindViewById<Button>(Resource.Id.previousSong);
            var songProgressBar = FindViewById<SeekBar>(Resource.Id.songProgressBar);

            startPause.Click += delegate
            {
                if (Player.IsPlaying)
                {
                    Player.Pause();
                }
                else
                {
                    Player.Start();
                }
            };
            nextSong.Click += delegate
            {
                var nextSongIndex = _album.Songs.IndexOf(_currentSong) + 1;

                if (nextSongIndex >= _album.Songs.Count)
                {
                    Player.Reset();
                    _currentSong = null;
                }
                else
                {
                    _currentSong = _album.Songs[nextSongIndex];
                    Player.Reset();
                    var uri = Uri.Parse(_currentSong.SongPath);
                    Player.SetAudioStreamType(Stream.Music);
                    Player.SetDataSource(ApplicationContext, uri);
                    Player.Prepare();
                    Player.Start();
                    songProgressBar.Max = Player.Duration;
                    songProgressBar.Progress = 0;
                }
            };
            previousSong.Click += delegate
            {
                var nextSongIndex = _album.Songs.IndexOf(_currentSong) - 1;

                if (nextSongIndex < 0)
                {
                    Player.Reset();
                    _currentSong = null;
                }
                else
                {
                    _currentSong = _album.Songs[nextSongIndex];
                    Player.Reset();
                    var uri = Uri.Parse(_currentSong.SongPath);
                    Player.SetAudioStreamType(Stream.Music);
                    Player.SetDataSource(ApplicationContext, uri);
                    Player.Prepare();
                    Player.Start();
                    songProgressBar.Max = Player.Duration;
                    songProgressBar.Progress = 0;
                }
            };


            Title = Intent.GetStringExtra("AlbumName") ?? "";
            if (Title != "")
            {
                _album = ApplicationData.Albums.SingleOrDefault(a => a.Name == Title);
            }

            if (_album == null)
                return;

            var layout = FindViewById<LinearLayout>(Resource.Id.linearSongsLayout);

            for (int i = 0; i < _album.Songs.Count; i++)
            {
                var button = new Button(this)
                {
                    Text = $"{i+1} {_album.Songs[i].Title}",
                    Id = i
                };
                button.Click += (sender, args) =>
                {
                    _currentSong = _album.Songs[button.Id];

                    if (_currentSong != null)
                    {
                        Player.Reset();
                        var uri = Uri.Parse(_currentSong.SongPath);
                        Player.SetAudioStreamType(Stream.Music);
                        Player.SetDataSource(ApplicationContext, uri);
                        Player.Prepare();
                        Player.Start();
                        songProgressBar.Max = Player.Duration;
                        songProgressBar.Progress = 0;
                    }
                };

                layout.AddView(button);
            }

            
            var progressText = FindViewById<TextView>(Resource.Id.progressText);
            songProgressBar.StopTrackingTouch += delegate
            {
                progressText.Text = $"{songProgressBar.Progress}";
                Player.SeekTo(songProgressBar.Progress);
            };

            SetCountDown();
        }

        private void SetCountDown()
        {
            var songProgressBar = FindViewById<SeekBar>(Resource.Id.songProgressBar);
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += (sender, args) =>
            {
                songProgressBar.Progress = Player.CurrentPosition;
            };
            timer.Enabled = true;
        }
    }
}