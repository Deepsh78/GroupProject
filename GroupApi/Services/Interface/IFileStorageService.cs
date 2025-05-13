using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GroupApi.CommonDomain;
using GroupApi.DTOs.Files;

namespace GroupApi.Services.Interface
{
    public interface IFileStorageService
    {
        Task<GenericResponse<FileReadDto>> UploadFileAsync(FileUploadDto dto);
        Task<GenericResponse<FileReadDto>> GetFileMetadataAsync(Guid fileId);
        Task<GenericResponse<IEnumerable<FileReadDto>>> GetAllFilesAsync();
        Task<GenericResponse<bool>> DeleteFileAsync(Guid fileId);
        Task<GenericResponse<byte[]>> DownloadFileAsync(Guid fileId);
    }
}