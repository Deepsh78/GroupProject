using AutoMapper;
using GroupApi.DTOs.Auth;
using GroupApi.DTOs.Books;
using GroupApi.Entities.Auth;
using GroupApi.Entities.Books;

namespace GroupApi.Configures
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDto, TempUserRegistration>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.OTP, opt => opt.Ignore())
                .ForMember(dest => dest.OTPExpiration, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<TempUserRegistration, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<BookCreateDto, Book>()
                .ForMember(dest => dest.BookId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Publisher, opt => opt.Ignore())
                .ForMember(dest => dest.BookAuthors, opt => opt.Ignore())
                .ForMember(dest => dest.BookGenres, opt => opt.Ignore())
                .ForMember(dest => dest.BookFormats, opt => opt.Ignore())
                .ForMember(dest => dest.BookCategories, opt => opt.Ignore());

            CreateMap<BookUpdateDto, Book>()
                .ForMember(dest => dest.BookId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Publisher, opt => opt.Ignore())
                .ForMember(dest => dest.BookAuthors, opt => opt.Ignore())
                .ForMember(dest => dest.BookGenres, opt => opt.Ignore())
                .ForMember(dest => dest.BookFormats, opt => opt.Ignore())
                .ForMember(dest => dest.BookCategories, opt => opt.Ignore());

            CreateMap<Book, BookReadDto>()
                .ForMember(dest => dest.PublisherName, opt => opt.MapFrom(src => src.Publisher != null ? src.Publisher.Name : ""));
        }
    }
}