namespace OpenSearchTest.Interfaces
{
    public interface IUploadDataOpenSearchApi
    {
        Task UploadChunkRecords(string path);
    }
}