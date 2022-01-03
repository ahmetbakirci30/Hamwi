using Android.Content;
using Android.Gms.Ads;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using HamwiVideoStatuses.Activities;
using HamwiVideoStatuses.Adapters;
using HamwiVideoStatuses.EventArgs;
using HamwiVideoStatuses.Helpers;
using Hamwi.Shared.Entities.Statuses;
using Hamwi.Shared.Filters.Entity.Statuses;
using Hamwi.Shared.Services.Client;
using Hamwi.Shared.Services.Client.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace HamwiVideoStatuses.Fragments
{
    public class VideosFragment : Fragment
    {
        #region Fields

        private readonly Guid? _categoryId;
        private View _view;
        private IHamwiServices _Hamwi;
        private IEnumerable<Video> _videos;
        private int _pageNumber;
        private InterstitialAd _interstitialAd;

        #endregion

        public VideosFragment(Guid? categoryId)
            => _categoryId = categoryId;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.fragment_videos, container, false);
            Init();
            return _view;
        }

        private async void Init()
        {
            LoadAds();
            _Hamwi = new HamwiServices();
            await LoadVideos();

            var adapter = new VideosAdapter(_videos ??= new List<Video>());
            adapter.ItemClick += Adapter_ItemClick;
            adapter.ItemLongClick += Adapter_ItemLongClick;

            var linearLayoutManager = new LinearLayoutManager(_view.Context);
            var onScrollListener = new EndlessRecyclerOnScrollListener(linearLayoutManager);
            onScrollListener.LoadMoreEvent += async (s, e) =>
            {
                await LoadVideos();
                adapter.Videos = _videos;
                adapter.NotifyDataSetChanged();
            };

            var recycler = _view.FindViewById<RecyclerView>(Resource.Id.recycler);
            recycler.AddOnScrollListener(onScrollListener);
            recycler.SetLayoutManager(linearLayoutManager);
            recycler.SetAdapter(adapter);
        }

        private void LoadAds()
        {
            _interstitialAd = new InterstitialAd(_view.Context)
            {
                AdUnitId = GetString(Resource.String.interstitial_ad_unit_id)
            };

            _interstitialAd.LoadAd(new AdRequest.Builder().Build());
            _view.FindViewById<AdView>(Resource.Id.banner_ad_view).LoadAd(new AdRequest.Builder().Build());
        }

        private async Task LoadVideos()
        {
            var loading = _view.FindViewById<ProgressBar>(Resource.Id.loading);

            try
            {
                loading.Visibility = ViewStates.Visible;

                var videos = await _Hamwi.VideoService.GetAsync(new VideoFilter
                {
                    Shuffle = true,
                    PageNumber = ++_pageNumber,
                    CategoryId = _categoryId
                });

                if (videos != null && videos.Any())
                    _videos = ((_videos != null) && _videos.Any()) ? _videos.Concat(videos) : _videos = videos;
            }
            catch
            {
                Toast.MakeText(_view.Context, Resource.String.connection_error, ToastLength.Short).Show();
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
                    var intent = new Intent(_view.Context, typeof(VideoViewerActivity));
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
                        Toast.MakeText(_view.Context, GetString(Resource.String.remove_from_favorites), ToastLength.Short).Show();
                    }
                    else
                    {
                        Preferences.Set(key, id, id);
                        Toast.MakeText(_view.Context, GetString(Resource.String.added_to_favorites), ToastLength.Short).Show();
                    }
                }
            }
        }
    }
}