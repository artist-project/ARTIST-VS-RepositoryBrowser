using System.Collections.Generic;
using Newtonsoft.Json;

namespace Spikes.Spikes_ArtistRepositoryPlugin.WebserviceClient
{
    public class ArProject
    {
        public string id;
        public string label;
        public string description;
        public IEnumerable<ArPackage> packages;
    }

    public class ArPackage
    {
        public string id;
        public string label;
        //public IEnumerable<ArArtifact> artifacts;
    }

    public class ArArtifact
    {
        public string project;
        public string packageId;

        public string id;
        public string label;
        public string description;
        public IEnumerable<string> tags;
        public IEnumerable<string> categories;
        public ArUser publisher;

        [JsonProperty("published-on")]
        public long published_on { get; set; }

        public ArUser modifier;

        [JsonProperty("last-modified")]
        public long last_modified { get; set; }

        public ArRating rating;
        public ArType type;

        [JsonProperty("average-rating")]
        public double average_rating { get; set; }
    }

    public class ArType
    {
        public string id;
        public string label;
        public string description;
        [JsonProperty("sub-categories")]
        public IEnumerable<string> sub_categories { get; set; }
    }

    public class ArRating
    {
        public int rating;
        public long date;
        public double averageRating;
        public int sampleSize;
    }

    public class ArUser
    {
        public string username;
        public string realname;
    }

    public class OAuthToken
    {
        public int expires_in;
        public string refresh_token;
        public string access_token;
    }
}
