// GroupApi.Services.Discounts/DiscountService.cs
using GroupApi.CommonDomain;
using GroupApi.Constants;
using GroupApi.DTOs.Discount;
using GroupApi.Entities;
using GroupApi.Entities.Auth;
using GroupApi.Entities.Books;
using GroupApi.Entities.Discount;
using GroupApi.GenericClasses;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GroupApi.Services.Admin
{
    public class DiscountAdminService : IDiscountService
    {
        private readonly IGenericRepository<Discount> _discountRepo;
        private readonly IGenericRepository<Book> _bookRepo;

        public DiscountAdminService(
            IGenericRepository<Discount> discountRepo,
            IGenericRepository<Book> bookRepo)
        {
            _discountRepo = discountRepo;
            _bookRepo = bookRepo;
        }

        public async Task<GenericResponse<IEnumerable<DiscountReadDto>>> GetAllAsync()
        {
            var discounts = await _discountRepo.TableNoTracking
                .Include(d => d.Book)
                .ToListAsync();

            return discounts.Select(d => new DiscountReadDto
            {
                DiscountId = d.DiscountId,
                BookId = d.BookId,
                BookName = d.Book.BookName,
                Percentage = d.Percentage,
                OnSale = d.OnSale,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                IsActive = d.IsActive
            }).ToList();
        }

        public async Task<GenericResponse<DiscountReadDto?>> GetByIdAsync(Guid id)
        {
            var discount = await _discountRepo.TableNoTracking
                .Include(d => d.Book)
                .FirstOrDefaultAsync(d => d.DiscountId == id);

            if (discount == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Discount not found");

            return new DiscountReadDto
            {
                DiscountId = discount.DiscountId,
                BookId = discount.BookId,
                BookName = discount.Book.BookName,
                Percentage = discount.Percentage,
                OnSale = discount.OnSale,
                StartDate = discount.StartDate,
                EndDate = discount.EndDate,
                IsActive = discount.IsActive
            };
        }

        public async Task<GenericResponse<DiscountReadDto>> CreateAsync(DiscountCreateDto dto)
        {
            var book = await _bookRepo.GetByIdAsync(dto.BookId);
            if (book == null)
                return new ErrorModel(HttpStatusCode.BadRequest, "Invalid book ID");

            if (dto.StartDate >= dto.EndDate)
                return new ErrorModel(HttpStatusCode.BadRequest, "Start date must be before end date");

            var discount = new Discount
            {
                DiscountId = Guid.NewGuid(),
                BookId = dto.BookId,
                Percentage = dto.Percentage,
                OnSale = dto.OnSale,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            await _discountRepo.AddAsync(discount);
            await _discountRepo.SaveChangesAsync();

            return new DiscountReadDto
            {
                DiscountId = discount.DiscountId,
                BookId = discount.BookId,
                BookName = book.BookName,
                Percentage = discount.Percentage,
                OnSale = discount.OnSale,
                StartDate = discount.StartDate,
                EndDate = discount.EndDate,
                IsActive = discount.IsActive
            };
        }

        public async Task<GenericResponse<DiscountReadDto?>> UpdateAsync(Guid id, DiscountUpdateDto dto)
        {
            var discount = await _discountRepo.GetByIdAsync(id);
            if (discount == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Discount not found");

            if (dto.StartDate >= dto.EndDate)
                return new ErrorModel(HttpStatusCode.BadRequest, "Start date must be before end date");

            discount.Percentage = dto.Percentage;
            discount.OnSale = dto.OnSale;
            discount.StartDate = dto.StartDate;
            discount.EndDate = dto.EndDate;

            _discountRepo.Update(discount);
            await _discountRepo.SaveChangesAsync();

            var book = await _bookRepo.GetByIdAsync(discount.BookId);

            return new DiscountReadDto
            {
                DiscountId = discount.DiscountId,
                BookId = discount.BookId,
                BookName = book.BookName,
                Percentage = discount.Percentage,
                OnSale = discount.OnSale,
                StartDate = discount.StartDate,
                EndDate = discount.EndDate,
                IsActive = discount.IsActive
            };
        }

        public async Task<Response> DeleteAsync(Guid id)
        {
            var discount = await _discountRepo.GetByIdAsync(id);
            if (discount == null)
                return new ErrorModel(HttpStatusCode.NotFound, "Discount not found");

            _discountRepo.Delete(discount);
            await _discountRepo.SaveChangesAsync();
            return new Response();
        }

        public async Task<GenericResponse<IEnumerable<DiscountReadDto>>> GetActiveDiscountsAsync()
        {
            var discounts = await _discountRepo.TableNoTracking
                .Include(d => d.Book)
                .Where(d => d.IsActive)
                .ToListAsync();

            return discounts.Select(d => new DiscountReadDto
            {
                DiscountId = d.DiscountId,
                BookId = d.BookId,
                BookName = d.Book.BookName,
                Percentage = d.Percentage,
                OnSale = d.OnSale,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                IsActive = d.IsActive
            }).ToList();
        }

        public async Task<GenericResponse<IEnumerable<DiscountReadDto>>> GetOnSaleDiscountsAsync()
        {
            var discounts = await _discountRepo.TableNoTracking
                .Include(d => d.Book)
                .Where(d => d.IsActive && d.OnSale)
                .ToListAsync();

            return discounts.Select(d => new DiscountReadDto
            {
                DiscountId = d.DiscountId,
                BookId = d.BookId,
                BookName = d.Book.BookName,
                Percentage = d.Percentage,
                OnSale = d.OnSale,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                IsActive = d.IsActive
            }).ToList();
        }
    }
}