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
using MusicPlayer.Models;

namespace MusicPlayer
{
    public static class ApplicationData
    {
        public const string MusicDataFileName = "musicData.json";

        public static List<Album> Albums { get; set; }
    }
}