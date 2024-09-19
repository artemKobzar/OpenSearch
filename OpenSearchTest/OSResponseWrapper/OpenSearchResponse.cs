using Newtonsoft.Json;

namespace OpenSearchTest.OSResponseWrapper
{
    public class OpenSearchResponse<T>
    {
        [JsonProperty("hits")]
        public HitsWrapper<T> Hits { get; set; }
    }
}
