using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using AndroidX.ViewPager.Widget;
using AndroidX.Work;
using Google.Android.Material.Navigation;
using Google.Android.Material.Tabs;
using Java.Lang;
using Hamwi.Android.Adapters;
using Hamwi.Android.Helpers;
using Hamwi.Shared.Entities;
using Hamwi.Shared.Filters.Entity;
using Hamwi.Shared.Services.Client;
using Hamwi.Shared.Services.Client.Interfaces;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using static AndroidX.AppCompat.Widget.SearchView;
using static Google.Android.Material.Navigation.NavigationView;
using Uri = Android.Net.Uri;

namespace Hamwi.Android.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        #region Variables
        private readonly IHamwiServices _Hamwi;
        private long _pressedTime;
        #endregion

        public MainActivity()
            => _Hamwi = new HamwiServices();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.activity_main);

                Init();
            }
            catch
            {
                FinishAndRemoveTask();
                Toast.MakeText(this, GetString(Resource.String.startup_error), ToastLength.Long).Show();
            }
        }

        private async void Init()
        {
            MobileAds.Initialize(this);

            var categories = (await _Hamwi.CategoryService.GetAsync(new CategoryFilter { PageSize = int.MaxValue, Shuffle = true })) ?? new List<Category>();
            var pager = FindViewById<ViewPager>(Resource.Id.pager);
            pager.Adapter = new TabsAdapter(SupportFragmentManager, categories);
            FindViewById<TabLayout>(Resource.Id.tab_layout).SetupWithViewPager(pager);

            var toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            var drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            var toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();
            FindViewById<NavigationView>(Resource.Id.nav_view).NavigationItemSelected += NavigationView_NavigationItemSelected;
            drawer.Visibility = ViewStates.Visible;
            FindViewById<RelativeLayout>(Resource.Id.splash_layout).Visibility = ViewStates.Gone;

            CheckRequiredPermissions();
            CheckNotifications();
        }

        private void CheckRequiredPermissions()
        {
            string permission = Manifest.Permission.WriteExternalStorage;

            if ((((int)Build.VERSION.SdkInt) >= 23) && (CheckSelfPermission(permission) != Permission.Granted))
            {
                new AndroidX.AppCompat.App.AlertDialog.Builder(this)
                    .SetTitle(GetString(Resource.String.access_permission))
                    .SetMessage(GetString(Resource.String.access))
                    .SetPositiveButton(GetString(Resource.String.confirm), (s, e) => { RequestPermissions(new string[] { permission }, 129937541); })
                    .SetNegativeButton(GetString(Resource.String.cancel), (s, e) => { })
                    .Show();
            }
        }

        private async void CheckNotifications()
        {
            WorkManager.GetInstance(this).Enqueue(new PeriodicWorkRequest.Builder(typeof(NotificationWorker), TimeSpan.FromMinutes(10)).Build());

            var notificationId = Intent.GetStringExtra("notification_id");

            if (!string.IsNullOrWhiteSpace(notificationId))
            {
                var notification = await _Hamwi.NotificationService.GetAsync(Guid.Parse(notificationId));

                if (notification != null)
                {
                    notification.OpenedCount++;
                    await _Hamwi.NotificationService.UpdateAsync(notification);
                }
            }
        }

        private void NavigationView_NavigationItemSelected(object sender, NavigationItemSelectedEventArgs e)
        {
            CheckSelectedItem(e.MenuItem.ItemId);
            FindViewById<DrawerLayout>(Resource.Id.drawer_layout).CloseDrawer(GravityCompat.Start);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            var searchView = menu.FindItem(Resource.Id.nav_search).ActionView as AndroidX.AppCompat.Widget.SearchView;
            searchView.SetSearchableInfo((GetSystemService(SearchService) as SearchManager).GetSearchableInfo(ComponentName));
            searchView.QueryTextSubmit += SearchView_QueryTextSubmit;
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            CheckSelectedItem(item.ItemId);
            return base.OnOptionsItemSelected(item);
        }

        private void CheckSelectedItem(int id)
        {
            if (id == Resource.Id.nav_added_videos)
                StartActivity(new Intent(this, typeof(RecentlyAddedVideosActivity)));
            else if (id == Resource.Id.nav_like)
                StartActivity(new Intent(this, typeof(LikedVideosActivity)));
            else if (id == Resource.Id.nav_rate)
                StartActivity(new Intent(Intent.ActionView, Uri.Parse(GetString(Resource.String.app_link) + PackageName)));
            else if (id == Resource.Id.nav_share)
            {
                Intent intent = new Intent(Intent.ActionSend);
                intent.SetType("text/plain");
                intent.PutExtra(Intent.ExtraText, $"{GetString(Resource.String.app_name)}\n\n{GetString(Resource.String.description)}\n{GetString(Resource.String.app_link)}{PackageName}");
                StartActivity(Intent.CreateChooser(intent, GetString(Resource.String.share)));
            }
        }

        private void SearchView_QueryTextSubmit(object sender, QueryTextSubmitEventArgs e)
        {
            var intent = new Intent(this, typeof(SearchResultActivity));
            intent.PutExtra("term", e.NewText);
            StartActivity(intent);
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            if (drawer.IsDrawerOpen(GravityCompat.Start))
                drawer.CloseDrawer(GravityCompat.Start);
            else
            {
                if (_pressedTime + 2000 > JavaSystem.CurrentTimeMillis())
                    base.OnBackPressed();
                else
                    Toast.MakeText(this, GetString(Resource.String.press_back), ToastLength.Short).Show();

                _pressedTime = JavaSystem.CurrentTimeMillis();
            }
        }
    }
}