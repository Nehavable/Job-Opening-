using System.Text.Json.Serialization;

namespace JobManagement.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [JsonIgnore]
        public ICollection<Job> Jobs { get; set; }
    }
}
