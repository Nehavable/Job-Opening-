using System;
namespace JobManagement.DTOs
{
    public class JobDetailsDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public LocationDto Location { get; set; }
        public DepartmentDto Department { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime? ClosingDate { get; set; }
    }

    public class LocationDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
    }

    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

}
