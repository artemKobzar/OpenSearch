using Newtonsoft.Json;
using OpenSearch.Client;
using OpenSearchTest.Interfaces;
using OpenSearchTest.Models;
using OpenSearchTest.OSResponseWrapper;
using System.Net;
using System.Text;

namespace OpenSearchTest.Services
{
    public class SearchServiceOpenApi : ISearchServiceOpenApi
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;
        private readonly string _userName;
        private readonly string _password;
        public SearchServiceOpenApi(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            _url = configuration["OpenSearch:Url"];
            _userName = configuration["OpenSearch:Username"];
            _password = configuration["OpenSearch:Password"];
            Console.WriteLine($"URL: {_url}, Username: {_userName}");
        }

        public async Task<bool> DeleteIndex(string indexName)
        {
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_userName}:{_password}"));

            var requestUri = $"{_url}/{indexName}";
            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
            try
            {
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Exeption deleting index: {errorContent}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exeption deleting index: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> CreateIndex(string indexName)
        {
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_userName}:{_password}"));
            var requestUri = $"{_url}/{indexName}";
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

            var mappingJson = @"
            {
                ""mappings"": {
                    ""properties"": {
                        ""Id"": { ""type"": ""keyword"" },
                        ""Number"": { ""type"": ""text"" },
                        ""Text"": { ""type"": ""text"" },
                        ""Date"": { 
                            ""type"": ""date"", 
                            ""format"": ""yyyy-MM-dd""
                        }
                    }
                }
            }";
            var content = new StringContent(mappingJson, Encoding.UTF8, "application/json");
            try
            {
                var response = await _httpClient.PutAsync(requestUri, content);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error creating index: {errorContent}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception creating index: {ex.Message}");
                return false;
            }
        }
        public async Task<List<SearchData>> FullSearchDataOpenApi(string? text, string? number, DateTime? startDate, DateTime? endDate, int pageNumber)
        {
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_userName}:{_password}"));
            var requestUri = $"{_url}/search-data-index/_search";
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

            var queryJson = new
            {
                from = (pageNumber - 1) * 100,
                size = 100,
                sort = new[] { new { Id = new { order = "asc" } } },
                query = new
                {
                    @bool = new
                    {
                        must = new List<object>()
                    }
                }
            };

            if (!string.IsNullOrEmpty(text))
            {
                queryJson.query.@bool.must.Add(new
                {
                    match_phrase = new
                    {
                        Text = text.ToLower()
                    }
                });
            }
            if (!string.IsNullOrEmpty(number))
            {
                queryJson.query.@bool.must.Add(new
                {
                    match = new
                    {
                        Number = new
                        {
                            query = number
                        }
                    }
                });
            }
            if (startDate.HasValue || endDate.HasValue)
            {
                var range = new Dictionary<string, string>();

                if (startDate.HasValue)
                {
                    range.Add("gte", startDate.Value.ToString("yyyy-MM-dd"));
                }
                if (endDate.HasValue)
                {
                    range.Add("lte", endDate.Value.ToString("yyyy-MM-dd"));
                }
                queryJson.query.@bool.must.Add(new
                {
                    range = new
                    {
                        Date = range
                    }
                });
            }

            var jsonPayload = JsonConvert.SerializeObject(queryJson);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            try
            {
                var response = await _httpClient.PostAsync(requestUri, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var searchResponse = JsonConvert.DeserializeObject<OpenSearchResponse<SearchData>>(responseContent);

                    return searchResponse?.Hits?.Hits.Select(h => h.Source).ToList() ?? new List<SearchData>();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to search data: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception searching data: {ex.Message}");
                return null;
            }
        }
    }
}