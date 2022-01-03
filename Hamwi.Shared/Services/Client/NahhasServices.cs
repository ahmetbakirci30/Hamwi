using Hamwi.Shared.Entities;
using Hamwi.Shared.Entities.Statuses;
using Hamwi.Shared.Services.Client.Interfaces;
using Hamwi.Shared.Services.Files;
using Hamwi.Shared.Services.Files.Interfaces;
using Hamwi.Shared.Services.Http;
using Hamwi.Shared.Services.Http.Interfaces;
using Hamwi.Shared.Services.Hamwi;
using Hamwi.Shared.Services.Hamwi.Interfaces;

namespace Hamwi.Shared.Services.Client
{
    public class HamwiServices : IHamwiServices
    {
        private readonly IHttpService _service;

        public HamwiServices() => _service = new HttpService();

        public IHamwiService<Category> CategoryService => new HamwiService<Category>(_service, "categories");
        public IHamwiService<Video> VideoService => new HamwiService<Video>(_service, "videos");
        public IHamwiService<Image> ImageService => new HamwiService<Image>(_service, "images");
        public IHamwiService<Quote> QuoteService => new HamwiService<Quote>(_service, "quotes");
        public IHamwiService<Notification> NotificationService => new HamwiService<Notification>(_service, "notifications");
        public IFileService FileService => new FileService(_service, "files");
    }
}