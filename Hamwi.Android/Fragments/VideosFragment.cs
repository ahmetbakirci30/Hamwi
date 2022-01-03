using Android.Content;
using Android.Gms.Ads;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using Hamwi.Android.Activities;
using Hamwi.Android.Adapters;
using Hamwi.Android.EventArguments;
using Hamwi.Android.Helpers;
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

namespace Hamwi.Android.Fragments
{
    public class VideosFragment : Fragment
    {
        #region Variables
        private readonly IHamwiServices _Hamwi;
        private readonly Guid _categoryId;
        private View _view;
        private IEnumerable<Video> _videos;
        private int _pageNumber = 1;
        private InterstitialAd _interstitialAd;
        #endregion

        public VideosFragment(Guid categoryId)
        {
            _Hamwi = new HamwiServices();
            _categoryId = categoryId;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.fragment_videos, container, false);
            Init();
            return _view;
        }

        private async void Init()
        {
            LoadAds();

            await LoadVideos();

            var adapter = new VideosAdapter(_videos);
            adapter.ItemClick += Adapter_ItemClick;
            adapter.ItemLongClick += Adapter_LikeItemClick;
            adapter.LikeItemClick += Adapter_LikeItemClick;

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
            loading.Visibility = ViewStates.Visible;

            var videos = (await _Hamwi.VideoService.GetAsync(new VideoFilter { CategoryId = _categoryId, PageNumber = _pageNumber++, Shuffle = true })) ?? new List<Video>();
            _videos = ((_videos != null) && _videos.Any()) ? _videos.Union(videos) : _videos = videos;

            loading.Visibility = ViewStates.Gone;
        }

        private async void Adapter_ItemClick(object sender, RecyclerViewAdapterClickEventArgs e)
        {
            if (_videos != null && _videos.Any())
            {
                var video = _videos.ElementAtOrDefault(e.Position);

                if (video != null)
                {
                    var intent = new Intent(Context, typeof(VideoViewerActivity));
                    intent.PutExtra("video", JsonConvert.SerializeObject(video));
                    StartActivity(intent);

                    if (_interstitialAd.IsLoaded) _interstitialAd.Show();

                    video.ViewsCount++;
                    await _Hamwi.VideoService.UpdateAsync(video);

                    LoadAds();
                }
            }
        }

        private async void Adapter_LikeItemClick(object sender, RecyclerViewAdapterClickEventArgs e)
        {
            if (_videos != null && _videos.Any())
            {
                var video = _videos.ElementAtOrDefault(e.Position);

                if (video != null)
                {
                    var id = video.Id.ToString();
                    var key = GetString(Resource.String.action_videos);
                    var likeBtn = e.View.FindViewById<ImageView>(Resource.Id.iv_like);
                    var likeCount = e.View.FindViewById<TextView>(Resource.Id.tv_likes_count);

                    if (Preferences.ContainsKey(key, id))
                    {
                        likeBtn.ClearColorFilter();
                        likeCount.Text = (--video.LikesCount).ToString();
                        Preferences.Remove(key, id);
                    }
                    else
                    {
                        likeBtn.SetColorFilter(Color.Red);
                        likeCount.Text = (++video.LikesCount).ToString();
                        Preferences.Set(key, id, id);
                    }

                    await _Hamwi.VideoService.UpdateAsync(video);
                }
            }
        }
    }
}