using Acb.Core.Domain;
using GroupApi.CommonDomain;
using GroupApi.DTOs.Bookmarks;
using GroupApi.Entities.Books;
using GroupApi.GenericClasses;
using GroupApi.Services.CurrentUser;
using GroupApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GroupApi.Services.BookMarks
{
    public class BookMarkService : IBookMarkService
    {
        private readonly IGenericRepository<BookMark> _bookMarkRepo;
        private readonly ICurrentUserService _currentUserService;
       

        public BookMarkService(IGenericRepository<BookMark> bookMarkRepo,  ICurrentUserService currentUserService)
        {
            _bookMarkRepo = bookMarkRepo;
            _currentUserService = currentUserService;
        
        }

      
        public async Task<GenericResponse<IEnumerable<BookMarkDto>>> GetBookmarksByMemberAsync(Guid memberId)
        {
            var bookmarks = await _bookMarkRepo.TableNoTracking
                .Where(b => b.MemberId == memberId)
                .Include(b => b.Book)
                .ToListAsync();

            if (bookmarks == null || bookmarks.Count == 0)
                return new ErrorModel(HttpStatusCode.NotFound, "No bookmarks found for this member.");

            var result = bookmarks.Select(b => new BookMarkDto
            {
                BookMarkId = b.BookMarkId,
                BookId = b.BookId,
                MemberId = b.MemberId,
              
            });

            return result.ToList();
        }

        public async Task<GenericResponse<BookMarkDto>> AddBookmarkAsync(Guid bookId)
        {
            var memberId = _currentUserService.UserId;  
            var existingBookmark = await _bookMarkRepo.TableNoTracking
                .FirstOrDefaultAsync(b => b.MemberId == memberId && b.BookId == bookId);

            if (existingBookmark != null)
                return new ErrorModel(HttpStatusCode.BadRequest, "This book is already bookmarked.");

            var bookMark = new BookMark
            {
                BookMarkId = Guid.NewGuid(),
                BookId = bookId,
                MemberId = memberId
            };

            await _bookMarkRepo.AddAsync(bookMark);
          
            return new BookMarkDto
            {
                BookMarkId = bookMark.BookMarkId,
                BookId = bookMark.BookId,
                MemberId = bookMark.MemberId,
              
            };
        }

       
        public async Task<GenericResponse<BookMarkDto>> RemoveBookmarkAsync(Guid bookId)
        {
            var memberId = _currentUserService.UserId;  

            var bookmark = await _bookMarkRepo.TableNoTracking
                .FirstOrDefaultAsync(b => b.MemberId == memberId && b.BookId == bookId);

            if (bookmark == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Bookmark not found.");

            _bookMarkRepo.Delete(bookmark);
         

            return new BookMarkDto
            {
                BookMarkId = bookmark.BookMarkId,
                BookId = bookmark.BookId,
                MemberId = bookmark.MemberId,
          
            };
        }
    }

}
