using OpenSearch.Client;
using OpenSearchTest.Models;

namespace OpenSearchTest.Interfaces
{
    public interface ISearchService
    {
        public Task<ISearchResponse<SearchData>> FullSearchData(string? text, int? year, int? month, int? day, int pageNumber);
        public Task<ISearchResponse<SearchData>> SearchByNumber(string? number, int pageNumber);
        public Task<ISearchResponse<SearchData>> SearchByText(string? text, int pageNumber);
    }
}
