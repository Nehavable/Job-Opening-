

using JobManagement.Data;
using JobManagement.DTOs;
using JobManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobManagement.Controllers
{
    [ApiController]
    [Route("api/v1")]
    //[Route("api")]
    public class JobOpeningController : ControllerBase
    {
        private readonly AppDbContext _db;
        public JobOpeningController(AppDbContext db) => _db = db;

        // ---------------- Departments ----------------

        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            var list = await _db.Departments
                .Select(d => new { id = d.Id, title = d.Title, count = d.Jobs.Count })
                .ToListAsync();
            return Ok(list);
        }

        // POST /api/v1/departments
        [HttpPost("departments")]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Title)) return BadRequest("Title required");
            var dep = new Department { Title = dto.Title.Trim() };
            _db.Departments.Add(dep);
            await _db.SaveChangesAsync();
            return Created($"/api/v1/departments/{dep.Id}", new { id = dep.Id, title = dep.Title });
        }

        // PUT /api/v1/departments/{id}
        [HttpPut("departments/{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] DepartmentCreateDto dto)
        {
            var dep = await _db.Departments.FindAsync(id);
            if (dep == null) return NotFound();
            dep.Title = dto?.Title?.Trim() ?? dep.Title;
            await _db.SaveChangesAsync();
            return Ok();
        }

        // ---------------- Locations ----------------

        // GET /api/v1/locations
        [HttpGet("locations")]
        public async Task<IActionResult> GetLocations()
        {
            var list = await _db.Locations
                .Select(l => new { id = l.Id, title = l.Title, city = l.City, state = l.State, country = l.Country, zip = l.Zip, count = l.Jobs.Count })
                .ToListAsync();
            return Ok(list);
        }

        // POST /api/v1/locations
        [HttpPost("locations")]
        public async Task<IActionResult> CreateLocation([FromBody] LocationCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Title)) return BadRequest("Title required");
            var loc = new Location
            {
                Title = dto.Title.Trim(),
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                Zip = dto.Zip
            };
            _db.Locations.Add(loc);
            await _db.SaveChangesAsync();
            return Created($"/api/v1/locations/{loc.Id}", new { id = loc.Id, title = loc.Title });
        }

        [HttpPut("locations/{id}")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] LocationCreateDto dto)
        {
            var loc = await _db.Locations.FindAsync(id);
            if (loc == null) return NotFound();
            loc.Title = dto?.Title ?? loc.Title;
            loc.City = dto?.City ?? loc.City;
            loc.State = dto?.State ?? loc.State;
            loc.Country = dto?.Country ?? loc.Country;
            loc.Zip = dto?.Zip ?? loc.Zip;
            await _db.SaveChangesAsync();
            return Ok();
        }

        // ---------------- Jobs ----------------

        [HttpPost("jobs")]
        public async Task<IActionResult> CreateJob(JobCreateDto dto)
        {
            var department = await _db.Departments.FindAsync(dto.DepartmentId);
            if (department == null) return BadRequest("Invalid departmentId");

            var location = await _db.Locations.FindAsync(dto.LocationId);
            if (location == null) return BadRequest("Invalid locationId");

            var lastJob = await _db.Jobs
                .OrderByDescending(j => j.Id)
                .FirstOrDefaultAsync();

            string nextCode;
            if (lastJob == null)
            {
                nextCode = "JOB-01";
            }
            else
            {
                var lastNumber = int.Parse(lastJob.Code.Replace("JOB-", ""));
                nextCode = $"JOB-{(lastNumber + 1).ToString("D2")}";
            }

            var job = new Job
            {
                Title = dto.Title,
                Description = dto.Description,
                LocationId = dto.LocationId,
                DepartmentId = dto.DepartmentId,
                ClosingDate = dto.ClosingDate,
                Code = nextCode
            };

            _db.Jobs.Add(job);
            await _db.SaveChangesAsync();

            return Ok(job);
        }


        [HttpPut("jobs/{id}")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] JobUpdateDto dto)
        {
            var job = await _db.Jobs.FindAsync(id);
            if (job == null) return NotFound();

            if (!await _db.Departments.AnyAsync(d => d.Id == dto.DepartmentId)) return BadRequest("Invalid departmentId");
            if (!await _db.Locations.AnyAsync(l => l.Id == dto.LocationId)) return BadRequest("Invalid locationId");

            job.Title = dto.Title?.Trim() ?? job.Title;
            job.Description = dto.Description ?? job.Description;
            job.DepartmentId = dto.DepartmentId;
            job.LocationId = dto.LocationId;
            job.ClosingDate = dto.ClosingDate;

            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("jobs/{id}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var job = await _db.Jobs
                .Include(j => j.Department)
                .Include(j => j.Location)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null) return NotFound();

            var dto = new JobDetailsDto
            {
                Id = job.Id,
                Code = job.Code,
                Title = job.Title,
                Description = job.Description,
                PostedDate = job.PostedDate,
                ClosingDate = job.ClosingDate,
                Department = new DepartmentDto { Id = job.Department.Id, Title = job.Department.Title },
                Location = new LocationDto
                {
                    Id = job.Location.Id,
                    Title = job.Location.Title,
                    City = job.Location.City,
                    State = job.Location.State,
                    Country = job.Location.Country,
                    Zip = job.Location.Zip
                }
            };

            return Ok(dto);
        }

        [HttpPost("jobs/list")]
        public async Task<IActionResult> ListJobs([FromBody] JobListRequestDto req)
        {
            req = req ?? new JobListRequestDto();
            var q = _db.Jobs
                .Include(j => j.Location)
                .Include(j => j.Department)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(req.q))
            {
                var s = req.q.Trim().ToLower();
                q = q.Where(j =>
                    (j.Title ?? "").ToLower().Contains(s) ||
                    (j.Description ?? "").ToLower().Contains(s) ||
                    (j.Location.Title ?? "").ToLower().Contains(s) ||
                    (j.Department.Title ?? "").ToLower().Contains(s));
            }

            if (req.locationId.HasValue) q = q.Where(j => j.LocationId == req.locationId.Value);
            if (req.departmentId.HasValue) q = q.Where(j => j.DepartmentId == req.departmentId.Value);

            var total = await q.CountAsync();
            var pageNo = Math.Max(1, req.pageNo);
            var pageSize = Math.Clamp(req.pageSize, 1, 200);

            var items = await q
                .OrderByDescending(j => j.PostedDate)
                .Skip((pageNo - 1) * pageSize)
                .Take(pageSize)
                .Select(j => new JobListItemDto
                {
                    Id = j.Id,
                    Code = j.Code,
                    Title = j.Title,
                    Description = j.Description,
                    Location = j.Location.Title,
                    Department = j.Department.Title,
                    PostedDate = j.PostedDate,
                    ClosingDate = j.ClosingDate
                }).ToListAsync();

            return Ok(new JobListResponseDto { total = total, data = items });
        }

        [HttpDelete("jobs/{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = await _db.Jobs.FindAsync(id);
            if (job == null) return NotFound();
            _db.Jobs.Remove(job);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}

