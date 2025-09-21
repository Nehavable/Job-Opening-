using System;

namespace JobManagement.DTOs
{
    public class JobCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int LocationId { get; set; }
        public int DepartmentId { get; set; }
        public DateTime? ClosingDate { get; set; }
        //public string Code { get; set; }
    }
}
