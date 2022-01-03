using Android.Views;
using System;

namespace Hamwi.Android.EventArguments
{
    public class RecyclerViewAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}