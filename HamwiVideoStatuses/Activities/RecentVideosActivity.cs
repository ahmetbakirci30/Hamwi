using Android.App;
using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using HamwiVideoStatuses.Adapters;
using HamwiVideoStatuses.EventArgs;
using Hamwi.Shared.Entities.Statuses;
using Hamwi.Shared.Services.Client;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace HamwiVideoStatuses.Activities
{
    [Activity(Label = "@string/recent_videos", Theme = "@style/AppTheme")]
    public class RecentVideosActivity : AppCompatActivity
    {
        #region Fields

        private IEnumerable<Video> _videos;
        private InterstitialAd _interstitialAd;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.list_layout);

            Init();
        }

        private async void Init()
        {
            LoadAds();
            await LoadVideos();

            var adapter = new VideosAdapter(_videos ??= new List<Video>());
            adapter.ItemClick += Adapter_ItemClick;
            adapter.ItemLongClick += Adapter_ItemLongClick;

            var recycler = FindViewById<RecyclerView>(Resource.Id.recycler);
            recycler.SetLayoutManager(new LinearLayoutManager(this));
            recycler.SetAdapter(adapter);
        }

        private void LoadAds()
        {
            _interstitialAd = new InterstitialAd(this)
            {
                AdUnitId = GetString(Resource.String.interstitial_ad_unit_id)
            };

            _interstitialAd.LoadAd(new AdRequest.Builder().Build());
            FindViewById<AdView>(Resource.Id.banner_ad_view).LoadAd(new AdRequest.Builder().Build());
        }

        private async Task LoadVideos()
        {
            var loading = FindViewById<ProgressBar>(Resource.Id.loading);

            try
            {
                loading.Visibility = ViewStates.Visible;

                var videos = await new HamwiServices().VideoService.GetAsync();

                if (videos != null && videos.Any())
                    _videos = videos.Reverse().Take(100);
            }
            catch
            {
                Toast.MakeText(this, GetString(Resource.String.connection_error), ToastLength.Short).Show();
            }
            finally
            {
                loading.Visibility = ViewStates.Gone;
            }
        }

        private void Adapter_ItemClick(object sender, RecyclerViewAdapterClickEventArgs e)
        {
            if (_videos != null && _videos.Any())
            {
                var video = _videos.ElementAtOrDefault(e.Position);

                if (video != null)
                {
                    var intent = new Intent(this, typeof(VideoViewerActivity));
                    intent.PutExtra("video", JsonConvert.SerializeObject(video));
                    StartActivity(intent);

                    if (_interstitialAd.IsLoaded)
                        _interstitialAd.Show();

                    LoadAds();
                }
            }
        }

        private void Adapter_ItemLongClick(object sender, RecyclerViewAdapterClickEventArgs e)
        {
            if (_videos != null && _videos.Any())
            {
                var video = _videos.ElementAtOrDefault(e.Position);

                if (video != null)
                {
                    var id = video.Id.ToString();
                    var key = GetString(Resource.String.videos);

                    if (Preferences.ContainsKey(key, id))
                    {
                        Preferences.Remove(key, id);
                        Toast.MakeText(this, GetString(Resource.String.remove_from_favorites), ToastLength.Short).Show();
                    }
                    else
                    {
                        Preferences.Set(key, id, id);
                        Toast.MakeText(this, GetString(Resource.String.added_to_favorites), ToastLength.Short).Show();
                    }
                }
            }
        }
    }
}