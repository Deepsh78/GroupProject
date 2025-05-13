using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GroupApi.CommonDomain;
using GroupApi.Data;
using GroupApi.DTOs.Files;
using GroupApi.Entities.Files;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GroupApi.Services.Files
{
    public class FileStorageService : IFileStorageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly string _storagePath;

        public FileStorageService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public async Task<GenericResponse<FileReadDto>> UploadFileAsync(FileUploadDto dto)
        {
            try
            {
                // Validate file
                if (dto.File == null || dto.File.Length == 0)
                {
                    return new ErrorModel(System.Net.HttpStatusCode.BadRequest, "No file provided.");
                }

                var maxFileSize = _configuration.GetValue<long>("FileStorage:MaxFileSize");
                var allowedTypes = _configuration.GetSection("FileStorage:AllowedTypes").Get<string[]>();

                if (dto.File.Length > maxFileSize)
                {
                    return new ErrorModel(System.Net.HttpStatusCode.BadRequest, $"File size exceeds the maximum limit of {maxFileSize / 1024 / 1024}MB.");
                }

                if (!allowedTypes.Contains(dto.File.ContentType))
                {
                    return new ErrorModel(System.Net.HttpStatusCode.BadRequest, "File type not allowed.");
                }

                // Generate unique file name
                var fileId = Guid.NewGuid();
                var fileExtension = Path.GetExtension(dto.File.FileName);
                var uniqueFileName = $"{fileId}{fileExtension}";
                var filePath = Path.Combine(_storagePath, uniqueFileName);

                // Save file to disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                // Create metadata
                var metadata = new FileMetadata
                {
                    FileId = fileId,
                    FileName = dto.File.FileName,
                    FilePath = filePath,
                    ContentType = dto.File.ContentType,
                    FileSize = dto.File.Length,
                    UploadedAt = DateTime.UtcNow
                };

                // Save to database
                _context.FileMetadatas.Add(metadata);
                await _context.SaveChangesAsync();

                // Map to DTO
                var fileReadDto = new FileReadDto
                {
                    FileId = metadata.FileId,
                    FileName = metadata.FileName,
                    ContentType = metadata.ContentType,
                    FileSize = metadata.FileSize,
                    UploadedAt = metadata.UploadedAt
                };

                return new GenericResponse<FileReadDto> { Data = fileReadDto };
            }
            catch (DbUpdateException ex)
            {
                return new ErrorModel(System.Net.HttpStatusCode.InternalServerError, $"Failed to upload file: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                return new ErrorModel(System.Net.HttpStatusCode.InternalServerError, $"Failed to upload file: {ex.Message}");
            }
        }

        public async Task<GenericResponse<FileReadDto>> GetFileMetadataAsync(Guid fileId)
        {
            var metadata = await _context.FileMetadatas
                .Where(f => f.FileId == fileId)
                .Select(f => new FileReadDto
                {
                    FileId = f.FileId,
                    FileName = f.FileName,
                    ContentType = f.ContentType,
                    FileSize = f.FileSize,
                    UploadedAt = f.UploadedAt
                })
                .FirstOrDefaultAsync();

            if (metadata == null)
            {
                return new ErrorModel(System.Net.HttpStatusCode.NotFound, "File not found.");
            }

            return new GenericResponse<FileReadDto> { Data = metadata };
        }

        public async Task<GenericResponse<IEnumerable<FileReadDto>>> GetAllFilesAsync()
        {
            var files = await _context.FileMetadatas
                .Select(f => new FileReadDto
                {
                    FileId = f.FileId,
                    FileName = f.FileName,
                    ContentType = f.ContentType,
                    FileSize = f.FileSize,
                    UploadedAt = f.UploadedAt
                })
                .ToListAsync();

            return new GenericResponse<IEnumerable<FileReadDto>> { Data = files };
        }

        public async Task<GenericResponse<bool>> DeleteFileAsync(Guid fileId)
        {
            var metadata = await _context.FileMetadatas.FindAsync(fileId);
            if (metadata == null)
            {
                return new ErrorModel(System.Net.HttpStatusCode.NotFound, "File not found.");
            }

            try
            {
                // Delete file from disk
                if (File.Exists(metadata.FilePath))
                {
                    File.Delete(metadata.FilePath);
                }

                // Remove metadata from database
                _context.FileMetadatas.Remove(metadata);
                await _context.SaveChangesAsync();

                return new GenericResponse<bool> { Data = true };
            }
            catch (Exception ex)
            {
                return new ErrorModel(System.Net.HttpStatusCode.InternalServerError, $"Failed to delete file: {ex.Message}");
            }
        }

        public async Task<GenericResponse<byte[]>> DownloadFileAsync(Guid fileId)
        {
            var metadata = await _context.FileMetadatas.FindAsync(fileId);
            if (metadata == null)
            {
                return new ErrorModel(System.Net.HttpStatusCode.NotFound, "File not found.");
            }

            if (!File.Exists(metadata.FilePath))
            {
                return new ErrorModel(System.Net.HttpStatusCode.NotFound, "File not found on disk.");
            }

            var fileBytes = await File.ReadAllBytesAsync(metadata.FilePath);
            return new GenericResponse<byte[]> { Data = fileBytes };
        }
    }
}