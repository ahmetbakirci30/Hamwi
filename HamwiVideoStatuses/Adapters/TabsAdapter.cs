using AndroidX.Fragment.App;
using HamwiVideoStatuses.Fragments;
using Java.Lang;
using Hamwi.Shared.Entities;
using System.Collections.Generic;
using System.Linq;

namespace HamwiVideoStatuses.Adapters
{
    public class TabsAdapter : FragmentStatePagerAdapter
    {
        private readonly IEnumerable<Category> _categories;

        public TabsAdapter(FragmentManager fm, IEnumerable<Category> categories) : base(fm, BehaviorResumeOnlyCurrentFragment)
            => _categories = categories;

        public override int Count => _categories.Count();

        public override Fragment GetItem(int position)
            => new VideosFragment(_categories?.ElementAtOrDefault(position)?.Id);

        public override ICharSequence GetPageTitleFormatted(int position)
            => new String(_categories?.ElementAtOrDefault(position)?.Name);
    }
}