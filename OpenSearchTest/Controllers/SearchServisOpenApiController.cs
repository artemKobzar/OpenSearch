using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenSearchTest.Interfaces;
using OpenSearchTest.Services;

namespace OpenSearchTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchServisOpenApiController : ControllerBase
    {
        private readonly ISearchServiceOpenApi _searchServiceOpenApi;
        private readonly IUploadDataOpenSearchApi _uploadDataOpenApi;
        public SearchServisOpenApiController(ISearchServiceOpenApi searchServiceOpenApi, IUploadDataOpenSearchApi uploadDataOpenApi)
        {
            _searchServiceOpenApi = searchServiceOpenApi;
            _uploadDataOpenApi = uploadDataOpenApi;
        }

        [HttpDelete("delete-index/{indexName}")]
        public async Task<IActionResult> DeleteIndex(string indexName)
        {
            var isDeleted = await _searchServiceOpenApi.DeleteIndex(indexName);

            if (isDeleted)
            {
                return Ok($"Index {indexName} deleted successfully.");
            }
            else
            {
                return BadRequest($"Failed to delete index {indexName}. It may not exist or an error occurred.");
            }
        }
        [HttpPost("create-index/{indexName}")]
        public async Task<IActionResult> CreateIndex(string indexName)
        {
            var isCreated = await _searchServiceOpenApi.CreateIndex(indexName);

            if (isCreated)
            {
                return Ok($"Index {indexName} created successfully.");
            }
            else
            {
                return BadRequest($"Failed to create index {indexName}. An error occurred.");
            }
        }
        [HttpPost("bulkUpload")]
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
            await _uploadDataOpenApi.UploadChunkRecords(path);

            return Ok("File uploaded and data indexed successfully.");
        }
        [HttpGet("search-index")]
        public async Task<IActionResult> GetIndexData(string? text, string? number, DateTime? startDate, DateTime? endDate, int pageNumber)
        {
            try
            {
                var result = await _searchServiceOpenApi.FullSearchDataOpenApi(text, number, startDate, endDate, pageNumber);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching", error = ex.Message });
            }
        }
    }
}
