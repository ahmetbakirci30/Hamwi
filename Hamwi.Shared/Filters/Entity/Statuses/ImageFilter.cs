using Hamwi.Shared.Entities.Statuses;
using Hamwi.Shared.Filters.Entity.Statuses.Base;
using System.Linq;

namespace Hamwi.Shared.Filters.Entity.Statuses
{
    public class ImageFilter : StatusFilter<Image>
    {
        public string ImagePath { get; set; }

        public override IQueryable<Image> Build(IQueryable<Image> images, bool applyPagination = true)
        {
            images = string.IsNullOrWhiteSpace(ImagePath) ? images :
                images.Where(image => image.ImagePath.Equals(ImagePath));

            return base.Build(images, applyPagination);
        }

        public override string ToString()
            => $"{base.ToString()}&{nameof(ImagePath)}={ImagePath}";
    }
}