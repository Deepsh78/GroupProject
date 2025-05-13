using GroupApi.CommonDomain;
using GroupApi.DTOs.Carts;
using GroupApi.Entities;
using GroupApi.Entities.Books;
using GroupApi.GenericClasses;
using GroupApi.Services.CurrentUser;
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

        public async Task<GenericResponse<CartDto>> GetAllAsync()
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
                CartItems = cart.CartItems.Select(c => new CartItemDto
                {
                    CartItemId = c.CartItemId,
                    CartId = c.CartId,
                    BookId = c.BookId,
                    BookName = c.Book.BookName,
                    Quantity = c.Quantity,
                    Price = c.Book.Price,
                    TotalPrice = c.Quantity * c.Book.Price
                }).ToList()
            };

            return cartDto;
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
                    Price = ci.Book.Price,
                    TotalPrice = ci.Quantity * ci.Book.Price // Calculate total price (price * quantity)
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

                // Get or create cart
                var cart = await _cartRepo.Table
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.MemberId == memberId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        CartId = Guid.NewGuid(),
                        MemberId = memberId,
                        CartItems = new List<CartItem>()
                    };
                    await _cartRepo.AddAsync(cart);
                    await _cartRepo.SaveChangesAsync(); // Save cart first to get ID
                }

                // Check if book already in cart
                var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.BookId == bookId);

                int totalRequestedQuantity = quantity;
                if (existingCartItem != null)
                    totalRequestedQuantity += existingCartItem.Quantity;

                if (totalRequestedQuantity > book.Stock)
                    return new ErrorModel(HttpStatusCode.BadRequest, "Not enough stock available for this book");

                if (existingCartItem != null)
                {
                    // UPDATE quantity
                    existingCartItem.Quantity += quantity;
                    _cartItemRepo.Update(existingCartItem); // Force EF to recognize change
                }
                else
                {
                    // ADD new item
                    var newItem = new CartItem
                    {
                        CartItemId = Guid.NewGuid(),
                        CartId = cart.CartId,
                        BookId = bookId,
                        Quantity = quantity
                    };
                    await _cartItemRepo.AddAsync(newItem);
                }

                await _cartRepo.SaveChangesAsync();

                // Return the updated cart
                var cartDto = new CartDto
                {
                    CartId = cart.CartId,
                    MemberId = cart.MemberId,
                    CartItems = await _cartItemRepo.TableNoTracking
                        .Where(ci => ci.CartId == cart.CartId)
                        .Include(ci => ci.Book)
                        .Select(ci => new CartItemDto
                        {
                            CartItemId = ci.CartItemId,
                            CartId = ci.CartId,
                            BookId = ci.BookId,
                            BookName = ci.Book.BookName,
                            Quantity = ci.Quantity,
                            Price = ci.Book.Price,
                            TotalPrice = ci.Quantity * ci.Book.Price
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

        public async Task<GenericResponse<CartDto>> RemoveAsync(Guid cartId)
        {
            var cart = await _cartRepo.GetByIdAsync(cartId);
            if (cart == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Cart not found");

            _cartRepo.Delete(cart);
            await _cartRepo.SaveChangesAsync();

            var cartDto = new CartDto
            {
                CartId = cart.CartId,
                MemberId = cart.MemberId,
                CartItems = new List<CartItemDto>()
            };

            return cartDto;
        }
        public async Task<GenericResponse<CartDto>> RemoveCartItemAsync(Guid cartItemId)
        {
            var cartItem = await _cartItemRepo.Table
                .Include(ci => ci.Book)
                .Include(ci => ci.Cart)
                .ThenInclude(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);

            if (cartItem == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Cart item not found");

            var cart = cartItem.Cart;

            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity -= 1;
                await _cartItemRepo.SaveChangesAsync();
            }
            else
            {
                _cartItemRepo.Delete(cartItem);
                await _cartItemRepo.SaveChangesAsync();
            }

            // Refresh the cart and its items with book info
            cart = await _cartRepo.TableNoTracking
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.CartId == cart.CartId);

            var updatedCartItems = cart.CartItems.Select(c => new CartItemDto
            {
                CartItemId = c.CartItemId,
                CartId = c.CartId,
                BookId = c.BookId,
                BookName = c.Book?.BookName ?? "[Unknown]",
                Quantity = c.Quantity,
                Price = c.Book?.Price ?? 0,
                TotalPrice = c.Quantity * (c.Book?.Price ?? 0)
            }).ToList();

            var updatedCartDto = new CartDto
            {
                CartId = cart.CartId,
                MemberId = cart.MemberId,
                CartItems = updatedCartItems
            };

            return updatedCartDto;
        }



    }
}
