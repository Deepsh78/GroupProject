﻿using GroupApi.CommonDomain;
using GroupApi.DTOs.Bookmarks;

namespace GroupApi.Services.Interface
{
    public interface IBookMarkService
    {
        Task<GenericResponse<IEnumerable<BookMarkDto>>> GetBookmarksByMemberAsync();
        Task<GenericResponse<BookMarkDto>> AddBookmarkAsync(Guid bookId);
        Task<GenericResponse<BookMarkDto>> RemoveBookmarkAsync(Guid bookId);
    }

}
