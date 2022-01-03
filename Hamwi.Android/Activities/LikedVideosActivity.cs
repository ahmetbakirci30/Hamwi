using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Hamwi.Android.Fragments;
using Hamwi.Shared.Entities.Statuses;
using Hamwi.Shared.Filters.Entity.Statuses;
using Hamwi.Shared.Services.Client;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace Hamwi.Android.Activities
{
    [Activity(Theme = "@style/AppTheme")]
    public class LikedVideosActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.videos_layout);

            Init();
        }

        private async void Init()
        {
            var videos = (await new HamwiServices().VideoService.GetAsync(new VideoFilter { PageSize = int.MaxValue, Shuffle = true })) ?? new List<Video>();
            var loved = new List<Video>();

            foreach (var video in videos)
                if (Preferences.ContainsKey(GetString(Resource.String.action_videos), video.Id.ToString()))
                    loved.Add(video);

            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.videos_layout, new ShowingVideosFragment(loved)).Commit();

            FindViewById<ProgressBar>(Resource.Id.videos_load_bar).Visibility = ViewStates.Gone;
        }
    }
}