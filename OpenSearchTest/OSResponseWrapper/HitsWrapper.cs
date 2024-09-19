using Newtonsoft.Json;

namespace OpenSearchTest.OSResponseWrapper
{
    public class HitsWrapper<T>
    {
        [JsonProperty("hits")]
        public List<Hit<T>> Hits { get; set; }
    }
}
