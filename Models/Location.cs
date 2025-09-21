using System.Text.Json.Serialization;

namespace JobManagement.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        [JsonIgnore]
        public ICollection<Job> Jobs { get; set; }
    }
}
