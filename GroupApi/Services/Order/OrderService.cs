using GroupApi.CommonDomain;
using GroupApi.Constants;
using GroupApi.Data;
using GroupApi.DTOs.Orders;
using GroupApi.Entities.Auth;
using GroupApi.Entities.Books;
using GroupApi.Entities.Oders;
using GroupApi.GenericClasses;
using GroupApi.Services.CurrentUser;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

public class OrderService : IOrderService
{
    private readonly IGenericRepository<Order> _orderRepo;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IGenericRepository<Book> _bookRepo;
    private readonly ICurrentUserService _currentUserService;

    public OrderService(
        IGenericRepository<Order> orderRepo,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        IGenericRepository<Book> bookRepo,
        ICurrentUserService currentUserService)
    {
        _orderRepo = orderRepo;
        _userManager = userManager;
        _context = context;
        _bookRepo = bookRepo;
        _currentUserService = currentUserService;
    }

    public async Task<GenericResponse<bool>> ProcessClaimCodeAsync(ProcessClaimCodeDto dto)
    {
        
        var staffId = _currentUserService.UserId.ToString();
        var staff = await _userManager.FindByIdAsync(staffId);
        if (staff == null || staff.Role != RoleType.Staff)
            return new ErrorModel(HttpStatusCode.Forbidden, "Only staff can process claim codes");

      
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderId == dto.OrderId);

        if (order == null)
            return new ErrorModel(HttpStatusCode.NotFound, "Order not found");

        if (order.ClaimCode != dto.ClaimCode)
            return new ErrorModel(HttpStatusCode.BadRequest, "Invalid claim code");

        
        if (order.Status == "Fulfilled")
            return new ErrorModel(HttpStatusCode.BadRequest, "Order already fulfilled");

       
        foreach (var item in order.OrderItems)
        {
            var book = await _bookRepo.Table.FirstOrDefaultAsync(b => b.BookId == item.BookId);
            if (book == null)
                return new ErrorModel(HttpStatusCode.NotFound, $"Book with ID {item.BookId} not found");

            if (book.Stock < item.Quantity)
                return new ErrorModel(HttpStatusCode.BadRequest, $"Insufficient stock for {book.BookName}");

            book.Stock -= item.Quantity;
            _bookRepo.Update(book);
        }

        order.Status = "Fulfilled";
        order.OrderDate = DateTime.UtcNow;
        _context.Orders.Update(order);

       
        await _context.SaveChangesAsync();

        return new GenericResponse<bool> { Data = true };
    }
}
