using Android.Views;

namespace HamwiVideoStatuses.EventArgs
{
    public class RecyclerViewAdapterClickEventArgs : System.EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}