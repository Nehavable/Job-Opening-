using System;
namespace JobManagement.DTOs
{
    public class JobListRequestDto
    {
        public string q { get; set; } = "";
        public int pageNo { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public int? locationId { get; set; }
        public int? departmentId { get; set; }
    }
}
