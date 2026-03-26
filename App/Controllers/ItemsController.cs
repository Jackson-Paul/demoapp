using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.Models;
using System.Net.Http;
using System.IO;
using App.Services;

namespace App.Controllers
{
    public class ItemsController : Controller
    {
        private readonly SearchService _searchService;
        private readonly FileAccessService _fileAccessService;

        public ItemsController(SearchService searchService, FileAccessService fileAccessService)
        {
            _searchService = searchService;
            _fileAccessService = fileAccessService;
        }

        public IActionResult Retrieve(string path)
        {
            string basePath = "/app/data";
            string fullPath = Path.Combine(basePath, path);
            if (System.IO.File.Exists(fullPath))
            {
                string content = System.IO.File.ReadAllText(fullPath);
                return Ok(content);
            }
            return NotFound();
        }

        public async Task<IActionResult> Fetch(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    string content = await response.Content.ReadAsStringAsync();
                    return Ok(content);
                }
                catch
                {
                    return BadRequest("Failed to fetch");
                }
            }
        }

        public IActionResult Display(string message)
        {
            string html = "<h1>" + message + "</h1>";
            return Content(html, "text/html");
        }


        public async Task<IActionResult> Download(string filename)
        {
            byte[] processedContent = await _fileAccessService.GetFileContent(filename);
            return File(processedContent, "application/octet-stream", filename);
        }
    }
}
