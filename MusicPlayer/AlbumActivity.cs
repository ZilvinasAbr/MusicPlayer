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
        private List<Album> _albums;
        private Album _album;
        private Song _currentSong;

        private static MediaPlayer _player = new MediaPlayer();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Album);

            var startPause = FindViewById<Button>(Resource.Id.startpause);
            var nextSong = FindViewById<Button>(Resource.Id.nextSong);
            var previousSong = FindViewById<Button>(Resource.Id.previousSong);

            startPause.Click += delegate
            {
                if (_player.IsPlaying)
                {
                    _player.Pause();
                }
                else
                {
                    _player.Start();
                }
            };
            nextSong.Click += delegate
            {
                var nextSongIndex = _album.Songs.IndexOf(_currentSong) + 1;

                if (nextSongIndex >= _album.Songs.Count)
                {
                    _player.Reset();
                    _currentSong = null;
                }
                else
                {
                    _currentSong = _album.Songs[nextSongIndex];
                    _player.Reset();
                    var uri = Uri.Parse(_currentSong.SongPath);
                    _player.SetAudioStreamType(Stream.Music);
                    _player.SetDataSource(ApplicationContext, uri);
                    _player.Prepare();
                    _player.Start();
                }
            };
            previousSong.Click += delegate
            {
                var nextSongIndex = _album.Songs.IndexOf(_currentSong) - 1;

                if (nextSongIndex < 0)
                {
                    _player.Reset();
                    _currentSong = null;
                }
                else
                {
                    _currentSong = _album.Songs[nextSongIndex];
                    _player.Reset();
                    var uri = Uri.Parse(_currentSong.SongPath);
                    _player.SetAudioStreamType(Stream.Music);
                    _player.SetDataSource(ApplicationContext, uri);
                    _player.Prepare();
                    _player.Start();
                }
            };

            // Create your application here
            _albumName = Intent.GetStringExtra("AlbumName") ?? "";
            if (_albumName != "")
            {
                var musicData = OpenFileInput(MusicDataFileName);
                var json = HelperMethods.ReadJsonFromInternalStorage(musicData);
                _albums = JsonConvert.DeserializeObject<List<Album>>(json);
                _album = _albums.SingleOrDefault(a => a.Name == _albumName);
            }

            if (_album == null)
                return;

            var layout = FindViewById<LinearLayout>(Resource.Id.linearSongsLayout);

            for (int i = 0; i < _album.Songs.Count; i++)
            {
                var button = new Button(this)
                {
                    Text = _album.Songs[i].Title,
                    Id = i
                };
                button.Click += delegate
                {
                    Song song = _album.Songs.SingleOrDefault(s => s.Title == button.Text);

                    if (song != null)
                    {
                        _currentSong = song;
                        //_player.Stop();
                        _player.Reset();
                        var uri = Uri.Parse(song.SongPath);
                        _player.SetAudioStreamType(Stream.Music);
                        _player.SetDataSource(ApplicationContext, uri);
                        _player.Prepare();
                        _player.Start();
                    }
                };

                layout.AddView(button);
            }
        }
    }
}