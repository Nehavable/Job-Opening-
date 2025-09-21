using System;

namespace JobManagement.DTOs
{
    public class JobListResponseDto
    {
        public int total { get; set; }
        public List<JobListItemDto> data { get; set; }
    }
}
