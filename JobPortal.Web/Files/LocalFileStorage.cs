using System;
using System.IO;
using System.Threading.Tasks;
using JobPortal.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
 

namespace JobPortal.Web.Files
{

    public class LocalFileStorage : IFileStorage
    {
        private readonly string _wwwroot;

        public LocalFileStorage(IWebHostEnvironment env)
        {
            _wwwroot = env.WebRootPath;
        }

        public async Task<string> SaveAsync(IFormFile file, string subFolder)
        {
            Directory.CreateDirectory(Path.Combine(_wwwroot, subFolder));

            var safeName = Path.GetFileNameWithoutExtension(file.FileName);
            var ext = Path.GetExtension(file.FileName);
            var stamped = $"{safeName}_{Guid.NewGuid():N}{ext}";
            var relPath = Path.Combine(subFolder, stamped).Replace("\\", "/");
            var absPath = Path.Combine(_wwwroot, relPath);

            using (var stream = new FileStream(absPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return "/" + relPath; // for <img src> etc.
        }

        public void Delete(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;
            var abs = Path.Combine(_wwwroot, relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(abs)) File.Delete(abs);
        }
    }
}
