using Android.App;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Bumptech.Glide;
using HamwiVideoStatuses.EventArgs;
using Hamwi.Shared.Entities.Statuses;
using System;

namespace HamwiVideoStatuses.ViewHolders
{
    public class VideoViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; }
        public ImageView CoverImage { get; }

        protected internal VideoViewHolder(View itemView, Action<RecyclerViewAdapterClickEventArgs> clickListener, Action<RecyclerViewAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.video_title);
            CoverImage = itemView.FindViewById<ImageView>(Resource.Id.video_cover_image);

            itemView.Click += (s, e) => clickListener(new RecyclerViewAdapterClickEventArgs { View = itemView, Position = LayoutPosition });
            itemView.LongClick += (s, e) => longClickListener(new RecyclerViewAdapterClickEventArgs { View = itemView, Position = LayoutPosition });
        }

        protected internal void SetData(Video video)
        {
            Title.Text = video?.Title;
            Glide.With(Application.Context).Load(video?.CoverPath).Into(CoverImage);
        }
    }
}