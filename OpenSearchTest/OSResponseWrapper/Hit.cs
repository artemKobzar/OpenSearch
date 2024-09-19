using Newtonsoft.Json;

namespace OpenSearchTest.OSResponseWrapper
{
    public class Hit<T>
    {
        [JsonProperty("_source")]
        public T Source { get; set; }
    }
}
