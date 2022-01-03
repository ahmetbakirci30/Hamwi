using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Hamwi.Android.Fragments;
using Hamwi.Shared.Entities.Statuses;
using Hamwi.Shared.Services.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hamwi.Android.Activities
{
    [Activity(Theme = "@style/AppTheme")]
    public class RecentlyAddedVideosActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.videos_layout);

            Init();
        }

        private async void Init()
        {
            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.videos_layout, new ShowingVideosFragment(
                ((await new HamwiServices().VideoService.GetAsync()) ?? new List<Video>()).Reverse().Take(75))).Commit();

            FindViewById<ProgressBar>(Resource.Id.videos_load_bar).Visibility = ViewStates.Gone;
        }
    }
}