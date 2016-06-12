using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Provider;
using Java.IO;
using MusicPlayer.Helpers;
using MusicPlayer.Models;
using Newtonsoft.Json;
using StringBuilder = System.Text.StringBuilder;

namespace MusicPlayer
{
    [Activity(Label = "Music Player", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Loads music data to ApplicationData.Albums
            LoadMusicData();

            // Adds album buttons to the layout
            AddAlbumButtons();
            
        }

        #region Private methods

        private void AddAlbumButtons()
        {
            var layout = FindViewById<LinearLayout>(Resource.Id.linearAlbumsLayout);

            for (var i = 0; i < ApplicationData.Albums.Count; i++)
            {
                var button = new Button(this)
                {
                    Text = ApplicationData.Albums[i].Name,
                    Id = i
                };
                button.Click += delegate
                {
                    AlbumButtonPressed(button);
                };

                layout.AddView(button);
            }
        }

        private void LoadMusicData()
        {
            // If there is no data saved about music files
            if (!FileExists(ApplicationData.MusicDataFileName))
            {
                // Generate music data
                ApplicationData.Albums = HelperMethods.GenerateMusicData();
                var json = JsonConvert.SerializeObject(ApplicationData.Albums);

                var musicData = OpenFileOutput(ApplicationData.MusicDataFileName, FileCreationMode.Private);
                HelperMethods.WriteJsonToInternalStorage(musicData, json);
                musicData.Close();
            }
            else
            {
                //Get music data from internal storage
                var musicData = OpenFileInput(ApplicationData.MusicDataFileName);

                var json = HelperMethods.ReadJsonFromInternalStorage(musicData);
                musicData.Close();
                ApplicationData.Albums = JsonConvert.DeserializeObject<List<Album>>(json);
            }

            ApplicationData.Albums.Sort();

            foreach (var album in ApplicationData.Albums)
            {
                ((List<Song>)album.Songs).Sort();
            }
        }

        private bool FileExists(string fileName)
        {
            var file = BaseContext.GetFileStreamPath(fileName);
            return file.Exists();
        }

        private void AlbumButtonPressed(Button button)
        {
            var albumActivity = new Intent(this, typeof(AlbumActivity));
            albumActivity.PutExtra("AlbumName", button.Text);
            StartActivity(albumActivity);
        }

        #endregion
    }
}
