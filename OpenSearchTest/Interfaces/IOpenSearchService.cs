namespace OpenSearchTest.Interfaces
{
    public interface IOpenSearchService
    {
        public Task UploadRecords(string path);
        public Task UploadChunkRecords(string path);
    }
}
