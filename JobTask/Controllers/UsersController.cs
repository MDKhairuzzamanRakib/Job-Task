using Bogus;
using JobTask.Data;
using JobTask.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace JobTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public UsersController(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            user.TimeStamp = DateTime.UtcNow;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _cache.Remove("users_cache"); // Clear cache
            return Ok(user);
        }

        [HttpPost("create-bulk-users")]
        public async Task<IActionResult> CreateBulkUsers([FromQuery] int userCount = 10)
        {
            var faker = new Faker<User>()
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.Age, f => f.Random.Int(18, 65))
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.TimeStamp, f => f.Date.Recent(10));

            var users = faker.Generate(userCount);

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
            _cache.Remove("users_cache");
            return Ok(new { Message = $"{userCount} users created successfully!" });
        }

        [HttpGet("fetch-users")]
        public async Task<IActionResult> FetchUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            // Try to get from cache
            if (!_cache.TryGetValue("users_cache", out List<User>? users))
            {
                users = await _context.Users.ToListAsync();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));

                _cache.Set("users_cache", users, cacheOptions);
            }

            // Pagination logic
            var totalRecords = users.Count;
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var pagedUsers = users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Return paginated result
            var result = new
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                Data = pagedUsers
            };

            return Ok(result);
        }

    }
}