using GroupApi.CommonDomain;
using GroupApi.DTOs.Publisher;
using GroupApi.Entities;
using GroupApi.GenericClasses;
using GroupApi.Services.Interface;
using System.Net;

namespace GroupApi.Services.Publishers
{
    public class PublisherService : IPublisherService
    {
        private readonly IGenericRepository<Publisher> _publisherRepo;

        public PublisherService(IGenericRepository<Publisher> publisherRepo)
        {
            _publisherRepo = publisherRepo;
        }

        public async Task<GenericResponse<IEnumerable<PublisherReadDto>>> GetAllAsync()
        {
            var items = await _publisherRepo.GetAllAsync();
            return items.Select(p => new PublisherReadDto
            {
                PublisherId = p.PublisherId,
                Name = p.Name,
                Contact = p.Contact,
                Email = p.Email
            }).ToList();
        }

        public async Task<GenericResponse<PublisherReadDto?>> GetByIdAsync(Guid id)
        {
            var publisher = await _publisherRepo.GetByIdAsync(id);
            if (publisher == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Publisher not found");

            return new PublisherReadDto
            {
                PublisherId = publisher.PublisherId,
                Name = publisher.Name,
                Contact = publisher.Contact,
                Email = publisher.Email
            };
        }

        public async Task<GenericResponse<PublisherReadDto>> CreateAsync(PublisherCreateDto dto)
        {
            var publisher = new Publisher
            {
                PublisherId = Guid.NewGuid(),
                Name = dto.Name,
                Contact = dto.Contact,
                Email = dto.Email
            };

            await _publisherRepo.AddAsync(publisher);
            await _publisherRepo.SaveChangesAsync(CancellationToken.None);

            return new PublisherReadDto
            {
                PublisherId = publisher.PublisherId,
                Name = publisher.Name,
                Contact = publisher.Contact,
                Email = publisher.Email
            };
        }

        public async Task<GenericResponse<PublisherReadDto?>> UpdateAsync(Guid id, PublisherUpdateDto dto)
        {
            var publisher = await _publisherRepo.GetByIdAsync(id);
            if (publisher == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Publisher not found");

            publisher.Name = dto.Name;
            publisher.Contact = dto.Contact;
            publisher.Email = dto.Email;

            _publisherRepo.Update(publisher);
            await _publisherRepo.SaveChangesAsync(CancellationToken.None);

            return new PublisherReadDto
            {
                PublisherId = publisher.PublisherId,
                Name = publisher.Name,
                Contact = publisher.Contact,
                Email = publisher.Email
            };
        }

        public async Task<Response> DeleteAsync(Guid id)
        {
            var publisher = await _publisherRepo.GetByIdAsync(id);
            if (publisher == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Publisher not found");

            _publisherRepo.Delete(publisher);
            await _publisherRepo.SaveChangesAsync(CancellationToken.None);
            return new Response();
        }
    }

}
