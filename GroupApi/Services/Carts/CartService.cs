using Acb.Core.Domain;
using GroupApi.CommonDomain;
using GroupApi.Entities.Books;
using GroupApi.Entities;
using GroupApi.GenericClasses;
using GroupApi.Services.CurrentUser;
using System.Net;
using Microsoft.EntityFrameworkCore;
using GroupApi.DTOs.Carts;

namespace GroupApi.Services.Carts
{
    public class CartService : ICartService
    {
        public Task<GenericResponse<CartDto>> AddToCartAsync(Guid bookId, int quantity)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse<IEnumerable<CartDto>>> GetCartByMemberAsync(Guid memberId)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse<IEnumerable<CartDto>>> GetCartForCurrentUserAsync()
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse<CartDto>> RemoveFromCartAsync(Guid cartItemId)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse<CartDto>> UpdateCartItemAsync(Guid cartItemId, int quantity)
        {
            throw new NotImplementedException();
        }
    }



}
