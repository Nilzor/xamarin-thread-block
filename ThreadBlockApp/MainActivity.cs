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
            //Task.Factory.StartNew(() => LoadImages());

            Task.Run(() => LoadImages());
            Log.Debug("BLOK", "onCreate done");
        }

        private async void LoadImages()
        {
            Log.Info("BLOK", "Load images initiating");
            startTime = System.Environment.TickCount;
            string[] urls = {
                "https://i.imgur.com/4AiXzf8.jpg",
                "https://i.imgur.com/wYTCtRu.jpg",
                "https://i.imgur.com/2oTdDKF.jpg",
                "https://i.imgur.com/4AiXzf8.jpg",
                "https://i.imgur.com/4AiXzf8.jpg",
                "https://i.imgur.com/wYTCtRu.jpg",
                "https://i.imgur.com/2oTdDKF.jpg",
                "https://i.imgur.com/4AiXzf8.jpg",
                "https://i.imgur.com/4AiXzf8.jpg",
                "https://i.imgur.com/wYTCtRu.jpg"
            };
            var list = new List<Task>();
            for (int i = 0; i < ImagesToLoad; i++)
            {
                Task task = LoadAndShowImage(urls[i], i);
                list.Add(task);
            }
            await Task.WhenAll(list);
        }

        public async Task LoadAndShowImage(String uri, int i)
        {
            var bitmap = await Task.Run(() => LoadAndDecodeBitmap(uri, i));
            Log.Info("BLOK", "Queueing SHOW BITMAP " + i);
            RunOnUiThread(() => {
                Log.Info("BLOK", "Executing SHOW BITMAP " + i);
                TryShowBitmap(bitmap, i);
                if (i == 0) ShowTime();
            });
        }
 
        private static HttpClient Client = new HttpClient();
        public async Task<Bitmap> LoadAndDecodeBitmap(String uri, int i)
        {
            try
            {
                Log.Info("BLOK", "Starting load of " + i);
                byte[] data = await Client.GetByteArrayAsync(uri);

                Log.Info("BLOK", "Load completed " + i);
                Bitmap img = BitmapFactory.DecodeByteArray(data, 0, data.Length);
                return img;
            }
            catch (Exception ex)
            {
                Log.Error("BLOK", String.Format("Failed loading image {0}: {1}", uri, ex.Message));
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

