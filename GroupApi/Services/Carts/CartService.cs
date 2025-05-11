using GroupApi.CommonDomain;
using GroupApi.DTOs.Carts;
using GroupApi.Entities;
using GroupApi.Entities.Books;
using GroupApi.GenericClasses;
using GroupApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GroupApi.Services.Carts
{
    public class CartService : ICartService
    {
        private readonly IGenericRepository<Cart> _cartRepo;
        private readonly IGenericRepository<CartItem> _cartItemRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IGenericRepository<Book> _bookRepo;

        public CartService(IGenericRepository<Cart> cartRepo, IGenericRepository<CartItem> cartItemRepo, ICurrentUserService currentUserService, IGenericRepository<Book> bookRepo)
        {
            _cartRepo = cartRepo;
            _cartItemRepo = cartItemRepo;
            _currentUserService = currentUserService;
            _bookRepo = bookRepo;
        }

        public async Task<GenericResponse<IEnumerable<CartDto>>> GetAllAsync()
        {
            var memberId = _currentUserService.UserId;
            var cart = await _cartRepo.TableNoTracking
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.MemberId == memberId);

            if (cart == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Cart not found");

            var result = cart.CartItems.Select(ci => new CartDto
            {
                CartId = ci.CartId,
                MemberId = ci.Cart.MemberId,
                CartItems = cart.CartItems.Select(c => new CartItemDto
                {
                    CartItemId = c.CartItemId,
                    CartId = c.CartId,
                    BookId = c.BookId,
                    BookName = c.Book.BookName,
                    Quantity = c.Quantity,
                    Price = c.Book.Price
                }).ToList()
            }).ToList();

            return result;
        }

        public async Task<GenericResponse<CartDto?>> GetByIdAsync()
        {
            var memberId = _currentUserService.UserId;
            var cart = await _cartRepo.TableNoTracking
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.MemberId == memberId);

            if (cart == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Cart not found");

            var cartDto = new CartDto
            {
                CartId = cart.CartId,
                MemberId = cart.MemberId,
                CartItems = cart.CartItems.Select(ci => new CartItemDto
                {
                    CartItemId = ci.CartItemId,
                    CartId = ci.CartId,
                    BookId = ci.BookId,
                    BookName = ci.Book.BookName,
                    Quantity = ci.Quantity,
                    Price = ci.Book.Price
                }).ToList()
            };

            return cartDto;
        }

        public async Task<GenericResponse<CartDto>> AddAsync(Guid bookId, int quantity)
        {
            try
            {
                var book = await _bookRepo.TableNoTracking
                    .FirstOrDefaultAsync(b => b.BookId == bookId);

                if (book == null)
                    return new ErrorModel(HttpStatusCode.NotFound, "Book not found");

                var memberId = _currentUserService.UserId;
                var cart = await _cartRepo.TableNoTracking
                    .FirstOrDefaultAsync(c => c.MemberId == memberId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        CartId = Guid.NewGuid(),
                        MemberId = memberId
                    };
                    await _cartRepo.AddAsync(cart);
                }

                var existingCartItem = await _cartItemRepo.TableNoTracking
                    .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.BookId == bookId);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += quantity; // Correctly add to the quantity
                    _cartItemRepo.Update(existingCartItem);
                }
                else
                {
                    var cartItem = new CartItem
                    {
                        CartItemId = Guid.NewGuid(),
                        CartId = cart.CartId,
                        BookId = bookId,
                        Quantity = quantity
                    };
                    await _cartItemRepo.AddAsync(cartItem);
                }

                await _cartRepo.SaveChangesAsync();

                var cartDto = new CartDto
                {
                    CartId = cart.CartId,
                    MemberId = cart.MemberId,
                    CartItems = await _cartItemRepo.TableNoTracking
                        .Where(ci => ci.CartId == cart.CartId)
                        .Include(ci => ci.Book)  // Eagerly load the related Book entity
                        .Select(ci => new CartItemDto
                        {
                            CartItemId = ci.CartItemId,
                            CartId = ci.CartId,
                            BookId = ci.BookId,
                            BookName = ci.Book.BookName,
                            Quantity = ci.Quantity,
                            Price = ci.Book.Price,
                            TotalPrice = ci.Quantity * ci.Book.Price // Calculate total price (price * quantity)
                        }).ToListAsync()
                };

                return cartDto;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<GenericResponse<CartDto>> UpdateAsync(Guid cartItemId, int quantity)
        {
            var cartItem = await _cartItemRepo.GetByIdAsync(cartItemId);
            if (cartItem == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Cart item not found");

            cartItem.Quantity = quantity;
            _cartItemRepo.Update(cartItem);
            await _cartRepo.SaveChangesAsync();

            var cartDto = new CartDto
            {
                CartId = cartItem.CartId,
                MemberId = cartItem.Cart.MemberId,
                CartItems = new List<CartItemDto>
                {
                    new CartItemDto
                    {
                        CartItemId = cartItem.CartItemId,
                        CartId = cartItem.CartId,
                        BookId = cartItem.BookId,
                        BookName = cartItem.Book.BookName,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Book.Price
                    }
                }
            };

            return cartDto;
        }

        public async Task<GenericResponse<CartDto>> RemoveAsync(Guid cartItemId)
        {
            var cartItem = await _cartItemRepo.GetByIdAsync(cartItemId);
            if (cartItem == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Cart item not found");

            _cartItemRepo.Delete(cartItem);
            await _cartRepo.SaveChangesAsync();

            var cartDto = new CartDto
            {
                CartId = cartItem.CartId,
                MemberId = cartItem.Cart.MemberId,
                CartItems = new List<CartItemDto>()
            };

            return cartDto;
        }
    }
}
