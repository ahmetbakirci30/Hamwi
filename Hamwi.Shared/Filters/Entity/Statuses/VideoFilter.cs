using Hamwi.Shared.Entities.Statuses;
using Hamwi.Shared.Filters.Entity.Statuses.Base;
using System.Linq;

namespace Hamwi.Shared.Filters.Entity.Statuses
{
    public class VideoFilter : StatusFilter<Video>
    {
        public string Title { get; set; }
        public string VideoPath { get; set; }
        public string CoverPath { get; set; }

        public override IQueryable<Video> Build(IQueryable<Video> videos, bool applyPagination = true)
        {
            videos = string.IsNullOrWhiteSpace(Title) ? videos :
                   videos.Where(video => video.Title.ToLower().Contains(Title.ToLower()));

            videos = string.IsNullOrWhiteSpace(VideoPath) ? videos :
                   videos.Where(video => video.VideoPath.Equals(VideoPath));

            videos = string.IsNullOrWhiteSpace(CoverPath) ? videos :
                   videos.Where(video => video.CoverPath.Equals(CoverPath));

            return base.Build(videos, applyPagination);
        }

        public override string ToString()
            => $"{base.ToString()}&{nameof(Title)}={Title}&{nameof(VideoPath)}={VideoPath}&{nameof(CoverPath)}={CoverPath}";
    }
}