using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MusicPlayer2.Models
{
    class Song
    {
        public string SongPath { get; set; }
        public string Title { get; set; }
        public string Artist { get;set; }
        public string Album { get; set; }
        public string Year { get; set; }
        public string Track { get; set; }
        public string Genre { get; set; }
        public string Comment { get; set; }
        public string AlbumArtist { get; set; }
    }
}