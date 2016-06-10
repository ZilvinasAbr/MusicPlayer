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
        private static MediaPlayer _player = new MediaPlayer();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Album);

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
                var button = new Button(this);
                button.Text = _album.Songs[i].Title;
                button.Id = i;
                button.Click += delegate
                {
                    Song song = _album.Songs.SingleOrDefault(s => s.Title == button.Text);

                    if (song != null)
                    {
                        //_player.Stop();
                        _player.Reset();
                        Uri uri = Uri.Parse(song.SongPath);
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