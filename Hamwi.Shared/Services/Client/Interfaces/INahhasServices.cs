using Hamwi.Shared.Entities;
using Hamwi.Shared.Entities.Statuses;
using Hamwi.Shared.Services.Files.Interfaces;
using Hamwi.Shared.Services.Hamwi.Interfaces;

namespace Hamwi.Shared.Services.Client.Interfaces
{
    public interface IHamwiServices
    {
        IHamwiService<Category> CategoryService { get; }
        IHamwiService<Video> VideoService { get; }
        IHamwiService<Image> ImageService { get; }
        IHamwiService<Quote> QuoteService { get; }
        IHamwiService<Notification> NotificationService { get; }
        IFileService FileService { get; }
    }
}