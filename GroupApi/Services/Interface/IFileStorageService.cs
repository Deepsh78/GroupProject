using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GroupApi.Services.Interface
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file);
        Task<byte[]?> GetFileAsync(string filePath);
        Task DeleteFileAsync(string filePath);
    }
}