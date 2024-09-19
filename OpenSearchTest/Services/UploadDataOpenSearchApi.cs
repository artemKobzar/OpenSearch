using CsvHelper;
using Newtonsoft.Json;
using OpenSearchTest.Interfaces;
using OpenSearchTest.Models;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;

namespace OpenSearchTest.Services
{
    public class UploadDataOpenSearchApi : IUploadDataOpenSearchApi
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;
        private readonly string _userName;
        private readonly string _password;
        private const int BatchSize = 1000;
        public UploadDataOpenSearchApi(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _url = configuration["OpenSearch:Url"];
            _userName = configuration["OpenSearch:Username"];
            _password = configuration["OpenSearch:Password"];

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_userName}:{_password}"));
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
        }
        public async Task UploadChunkRecords(string path)
        {
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = new List<SearchData>();

                while (await csv.ReadAsync())
                {
                    var record = csv.GetRecord<SearchData>();
                    records.Add(record);

                    if (records.Count > BatchSize)
                    {
                        await UploadBulkChunk(records);
                        records.Clear();
                    }
                }
                if (records.Any())
                {
                    await UploadBulkChunk(records);
                }
            }
        }
        private async Task UploadBulkChunk(List<SearchData> records)
        {
            var bulkJson = BuildBulkJson(records);
            var content = new StringContent(bulkJson, Encoding.UTF8, "application/json");
            var requestUri = $"{_url}/_bulk";

            try
            {
                var response = await _httpClient.PostAsync(requestUri, content);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Chunk has been uploaded successfully");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error uploading chunk: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception uploading chunk: {ex.Message}");
            }
        }
        private string BuildBulkJson(List<SearchData> records)
        {
            var sb = new StringBuilder();
            foreach (var record in records)
            {
                var actionMetaData = new
                {
                    index = new
                    {
                        _index = "search-data-index"
                    }
                };
                sb.AppendLine(JsonConvert.SerializeObject(actionMetaData));
                sb.AppendLine(JsonConvert.SerializeObject(record));
            }
            return sb.ToString();
        }
    }
}
