using System;
using System.Threading.Tasks;
using GroupApi.CommonDomain;
using GroupApi.DTOs.Files;
using GroupApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GroupApi.Controllers.Files
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileStorageController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;

        public FileStorageController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorModel(HttpStatusCode.BadRequest, "Invalid input data"));

            var result = await _fileStorageService.UploadFileAsync(dto);
            return result.IsSuccess
                ? Ok(result)
                : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> GetFileMetadata(Guid fileId)
        {
            var result = await _fileStorageService.GetFileMetadataAsync(fileId);
            return result.IsSuccess
                ? Ok(result)
                : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFiles()
        {
            var result = await _fileStorageService.GetAllFilesAsync();
            return result.IsSuccess
                ? Ok(result)
                : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpDelete("{fileId}")]
        public async Task<IActionResult> DeleteFile(Guid fileId)
        {
            var result = await _fileStorageService.DeleteFileAsync(fileId);
            return result.IsSuccess
                ? Ok(result)
                : StatusCode((int)result.Error!.StatusCode, result);
        }

        [HttpGet("{fileId}/download")]
        public async Task<IActionResult> DownloadFile(Guid fileId)
        {
            var result = await _fileStorageService.DownloadFileAsync(fileId);
            if (!result.IsSuccess)
                return StatusCode((int)result.Error!.StatusCode, result);

            var metadataResult = await _fileStorageService.GetFileMetadataAsync(fileId);
            if (!metadataResult.IsSuccess)
                return StatusCode((int)metadataResult.Error!.StatusCode, metadataResult);

            return File(result.Data, metadataResult.Data.ContentType, metadataResult.Data.FileName);
        }
    }
}