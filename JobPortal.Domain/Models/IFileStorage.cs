using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JobPortal.Domain.Models;

public interface IFileStorage
{
    Task<string> SaveAsync(IFormFile file, string subFolder); // returns relative path under wwwroot
    void Delete(string relativePath);
}

