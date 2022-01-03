using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Media;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.CardView.Widget;
using AndroidX.Core.App;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using HamwiVideoStatuses.Adapters;
using HamwiVideoStatuses.EventArgs;
using HamwiVideoStatuses.Helpers;
using Java.Net;
using Hamwi.Shared.Entities.Statuses;
using Hamwi.Shared.Filters.Entity.Statuses;
using Hamwi.Shared.Services.Client;
using Hamwi.Shared.Services.Client.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace HamwiVideoStatuses.Activities
{
    [Activity(Label = "@string/videos", Theme = "@style/AppTheme")]
    public class VideoViewerActivity : AppCompatActivity
    {
        #region Fields

        private const string _channelId = "app_notification_channel_id";
        private IHamwiServices _Hamwi;
        private InterstitialAd _interstitialAd;
        private IEnumerable<Video> _videos;
        private int _pageNumber = 1;
        private Video _video;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_video_viewer);

            Init();
        }

        private async void Init()
        {
            LoadAds();
            _Hamwi = new HamwiServices();
            _video = JsonConvert.DeserializeObject<Video>(Intent.GetStringExtra("video"));

            if (_video != null)
            {
                var videoTitle = FindViewById<TextView>(Resource.Id.video_title);
                var videoCoverImage = FindViewById<ImageView>(Resource.Id.video_cover_image);
                var videoPlayer = FindViewById<VideoView>(Resource.Id.video);
                var likeVideo = FindViewById<ImageView>(Resource.Id.like);
                var downlaodVideo = FindViewById<ImageView>(Resource.Id.download);
                var shareVideo = FindViewById<ImageView>(Resource.Id.share);
                var whatsappShareVideo = FindViewById<ImageView>(Resource.Id.whatsapp_share);

                if (Preferences.ContainsKey(GetString(Resource.String.videos), _video.Id.ToString()))
                    likeVideo.SetColorFilter(Android.Graphics.Color.Red);

                videoTitle.Text = _video.Title;
                Glide.With(this).Load(_video.CoverPath).Into(videoCoverImage);
                videoPlayer.SetVideoPath(_video.VideoPath);
                videoPlayer.SetMediaController(new MediaController(this));
                videoPlayer.Prepared += VideoPlayer_Prepared;
                likeVideo.Click += LikeVideo_Click;
                downlaodVideo.Click += DownlaodVideo_Click;
                shareVideo.Click += ShareVideo_Click;
                whatsappShareVideo.Click += WhatsappShareVideo_Click;
            }

            await LoadVideos();

            var adapter = new VideosAdapter(_videos ??= new List<Video>());
            adapter.ItemClick += Adapter_ItemClick;
            adapter.ItemLongClick += Adapter_ItemLongClick;

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

        private void LoadAds()
        {
            _interstitialAd = new InterstitialAd(this)
            {
                AdUnitId = GetString(Resource.String.interstitial_ad_unit_id)
            };

            _interstitialAd.LoadAd(new AdRequest.Builder().Build());
            FindViewById<AdView>(Resource.Id.banner_ad_view).LoadAd(new AdRequest.Builder().Build());
        }

        private void VideoPlayer_Prepared(object sender, System.EventArgs e)
        {
            FindViewById<ProgressBar>(Resource.Id.load_bar).Visibility = ViewStates.Gone;
            (sender as MediaPlayer).Start();
        }

        private void LikeVideo_Click(object sender, System.EventArgs e)
        {
            if (_video != null)
            {
                var likeIcon = FindViewById<ImageView>(Resource.Id.like);
                var key = GetString(Resource.String.videos);
                var id = _video.Id.ToString();

                if (Preferences.ContainsKey(key, id))
                {
                    likeIcon.ClearColorFilter();
                    Preferences.Remove(key, id);
                    Toast.MakeText(this, GetString(Resource.String.remove_from_favorites), ToastLength.Short).Show();
                }
                else
                {
                    likeIcon.SetColorFilter(Android.Graphics.Color.Red);
                    Preferences.Set(key, id, id);
                    Toast.MakeText(this, GetString(Resource.String.added_to_favorites), ToastLength.Short).Show();
                }
            }
        }

        private void DownlaodVideo_Click(object sender, System.EventArgs e)
            => new AndroidX.AppCompat.App.AlertDialog.Builder(this)
                .SetMessage(GetString(Resource.String.watch_ad))
                .SetPositiveButton(GetString(Resource.String.confirm), async (s, e) =>
                {
                    if (_interstitialAd.IsLoaded)
                        _interstitialAd.Show();

                    await InstallVideo();

                    LoadAds();
                })
                .SetNegativeButton(GetString(Resource.String.cancel), (s, e) => { })
                .Show();

        private void ShareVideo_Click(object sender, System.EventArgs e)
            => ShareVideo();

        private void WhatsappShareVideo_Click(object sender, System.EventArgs e)
        {
            var whatsapp = GetString(Resource.String.whatsapp_package_name);
            var whatsappBusiness = GetString(Resource.String.whatsapp_business_package_name);

            if (IsPackageInstalled(whatsapp)) ShareVideo(whatsapp);
            else if (IsPackageInstalled(whatsappBusiness)) ShareVideo(whatsappBusiness);
            else Toast.MakeText(this, GetString(Resource.String.whatsapp_not_found), ToastLength.Short).Show();
        }

        private async Task<string> InstallVideo()
        {
            if (_video == null)
                return string.Empty;

            var permission = Manifest.Permission.WriteExternalStorage;

            if (((int)Build.VERSION.SdkInt) >= 23)
            {
                if (CheckSelfPermission(permission) != Permission.Granted)
                {
                    new AndroidX.AppCompat.App.AlertDialog.Builder(this)
                        .SetTitle(GetString(Resource.String.access_permission))
                        .SetMessage(GetString(Resource.String.access))
                        .SetPositiveButton(GetString(Resource.String.confirm), (s, e) => { RequestPermissions(new string[] { permission }, 1963258470); })
                        .SetNegativeButton(GetString(Resource.String.cancel), (s, e) => { })
                        .Show();

                    return string.Empty;
                }
            }

            string applicationPath = Path.Combine(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads).AbsolutePath, GetString(Resource.String.app_name));

            if (!Directory.Exists(applicationPath))
                Directory.CreateDirectory(applicationPath);

            var downloadPath = Path.Combine(applicationPath, $"{_video.Title}-{_video.Id}.mp4");

            if (!File.Exists(downloadPath))
            {
                var loadingLayout = FindViewById<CardView>(Resource.Id.laoding_layout);

                try
                {
                    loadingLayout.Visibility = ViewStates.Visible;
                    await File.WriteAllBytesAsync(downloadPath, await _Hamwi.FileService.DownloadAsync(_video.VideoPath));
                    SendBroadcast(new Intent(Intent.ActionMediaScannerScanFile).SetData(Android.Net.Uri.FromFile(new Java.IO.File(downloadPath))));
                    Toast.MakeText(this, GetString(Resource.String.downloading_successfully), ToastLength.Short).Show();
                    ShowNotification(downloadPath);
                    return downloadPath;
                }
                catch
                {
                    Toast.MakeText(this, GetString(Resource.String.downloading_error), ToastLength.Short).Show();
                    return downloadPath;
                }
                finally
                {
                    loadingLayout.Visibility = ViewStates.Gone;
                }
            }

            Toast.MakeText(this, GetString(Resource.String.already_exists), ToastLength.Short).Show();
            return downloadPath;
        }

        private void ShowNotification(string path)
        {
            var intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(Android.Net.Uri.Parse(path), "video/*");

            NotificationManagerCompat.From(this).Notify(new System.Random().Next(),
                new NotificationCompat.Builder(this, _channelId)
                    .SetAutoCancel(true)
                    .SetSmallIcon(Resource.Mipmap.ic_launcher)
                    .SetContentTitle(GetString(Resource.String.downloading_successfully))
                    .SetContentText(_video.Title)
                    .SetStyle(new NotificationCompat.BigTextStyle()
                    .SetBigContentTitle(GetString(Resource.String.downloading_successfully))
                    .BigText(_video.Title))
                    .SetPriority(NotificationCompat.PriorityHigh)
                    .SetCategory(NotificationCompat.CategoryMessage)
                    .SetContentIntent(PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot))
                    .Build());
        }

        private void ShareVideo(string packageName = null)
        {
            new AndroidX.AppCompat.App.AlertDialog.Builder(this)
                .SetMessage(GetString(Resource.String.watch_ad))
                .SetPositiveButton(GetString(Resource.String.confirm), async (s, e) =>
                {
                    if (_interstitialAd.IsLoaded)
                        _interstitialAd.Show();

                    var path = await InstallVideo();

                    if ((!string.IsNullOrWhiteSpace(path)) && File.Exists(path))
                    {
                        var intentShareFile = new Intent(Intent.ActionSend);
                        intentShareFile.SetType(URLConnection.GuessContentTypeFromName(path));
                        intentShareFile.PutExtra(Intent.ExtraStream, Android.Net.Uri.Parse(path));
                        intentShareFile.PutExtra(Intent.ExtraText, GetString(Resource.String.download_our_app) + PackageName);
                        intentShareFile.PutExtra(Intent.ExtraSubject, GetString(Resource.String.app_name));

                        if (!string.IsNullOrWhiteSpace(packageName))
                            intentShareFile.SetPackage(packageName);

                        StartActivity(Intent.CreateChooser(intentShareFile, GetString(Resource.String.share)));

                        LoadAds();
                    }
                })
                .SetNegativeButton(GetString(Resource.String.cancel), (s, e) => { })
                .Show();
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

            try
            {
                loading.Visibility = ViewStates.Visible;

                var videos = await _Hamwi.VideoService.GetAsync(new VideoFilter
                {
                    Shuffle = true,
                    PageNumber = ++_pageNumber
                });

                if (videos != null && videos.Any())
                    _videos = ((_videos != null) && _videos.Any()) ? _videos.Concat(videos) : _videos = videos;
            }
            catch
            {
                Toast.MakeText(this, Resource.String.connection_error, ToastLength.Short).Show();
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
                    Finish();

                    if (_interstitialAd.IsLoaded)
                        _interstitialAd.Show();
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