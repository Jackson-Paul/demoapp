using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.Data;
using App.Models;
using System.Net.Http;
using System.IO;
using App.Services;

namespace App.Controllers
{
    public class ItemsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly SearchService _searchService;
        private readonly FileAccessService _fileAccessService;

        public ItemsController(AppDbContext context, SearchService searchService, FileAccessService fileAccessService)
        {
            _context = context;
            _searchService = searchService;
            _fileAccessService = fileAccessService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _context.Items.AsNoTracking().ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id.Value);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Item item)
        {
            if (!ModelState.IsValid) return View(item);
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Items.FindAsync(id.Value);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var itemToUpdate = await _context.Items.FindAsync(id);
            if (itemToUpdate == null) return NotFound();

            // Prevent overposting by specifying fields to update
            if (await TryUpdateModelAsync<Item>(itemToUpdate, "", i => i.Name, i => i.Description))
            {
                if (!ModelState.IsValid) return View(itemToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(itemToUpdate);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id.Value);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
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

        public async Task<IActionResult> SearchByFilter(string filter)
        {
            string query = "SELECT * FROM Items WHERE Name LIKE '%" + filter + "%'";
            var results = await _searchService.SearchItems(query);
            return Ok(results);
        }

        public async Task<IActionResult> Download(string filename)
        {
            byte[] processedContent = await _fileAccessService.GetFileContent(filename);
            return File(processedContent, "application/octet-stream", filename);
        }
    }
}
