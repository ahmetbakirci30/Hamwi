using AndroidX.RecyclerView.Widget;
using System;

namespace HamwiVideoStatuses.Helpers
{
    public class EndlessRecyclerOnScrollListener : RecyclerView.OnScrollListener
    {
        public event EventHandler LoadMoreEvent;

        private readonly LinearLayoutManager _linearLayoutManager;
        private readonly int visibleThreshold = 5;
        private int previousTotal = 0;
        private bool loading = true;

        public EndlessRecyclerOnScrollListener(LinearLayoutManager linearLayoutManager)
            => _linearLayoutManager = linearLayoutManager;

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            var visibleItemCount = _linearLayoutManager.ChildCount;
            var totalItemCount = _linearLayoutManager.ItemCount;
            var firstVisibleItem = _linearLayoutManager.FindFirstVisibleItemPosition();

            if (loading && (totalItemCount > previousTotal))
            {
                loading = false;
                previousTotal = totalItemCount;
            }

            if (!loading && ((totalItemCount - visibleItemCount) <= (firstVisibleItem + visibleThreshold)))
            {
                LoadMoreEvent?.Invoke(this, System.EventArgs.Empty);
                loading = true;
            }

            //if ((visibleItemCount + firstVisibleItem) >= totalItemCount)
            //    LoadMoreEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}