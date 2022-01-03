using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.CardView.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Java.Net;
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Environment = Android.OS.Environment;
using Path = System.IO.Path;
using Uri = Android.Net.Uri;

namespace Hamwi.Android.Activities
{
    [Activity(Theme = "@style/AppTheme")]
    public class VideoViewerActivity : AppCompatActivity
    {
        #region Variables
        private readonly IHamwiServices _Hamwi;
        private Video _video;
        private IEnumerable<Video> _videos;
        private int _pageNumber = 1;
        private InterstitialAd _interstitialAd;
        #endregion

        public VideoViewerActivity()
            => _Hamwi = new HamwiServices();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_video_viewer);

            Init();
        }

        private async void Init()
        {
            LoadAds();

            _video = JsonConvert.DeserializeObject<Video>(Intent.GetStringExtra("video"));

            var videoTitle = FindViewById<TextView>(Resource.Id.video_title);
            var videoCoverImage = FindViewById<ImageView>(Resource.Id.video_cover_image);
            var videoPlayer = FindViewById<VideoView>(Resource.Id.video);
            var likeVideo = FindViewById<ImageView>(Resource.Id.like);
            var downlaodVideo = FindViewById<ImageView>(Resource.Id.download);
            var shareVideo = FindViewById<ImageView>(Resource.Id.share);
            var whatsappShareVideo = FindViewById<ImageView>(Resource.Id.whatsapp_share);

            if (Preferences.ContainsKey(GetString(Resource.String.action_videos), _video.Id.ToString()))
                likeVideo.SetColorFilter(Color.Red);

            videoTitle.Text = _video.Title;
            Glide.With(this).Load(_video.CoverPath).Into(videoCoverImage);
            videoPlayer.SetVideoPath(_video.VideoPath);
            videoPlayer.SetMediaController(new MediaController(this));
            videoPlayer.Prepared += VideoPlayer_Prepared;
            likeVideo.Click += LikeVideo_Click;
            downlaodVideo.Click += DownlaodVideo_Click;
            shareVideo.Click += ShareVideo_Click;
            whatsappShareVideo.Click += WhatsappShareVideo_Click;

            await LoadVideos();

            var adapter = new VideosAdapter(_videos);
            adapter.ItemClick += Adapter_ItemClick;
            adapter.ItemLongClick += Adapter_LikeItemClick;
            adapter.LikeItemClick += Adapter_LikeItemClick;

            var linearLayoutManager = new LinearLayoutManager(this);
            var onScrollListener = new EndlessRecyclerOnScrollListener(linearLayoutManager);
            onScrollListener.LoadMoreEvent += async (s, e) =>
            {
                await LoadVideos();
                adapter.Videos = _videos;
                adapter.NotifyDataSetChanged();
            };

            var recycler = FindViewById<RecyclerView>(Resource.Id.recycler);
            recycler.AddOnScrollListener(onScrollListener);
            recycler.SetLayoutManager(linearLayoutManager);
            recycler.SetAdapter(adapter);
        }

        private void VideoPlayer_Prepared(object sender, EventArgs e)
        {
            FindViewById<ProgressBar>(Resource.Id.load_bar).Visibility = ViewStates.Gone;
            (sender as MediaPlayer).Start();
        }

        private async void LikeVideo_Click(object sender, EventArgs e)
        {
            if (_video != null)
            {
                var likeIcon = FindViewById<ImageView>(Resource.Id.like);
                var key = GetString(Resource.String.action_videos);
                var id = _video.Id.ToString();

                if (Preferences.ContainsKey(key, id))
                {
                    _video.LikesCount--;
                    likeIcon.ClearColorFilter();
                    Preferences.Remove(key, id);
                }
                else
                {
                    _video.LikesCount++;
                    likeIcon.SetColorFilter(Color.Red);
                    Preferences.Set(key, id, id);
                }

                try { await _Hamwi.VideoService.UpdateAsync(_video); } catch { return; }
            }
        }

        private async void DownlaodVideo_Click(object sender, EventArgs e)
            => await InstallVideo();

        private void ShareVideo_Click(object sender, EventArgs e)
            => ShareVideo();

        private void WhatsappShareVideo_Click(object sender, EventArgs e)
        {
            var whatsapp = GetString(Resource.String.whatsapp_package_name);
            var whatsappBusiness = GetString(Resource.String.whatsapp_business_package_name);

            if (IsPackageInstalled(whatsapp)) ShareVideo(whatsapp);
            else if (IsPackageInstalled(whatsappBusiness)) ShareVideo(whatsappBusiness);
            else Toast.MakeText(this, GetString(Resource.String.whatsapp_not_found), ToastLength.Short).Show();
        }

        private async Task<string> InstallVideo()
        {
            if (_video == null) return string.Empty;

            var permission = Manifest.Permission.WriteExternalStorage;

            if (((int)Build.VERSION.SdkInt) >= 23)
            {
                if (CheckSelfPermission(permission) != Permission.Granted)
                {
                    new AndroidX.AppCompat.App.AlertDialog.Builder(this)
                        .SetTitle(GetString(Resource.String.access_permission))
                        .SetMessage(GetString(Resource.String.access))
                        .SetPositiveButton(GetString(Resource.String.confirm), (s, e) => { RequestPermissions(new string[] { permission }, 0249835816); })
                        .SetNegativeButton(GetString(Resource.String.cancel), (s, e) => { })
                        .Show();

                    return string.Empty;
                }
            }

            string applicationPath = Path.Combine(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads).AbsolutePath, GetString(Resource.String.app_name));

            if (!Directory.Exists(applicationPath)) Directory.CreateDirectory(applicationPath);

            var downloadPath = Path.Combine(applicationPath, $"{_video.Title}-{_video.Id}.mp4");

            if (!File.Exists(downloadPath))
            {
                var loadingLayout = FindViewById<CardView>(Resource.Id.laoding_layout);

                try
                {
                    loadingLayout.Visibility = ViewStates.Visible;
                    await File.WriteAllBytesAsync(downloadPath, await _Hamwi.FileService.DownloadAsync(_video.VideoPath));
                    SendBroadcast(new Intent(Intent.ActionMediaScannerScanFile).SetData(Uri.FromFile(new Java.IO.File(downloadPath))));
                    Toast.MakeText(this, GetString(Resource.String.downloading_successfully), ToastLength.Short).Show();

                    _video.DownloadsCount++;
                    await _Hamwi.VideoService.UpdateAsync(_video);
                    loadingLayout.Visibility = ViewStates.Gone;
                    return downloadPath;
                }
                catch
                {
                    loadingLayout.Visibility = ViewStates.Gone;
                    Toast.MakeText(this, GetString(Resource.String.downloading_error), ToastLength.Short).Show();
                    return downloadPath;
                }
            }

            Toast.MakeText(this, GetString(Resource.String.already_exists), ToastLength.Short).Show();
            return downloadPath;
        }

        private async void ShareVideo(string packageName = null)
        {
            var path = await InstallVideo();

            if ((!string.IsNullOrWhiteSpace(path)) && File.Exists(path))
            {
                var intentShareFile = new Intent(Intent.ActionSend);
                intentShareFile.SetType(URLConnection.GuessContentTypeFromName(path));
                intentShareFile.PutExtra(Intent.ExtraStream, Uri.Parse(path));
                intentShareFile.PutExtra(Intent.ExtraText, GetString(Resource.String.download_our_app) + PackageName);
                intentShareFile.PutExtra(Intent.ExtraSubject, GetString(Resource.String.app_name));

                if (!string.IsNullOrWhiteSpace(packageName))
                    intentShareFile.SetPackage(packageName);

                StartActivity(Intent.CreateChooser(intentShareFile, GetString(Resource.String.share)));

                if (_video != null)
                {
                    _video.SharesCount++;
                    try { await _Hamwi.VideoService.UpdateAsync(_video); } catch { return; }
                }
            }
        }

        private bool IsPackageInstalled(string packageName)
        {
            try
            {
                PackageManager.GetPackageInfo(packageName, 0);
                return true;
            }
            catch { return false; }
        }

        private async Task LoadVideos()
        {
            var loading = FindViewById<ProgressBar>(Resource.Id.loading);
            loading.Visibility = ViewStates.Visible;

            var videos = (await _Hamwi.VideoService.GetAsync(new VideoFilter { PageNumber = _pageNumber++, Shuffle = true })) ?? new List<Video>();
            _videos = ((_videos != null) && _videos.Any()) ? _videos.Union(videos) : _videos = videos;

            loading.Visibility = ViewStates.Gone;
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

        private async void Adapter_ItemClick(object sender, RecyclerViewAdapterClickEventArgs e)
        {
            if (_videos != null && _videos.Any())
            {
                var video = _videos.ElementAtOrDefault(e.Position);

                if (video != null)
                {
                    var intent = new Intent(this, typeof(VideoViewerActivity));
                    intent.PutExtra("video", JsonConvert.SerializeObject(video));
                    StartActivity(intent);

                    if (_interstitialAd.IsLoaded) _interstitialAd.Show();

                    video.ViewsCount++;
                    try { await _Hamwi.VideoService.UpdateAsync(video); } catch { return; }

                    Finish();
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

                    try { await _Hamwi.VideoService.UpdateAsync(video); } catch { return; }
                }
            }
        }
    }
}