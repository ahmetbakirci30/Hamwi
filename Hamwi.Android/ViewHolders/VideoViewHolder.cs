using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using Hamwi.Android.EventArguments;
using Hamwi.Shared.Entities.Statuses;
using System;
using Xamarin.Essentials;

namespace Hamwi.Android.ViewHolders
{
    public class VideoViewHolder : RecyclerView.ViewHolder
    {
        #region Initialize Properties
        public TextView Title { get; private set; }
        public ImageView CoverImage { get; private set; }
        public TextView ViewsCount { get; private set; }
        public TextView LikesCount { get; private set; }
        public TextView SharesCount { get; private set; }
        public TextView DownloadsCount { get; private set; }
        public ImageView LikeIcon { get; private set; }
        public LinearLayout BtnLike { get; private set; }
        #endregion

        protected internal VideoViewHolder(View itemView, Action<RecyclerViewAdapterClickEventArgs> clickListener, Action<RecyclerViewAdapterClickEventArgs> longClickListener, Action<RecyclerViewAdapterClickEventArgs> likeClickListener) : base(itemView)
        {
            #region Assign Properties
            Title = itemView.FindViewById<TextView>(Resource.Id.tv_video_title);
            CoverImage = itemView.FindViewById<ImageView>(Resource.Id.iv_video_cover);
            ViewsCount = itemView.FindViewById<TextView>(Resource.Id.tv_views_count);
            LikesCount = itemView.FindViewById<TextView>(Resource.Id.tv_likes_count);
            SharesCount = itemView.FindViewById<TextView>(Resource.Id.tv_shares_count);
            DownloadsCount = itemView.FindViewById<TextView>(Resource.Id.tv_downloads_count);
            LikeIcon = itemView.FindViewById<ImageView>(Resource.Id.iv_like);
            BtnLike = itemView.FindViewById<LinearLayout>(Resource.Id.ll_like);
            #endregion

            #region Click Events
            itemView.Click += (s, e) => clickListener(new RecyclerViewAdapterClickEventArgs { View = itemView, Position = LayoutPosition });
            itemView.LongClick += (s, e) => longClickListener(new RecyclerViewAdapterClickEventArgs { View = itemView, Position = LayoutPosition });
            BtnLike.Click += (s, e) => likeClickListener(new RecyclerViewAdapterClickEventArgs { View = itemView, Position = LayoutPosition });
            #endregion
        }

        protected internal void SetData(Video video)
        {
            LikeIcon.ClearColorFilter();

            if (Preferences.ContainsKey(Application.Context.GetString(Resource.String.action_videos), video.Id.ToString()))
                LikeIcon.SetColorFilter(Color.Red);

            Title.Text = video.Title;
            Glide.With(Application.Context).Load(video.CoverPath).Into(CoverImage);
            ViewsCount.Text = video.ViewsCount.ToString();
            LikesCount.Text = video.LikesCount.ToString();
            SharesCount.Text = video.SharesCount.ToString();
            DownloadsCount.Text = video.DownloadsCount.ToString();
        }
    }
}