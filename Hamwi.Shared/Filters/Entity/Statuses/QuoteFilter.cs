using Hamwi.Shared.Entities.Statuses;
using Hamwi.Shared.Filters.Entity.Statuses.Base;
using System.Linq;

namespace Hamwi.Shared.Filters.Entity.Statuses
{
    public class QuoteFilter : StatusFilter<Quote>
    {
        public string Content { get; set; }

        public override IQueryable<Quote> Build(IQueryable<Quote> quotes, bool applyPagination = true)
        {
            quotes = string.IsNullOrWhiteSpace(Content) ? quotes :
                quotes.Where(quote => quote.Content.ToLower().Contains(Content.ToLower()));

            return base.Build(quotes, applyPagination);
        }

        public override string ToString()
            => $"{base.ToString()}&{nameof(Content)}={Content}";
    }
}