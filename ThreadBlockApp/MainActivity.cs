using Android.App;
using Android.Widget;
using Android.OS;
using System;
using System.Net.Http;
using Android.Graphics;
using System.Threading.Tasks;
using Android.Util;
using System.Collections.Generic;
using Android.Views;

namespace ThreadBlockApp
{
    [Activity(Label = "ThreadBlockApp", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        int ImagesToLoad = 5; // Set this to 1 to observe the difference in time for the first picture loaded
        long startTime;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            LoadImages();
        }

        private async void LoadImages()
        {
            startTime = System.Environment.TickCount;
            string[] urls = {
                "https://i.imgur.com/4AiXzf8.jpg",
                "https://i.imgur.com/wYTCtRu.jpg",
                "https://i.imgur.com/2oTdDKF.jpg",
                "https://i.imgur.com/4AiXzf8.jpg",
                "https://i.imgur.com/wYTCtRu.jpg"
            };
            var list = new List<Task<Bitmap>>();
            for (int i = 0; i < ImagesToLoad; i++)
            {
                Log.Info("BLOK", "Starting load of " + i);
                Task<Bitmap> task = loadAndDecodeBitmap(urls[i], i);

                Log.Info("BLOK", "Load started " + i);
                list.Add(task);
            }
            await Task.Run(() => Task.WhenAll<Bitmap>(list));
        }
 
        private static HttpClient Client = new HttpClient();
        public async Task<Bitmap> loadAndDecodeBitmap(String uri, int i)
        {
            try
            {
                byte[] data = await Client.GetByteArrayAsync(uri);

                Log.Info("BLOK", "Load completed " + i);
                Bitmap img = BitmapFactory.DecodeByteArray(data, 0, data.Length);
                TryShowBitmap(img, i);
                if (i == 0) ShowTime();
                return img;
            }
            catch (Exception ex)
            {
                Log.Error("BLOK", String.Format("Failed loading image %s: %s", uri, ex.Message));
                throw ex;
            }
        }

        private void ShowTime()
        {
            FindViewById<TextView>(Resource.Id.timestamp).SetText((System.Environment.TickCount - startTime) + "ms", TextView.BufferType.Normal);
        }

        /// <summary>
        ///  Shows the bitmap in the n'th View component in imageListContainer
        /// </summary> 
        private void TryShowBitmap(Bitmap img, int n)
        {
            LinearLayout container = FindViewById<LinearLayout>(Resource.Id.imageListContainer);
            View view = container.GetChildAt(n + 1); // Skip the textview
            if (view is ImageView)
            {
                ((ImageView)view).SetImageBitmap(img);
            }
        }
    }
}

