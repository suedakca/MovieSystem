using Microsoft.AspNetCore.Http;

namespace CORE.APP.Models.Files;

public interface IFileRequest
{
    public IFormFile FormFile { get; set; }
}