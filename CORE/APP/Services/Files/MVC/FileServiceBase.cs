using CORE.APP.Models;
using Microsoft.AspNetCore.Http;

namespace CORE.APP.Services.Files.MVC;
public abstract class FileServiceBase : ServiceBase
    {
       
        public virtual string GetFilePath(IFormFile formFile, string filesFolder = "files")
        {
            if (formFile is null || formFile.Length == 0)
                return null;
            var fileExtension = Path.GetExtension(formFile.FileName);
            var fileName = $"{Guid.NewGuid().ToString()}{fileExtension}";
            return $"/{filesFolder}/{fileName}";
        }
        
        public virtual CommandResponse SaveFile(IFormFile formFile, string filePath, double maximumFileSizeInMb = 2, string fileExtensions = "jpg,jpeg,png")
        {
            if (formFile is not null && formFile.Length > 0 && !string.IsNullOrWhiteSpace(filePath))
            {
                var allowedMaximumFileSizeInMb = maximumFileSizeInMb * Math.Pow(1024, 2);
                if (formFile.Length > allowedMaximumFileSizeInMb)
                    return Error($"File size can't exceed {maximumFileSizeInMb.ToString("N1")} megabytes!");
                var formFileExtension = Path.GetExtension(formFile.FileName).TrimStart('.').ToLower();
                var allowedFileExtensions = fileExtensions.Split(',').Select(fileExtension => fileExtension.ToLower());
                if (!allowedFileExtensions.Contains(formFileExtension))
                    return Error($"Only {string.Join(", ", allowedFileExtensions)} file extensions are allowed!");
                using (var fileStream = new FileStream($"wwwroot/{filePath}", FileMode.Create))
                {
                    formFile.CopyTo(fileStream);
                }
            }
            return Success(string.Empty, 0);
        }

        public virtual void DeleteFile(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists($"wwwroot/{filePath}"))
                File.Delete($"wwwroot/{filePath}");
        }
    }
