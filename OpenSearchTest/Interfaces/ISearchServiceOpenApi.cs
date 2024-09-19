
using OpenSearchTest.Models;

namespace OpenSearchTest.Interfaces
{
    public interface ISearchServiceOpenApi
    {
        Task<bool> CreateIndex(string indexName);
        Task<bool> DeleteIndex(string indexName);
        Task<List<SearchData>> FullSearchDataOpenApi(string? text, string? number, DateTime? startDate, DateTime? endDate, int pageNumber);
    }
}