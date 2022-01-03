using Android.Views;
using AndroidX.RecyclerView.Widget;
using HamwiVideoStatuses.EventArgs;
using HamwiVideoStatuses.ViewHolders;
using Hamwi.Shared.Entities.Statuses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HamwiVideoStatuses.Adapters
{
    public class VideosAdapter : RecyclerView.Adapter
    {
        #region Click Events
        public event EventHandler<RecyclerViewAdapterClickEventArgs> ItemClick;
        public event EventHandler<RecyclerViewAdapterClickEventArgs> ItemLongClick;

        private void OnClick(RecyclerViewAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        private void OnLongClick(RecyclerViewAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        #endregion

        public IEnumerable<Video> Videos { get; set; }

        public VideosAdapter(IEnumerable<Video> videos)
            => Videos = videos;

        public override int ItemCount => Videos.Count();

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            => (holder as VideoViewHolder).SetData(Videos?.ElementAtOrDefault(position));

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            => new VideoViewHolder(LayoutInflater.From(parent.Context).Inflate(Resource.Layout.video_item, parent, false), OnClick, OnLongClick);
    }
}