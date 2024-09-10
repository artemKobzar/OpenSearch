using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenSearchTest.Interfaces;
using OpenSearchTest.Services;

namespace OpenSearchTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpenSearchController : ControllerBase
    {
        private readonly IOpenSearchService _openSearchService;
        public OpenSearchController(IOpenSearchService openSearchService)
        {
            //string openSearchUri = "https://search-mytestdomain-5hba2rt273ckarshnxsegh2qye.aos.us-east-1.on.aws";
            //_openSearchService = new OpenSearchService(openSearchUri);
            _openSearchService = openSearchService;
        }
        [HttpPost("uploadCsv")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var path = Path.GetTempFileName();

            using(var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            await _openSearchService.UploadRecords(path);

            return Ok("File uploaded and data indexed successfully.");
        }
        [HttpPost("uploadCsvChunk")]
        public async Task<IActionResult> UploadCsvChunk(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }
            var path = Path.GetTempFileName();

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            await _openSearchService.UploadChunkRecords(path);

            return Ok("File uploaded and data indexed successfully.");
        }
    }
}
