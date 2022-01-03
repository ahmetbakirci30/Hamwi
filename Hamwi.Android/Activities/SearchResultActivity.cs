using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Hamwi.Android.Fragments;
using Hamwi.Shared.Entities.Statuses;
using Hamwi.Shared.Filters.Entity.Statuses;
using Hamwi.Shared.Services.Client;
using System.Collections.Generic;

namespace Hamwi.Android.Activities
{
    [Activity(Theme = "@style/AppTheme")]
    public class SearchResultActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.videos_layout);

            Init();
        }

        private async void Init()
        {
            SupportFragmentManager.BeginTransaction().Replace(Resource.Id.videos_layout,
                new ShowingVideosFragment((await new HamwiServices().VideoService.GetAsync(new VideoFilter
                {
                    Title = Intent.GetStringExtra("term"),
                    PageSize = int.MaxValue,
                    Shuffle = true
                })) ?? new List<Video>())).Commit();

            FindViewById<ProgressBar>(Resource.Id.videos_load_bar).Visibility = ViewStates.Gone;
        }
    }
}