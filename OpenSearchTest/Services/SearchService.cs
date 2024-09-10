using OpenSearch.Client;
using OpenSearchTest.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using OpenSearchTest.Interfaces;
using Microsoft.AspNetCore.Http;

namespace OpenSearchTest.Services
{
    public class SearchService: ISearchService
    {
        private readonly IOpenSearchClient _client;
        public SearchService(IOpenSearchClient client)
        {
            _client = client;
        }
        public async Task<ISearchResponse<SearchData>> SearchByNumber(string? number, int pageNumber)
        {
            if (!string.IsNullOrEmpty(number) && int.TryParse(number, out int numPrefix))
            {
                int lowerBound = numPrefix * (int)Math.Pow(10, number.Length - numPrefix.ToString().Length);
                int upperBound = (numPrefix + 1) * (int)Math.Pow(10, number.Length - numPrefix.ToString().Length);

                var response = await _client.SearchAsync<SearchData>(s => s
                .Index("search-data-index")
                .Query(q =>
                    q.Range(r => r
                        .Field(f => f.Number)
                        .GreaterThanOrEquals(lowerBound)
                        .LessThan(upperBound)
                    )
                )
                .From((pageNumber - 1) * 100)
                .Sort(s => s.Ascending(f => f.Id))
                .Size(100));
                //Console.WriteLine($"Lower Bound: {lowerBound}, Upper Bound: {upperBound}");
                return response;
            }

            else
            {
                return await _client.SearchAsync<SearchData>(s => s
                .Index("search-data-index")
                .Query(q =>
                    q.MatchAll())
                .From((pageNumber - 1) * 100)
                .Sort(s => s.Ascending(f => f.Id))
                .Size(100));
            }            
        }
        public async Task<ISearchResponse<SearchData>> SearchByText(string? text, int pageNumber)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var response = await _client.SearchAsync<SearchData>(s => s
                    .Index("search-data-index")
                    .Query(q => q
                        .MatchPhrasePrefix(mpp => mpp
                            .Field(f => f.Text)
                            .Query(text.ToLower())
                        )
                    )
                    .From((pageNumber - 1) * 100)
                    .Size(100)
                    .Sort(so => so.Ascending(f => f.Id))
                );

                return response;
            }
            else
            {
                return await _client.SearchAsync<SearchData>(s => s
                    .Index("search-data-index")
                    .Query(q => q.MatchAll())
                    .From((pageNumber - 1) * 100)
                    .Size(100)
                    .Sort(so => so.Ascending(f => f.Id))
                );
            }
        }
        public async Task<ISearchResponse<SearchData>> FullSearchData(string? text, int? year, int? month, int? day, int pageNumber)
        {
            var queryContainer = new QueryContainer();

            // Full-text search for the 'Text' field
            if (!string.IsNullOrEmpty(text))
            {
                queryContainer &= new MatchPhraseQuery
                {
                    Field = "text",
                    Query = text.ToLower()
                };
            }

            // Date search (exact date if provided)
            if (year.HasValue)
            {
                queryContainer &= new MatchQuery
                {
                    Field = "date.year",
                    Query = year.Value.ToString()  // This ensures we match the full day
                };
            }
            if (month.HasValue)
            {
                queryContainer &= new MatchQuery
                {
                    Field = "date.month",
                    Query = month.Value.ToString()  // This ensures we match the full day
                };
            }
            if (day.HasValue)
            {
                queryContainer &= new MatchQuery
                {
                    Field = "date.day",
                    Query = day.Value.ToString()  // This ensures we match the full day
                };
            }
            // Execute the search using the combined queries
            var response = await _client.SearchAsync<SearchData>(s => s
                .Index("search-data-index")
                .Query(q => queryContainer)
                .From((pageNumber - 1) * 100)
                .Size(100)
                .Sort(so => so.Ascending(f => f.Id)));

            return response;
        }
    }
}
