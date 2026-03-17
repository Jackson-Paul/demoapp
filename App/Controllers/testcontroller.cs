using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    public class testController : Controller
    {
        
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
    }

}