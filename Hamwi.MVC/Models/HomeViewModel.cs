using Hamwi.Shared.Entities.Statuses;
using System.Collections.Generic;

namespace Hamwi.MVC.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Video> Videos { get; set; }
        public IEnumerable<Image> Images { get; set; }
        public IEnumerable<Quote> Quotes { get; set; }
    }
}