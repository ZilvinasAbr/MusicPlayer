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
using StringBuilder = System.Text.StringBuilder;

namespace MusicPlayer2
{
    [Activity(Label = "MusicPlayer2", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private MediaMetadataRetriever _reader = new MediaMetadataRetriever();
        private List<string> _albumsList = new List<string>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get album list
            GetListAlbums(new File("/storage"));
            _albumsList.Sort();

            LinearLayout layout = FindViewById<LinearLayout>(Resource.Id.linearLayout1);

            for (int i = 0; i < _albumsList.Count; i++)
            {
                Button button = new Button(this);
                button.Text = _albumsList[i];
                button.Id = i;

                layout.AddView(button);

                Button button2 = new Button(this);
                button2.Text = _albumsList[i];
                button2.Id = i+100;

                layout.AddView(button2);

                Button button3 = new Button(this);
                button3.Text = _albumsList[i];
                button3.Id = i+200;

                layout.AddView(button3);
            }   
        }

        private void GetListAlbums(File parentDir)
        {
            List<string> albums = new List<string>();
            File[] files = parentDir.ListFiles();

            if (files == null)
            {
                return;
            }

            foreach (var file in files)
            {
                if (_albumsList.Count >= 10)
                    return;

                if (file.IsDirectory)
                {
                    GetListAlbums(file);
                }
                else
                {
                    if (file.Name.EndsWith(".mp3") || file.Name.EndsWith(".wav") || file.Name.EndsWith(".flac"))
                    {
                        _reader.SetDataSource(file.AbsolutePath);
                        string album = _reader.ExtractMetadata(Android.Media.MetadataKey.Album);

                        if (album != null && !_albumsList.Contains(album))
                        {
                            _albumsList.Add(album);
                        }
                    }
                }
            }
        }

        private List<File> GetListFiles(File parentDir)
        {
            List<File> inFiles = new List<File>();
            File[] files = parentDir.ListFiles();

            if (files == null)
            {
                return null;
            }

            foreach (var file in files)
            {
                if (file.IsDirectory)
                {
                    var filesList = GetListFiles(file);
                    if (filesList != null)
                    {
                        inFiles.AddRange(filesList);
                    }
                }
                else
                {
                    if (file.Name.EndsWith(".mp3") || file.Name.EndsWith(".wav") || file.Name.EndsWith(".flac"))
                    {
                        _reader.SetDataSource(file.AbsolutePath);
                        string title = _reader.ExtractMetadata(Android.Media.MetadataKey.Artist);
                        string album = _reader.ExtractMetadata(Android.Media.MetadataKey.Album);
                        inFiles.Add(file);
                    }
                }
            }
            return inFiles;
        }
    }
}

