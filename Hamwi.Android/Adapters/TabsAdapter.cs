using AndroidX.Fragment.App;
using Java.Lang;
using Hamwi.Android.Fragments;
using Hamwi.Shared.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Hamwi.Android.Adapters
{
    public class TabsAdapter : FragmentStatePagerAdapter
    {
        private readonly IEnumerable<Category> _categories;

        public TabsAdapter(FragmentManager fm, IEnumerable<Category> categories) : base(fm, BehaviorResumeOnlyCurrentFragment)
            => _categories = categories;

        public override int Count => _categories.Count();

        public override Fragment GetItem(int position)
            => new VideosFragment(_categories.ElementAt(position).Id);

        public override ICharSequence GetPageTitleFormatted(int position)
            => new String(_categories.ElementAt(position).Name);
    }
}