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
using MusicPlayer.Models;
using Newtonsoft.Json;
using Stream = System.IO.Stream;
using StringBuilder = System.Text.StringBuilder;

namespace MusicPlayer.Helpers
{
    class Person
    {
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
    }

    public static class HelperMethods
    {
        private static readonly MediaMetadataRetriever Reader = new MediaMetadataRetriever();

        /// <summary>
        /// parentDir should be equal to "/storage"
        /// </summary>
        /// <param name="parentDir"></param>
        /// <returns></returns>
        public static List<string> GetListAlbums(File parentDir)
        {
            List<string> albumsList = new List<string>();

            GetListAlbumsHelper(parentDir, ref albumsList);
            return albumsList;
        }

        private static void GetListAlbumsHelper(File parentDir, ref List<string> albumsList)
        {
            List<string> albums = new List<string>();
            File[] files = parentDir.ListFiles();

            if (files == null)
            {
                return;
            }

            foreach (var file in files)
            {
                if (albumsList.Count >= 10)
                    return;

                if (file.IsDirectory)
                {
                    GetListAlbumsHelper(file, ref albumsList);
                }
                else
                {
                    if (file.Name.EndsWith(".mp3") || file.Name.EndsWith(".wav") || file.Name.EndsWith(".flac"))
                    {
                        Reader.SetDataSource(file.AbsolutePath);
                        string album = Reader.ExtractMetadata(Android.Media.MetadataKey.Album);

                        if (album != null && !albumsList.Contains(album))
                        {
                            albumsList.Add(album);
                        }
                    }
                }
            }
        }


        /*public static void WriteToInternalStorage()
        {
            string fileName = "test";
            string text = "hello world";

            var fos = OpenFileOutput(fileName, FileCreationMode.Private);
            var bytes = GetBytes(text);
            fos.Write(bytes, 0, bytes.Length);
            fos.Close();
        }*/

        /*public static void ReadFromInternalStorage()
        {
            string fileName = "test";
            var fi = OpenFileInput(fileName);

            byte[] bytes = new byte[100];

            fi.Read(bytes, 0, 100);

            string text = GetString(bytes);
            fi.Close();
        }*/

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        /*public static bool SaveDataToInternalStorage()
        {
            string fileName = "musicList";
            string text = "hello world";

            var fos = OpenFileOutput(fileName, FileCreationMode.Private);
            var bytes = GetBytes(text);
            fos.Write(bytes, 0, bytes.Length);
            fos.Close();

            return true;
        }*/

        public static void TestJsonNet()
        {
            var person = new Person { Name = "Bob", Birthday = new DateTime(1987, 2, 2) };
            var json = JsonConvert.SerializeObject(person);
            var person2 = JsonConvert.DeserializeObject<Person>(json);
        }

        public static void WriteJsonToInternalStorage(System.IO.Stream fileStream, string json)
        {
            var jsonBytes = GetBytes(json);
            fileStream.Write(jsonBytes, 0, jsonBytes.Length);
        }

        public static string ReadJsonFromInternalStorage(System.IO.Stream fileStream)
        {
            const long bufferLength = 1024*1024*20;

            byte[] jsonBytes = new byte[bufferLength];

            fileStream.Read(jsonBytes, 0, jsonBytes.Length);

            var json = GetString(jsonBytes);

            return json;

        }

        /// <summary>
        /// Generates Album list
        /// </summary>
        /// <returns></returns>
        public static List<Album> GenerateMusicData()
        {
            List<Album> albums = new List<Album>();

            GenerateMusicDataHelper(new File("/storage/external_SD/Muzika"), albums);
            GenerateMusicDataHelper(new File("/storage/emulated/0/Music"), albums);


            return albums;
        }

        private static void GenerateMusicDataHelper(File parentDir, IList<Album> albums)
        {
            File[] files = parentDir.ListFiles();

            if (files == null)
            {
                return;
            }

            foreach (var file in files)
            {
                if (file.IsDirectory)
                {
                    GenerateMusicDataHelper(file, albums);
                }
                else
                {
                    if (file.Name.EndsWith(".mp3") || file.Name.EndsWith(".wav") || file.Name.EndsWith(".flac"))
                    {
                        Reader.SetDataSource(file.AbsolutePath);

                        Song newSong = new Song()
                        {
                            Album = Reader.ExtractMetadata(MetadataKey.Album),
                            AlbumArtist = Reader.ExtractMetadata(MetadataKey.Albumartist),
                            Artist = Reader.ExtractMetadata(MetadataKey.Artist),
                            Genre = Reader.ExtractMetadata(MetadataKey.Genre),
                            Year = Reader.ExtractMetadata(MetadataKey.Year),
                            SongPath = file.AbsolutePath,
                            Title = Reader.ExtractMetadata(MetadataKey.Title),
                            Track = Reader.ExtractMetadata(MetadataKey.CdTrackNumber)
                        };

                        string albumName = newSong.Album;

                        if (albumName == null)
                        {
                            albumName = "None";
                        }

                        var album = albums.SingleOrDefault(a => a.Name != null && a.Name.Equals(albumName));
                        if (album == null)
                        {
                            album = new Album()
                            {
                                Name = albumName,
                                Songs = new List<Song>()
                            };
                            albums.Add(album);
                        }

                        album.Songs.Add(newSong);
                    }
                }
            }
        }
    }
}