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

        private const string MusicDataFileName = "musicData.json";
        private List<Album> _albums;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);






            //var musicData = OpenFileOutput(MusicDataFileName, FileCreationMode.Private);

            // If there is no data saved about music files
            if (!FileExists(MusicDataFileName))
            {
                // Generate music data
                _albums = HelperMethods.GenerateMusicData();
                var json = JsonConvert.SerializeObject(_albums);

                var musicData = OpenFileOutput(MusicDataFileName, FileCreationMode.Private);
                HelperMethods.WriteJsonToInternalStorage(musicData, json);
                musicData.Close();
            }
            else
            {
                //Get music data from internal storage
                var musicData = OpenFileInput(MusicDataFileName);
                
                var json = HelperMethods.ReadJsonFromInternalStorage(musicData);
                musicData.Close();
                _albums = JsonConvert.DeserializeObject<List<Album>>(json);
                //_albums = (List<Album>) JsonConvert.DeserializeObject(json);
            }


            var layout = FindViewById<LinearLayout>(Resource.Id.linearAlbumsLayout);

            for (var i = 0; i < _albums.Count; i++)
            {
                var button = new Button(this)
                {
                    Text = _albums[i].Name,
                    Id = i
                };

                layout.AddView(button);
            }   
        }

        public bool FileExists(string fileName)
        {
            var file = BaseContext.GetFileStreamPath(fileName);
            return file.Exists();
        }
    }
}
