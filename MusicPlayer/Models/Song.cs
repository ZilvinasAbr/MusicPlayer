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

namespace MusicPlayer.Models
{
    public class Song : IComparable<Song>
    {
        public string TrackNumber { get; set; }
        public string SongPath { get; set; }
        public string Title { get; set; }
        public string Artist { get;set; }
        public string Album { get; set; }
        public string Year { get; set; }
        public string Genre { get; set; }
        public string AlbumArtist { get; set; }


        public int CompareTo(Song other)
        {
            int songNumber;
            int otherSongNumber = 0;

            var success = int.TryParse(TrackNumber, out songNumber) &&
                           int.TryParse(other.TrackNumber, out otherSongNumber);

            if (success)
            {
                return songNumber.CompareTo(otherSongNumber);
            }

            return String.Compare(TrackNumber, other.TrackNumber, StringComparison.Ordinal);
        }
    }
}