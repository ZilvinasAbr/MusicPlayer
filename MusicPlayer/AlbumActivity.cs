using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private string _albumName;
        private Album _album;
        private Song _currentSong;

        private static readonly MediaPlayer Player = new MediaPlayer();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Album);

            var startPause = FindViewById<Button>(Resource.Id.startpause);
            var nextSong = FindViewById<Button>(Resource.Id.nextSong);
            var previousSong = FindViewById<Button>(Resource.Id.previousSong);

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
                }
            };

            // Create your application here
            _albumName = Intent.GetStringExtra("AlbumName") ?? "";
            if (_albumName != "")
            {
                this.Title = _albumName;
                var musicData = OpenFileInput(MusicDataFileName);
                var json = HelperMethods.ReadJsonFromInternalStorage(musicData);
                _album = ApplicationData.Albums.SingleOrDefault(a => a.Name == _albumName);
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
                button.Click += delegate
                {
                    //Song song = _album.Songs.SingleOrDefault(s => s.Title == button.Text);
                    Song song = _album.Songs[button.Id];

                    if (song != null)
                    {
                        _currentSong = song;
                        //_player.Stop();
                        Player.Reset();
                        var uri = Uri.Parse(song.SongPath);
                        Player.SetAudioStreamType(Stream.Music);
                        Player.SetDataSource(ApplicationContext, uri);
                        Player.Prepare();
                        Player.Start();
                    }
                };

                layout.AddView(button);
            }
        }
    }
}