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
using Java.IO;

namespace MusicPlayer.Models
{
    public class Album : IComparable<Album>
    {
        public string Name { get; set; }
        public IList<Song> Songs { get; set; }

        public int CompareTo(Album other)
        {
            return string.Compare(this.Name, other.Name, StringComparison.Ordinal);
        }
    }
}