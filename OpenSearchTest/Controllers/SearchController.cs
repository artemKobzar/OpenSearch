using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenSearchTest.Interfaces;
using OpenSearchTest.Services;

namespace OpenSearchTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;
        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("searchByNumber")]
        public async Task<IActionResult> SearchByNumber([FromQuery] string? number, int pageNumber)
        {
            var result = await _searchService.SearchByNumber(number, pageNumber);
            return Ok(result.Documents);
        }

        [HttpGet("searchByText")]
        public async Task<IActionResult> SearchByText([FromQuery] string? text, int pageNumber)
        {
            var result = await _searchService.SearchByText(text, pageNumber);
            return Ok(result.Documents);
        }

        [HttpGet("fullSearchData")]
        public async Task<IActionResult> FullSearchData([FromQuery] string? text, int? year, int? month, int? day, int pageNumber)
        {
            var result = await _searchService.FullSearchData(text, year, month, day, pageNumber);
            return Ok(result.Documents);
        }
    }
}
