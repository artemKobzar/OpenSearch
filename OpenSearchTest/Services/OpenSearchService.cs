using CsvHelper;
using CsvHelper.Configuration;
using OpenSearch.Client;
using OpenSearchTest.Interfaces;
using OpenSearchTest.Models;
using System.Globalization;
using System.Text.Json;

namespace OpenSearchTest.Services
{
    public class OpenSearchService: IOpenSearchService
    {
        private readonly IOpenSearchClient _client;
        private const int BatchSize = 1000;
        public OpenSearchService(IOpenSearchClient client)
        {
            _client = client;
        }
        public async Task UploadRecords(string path)
        {
            var indexExists = await _client.Indices.ExistsAsync("search-data-index");
            if(!indexExists.Exists)
            {
                var createIndexResponse = await _client.Indices.CreateAsync("search-data-index", 
                    c => c.Map<SearchData>(m => m.AutoMap()));

                if(!createIndexResponse.IsValid)
                {
                    Console.WriteLine($"Failed to create index: {createIndexResponse.ServerError.Error.Reason}");
                }
            }

            var records = ReadCsvFile(path);

            var bulkResponse = await _client.BulkAsync(b => b.Index("search-data-index").IndexMany(records));
            if (bulkResponse.Errors)
            {
                Console.WriteLine("Failed to upload some records.");
            }
            else
            {
                Console.WriteLine("All records uploaded successfully.");
            }
        }
        private IEnumerable<SearchData> ReadCsvFile(string path)
        {
            using (var reader = new StreamReader(path))
            using(var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<SearchData>().ToList();
            }
        }

        public async Task UploadChunkRecords(string path)
        {
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = new List<SearchData>();
                long currentSize = 0;
                var maxBulkSize = 10 * 1024 * 1024;

                while(await csv.ReadAsync())
                {
                    var record = csv.GetRecord<SearchData>();
                    records.Add(record);

                    if(records.Count > BatchSize)
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
            var bulkResponse = await _client.BulkAsync(b => b.Index("search-data-index").IndexMany(records));

            if (bulkResponse.Errors)
            {
                Console.WriteLine("Failed to upload some records.");
            }
            else
            {
                Console.WriteLine("All records uploaded successfully.");
            }
        }
    }
}
//var bulkDescriptor = new BulkDescriptor();

//foreach (var record in records)
//{
//    bulkDescriptor.Index<SearchData>(os => os.Document(record));
//}
//var response = await _client.BulkAsync(bulkDescriptor);
