using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Application.Interfaces.ServicesAbstractions;
using Infrastructure.data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Services
{
    public class LevelService(ApplicationDbContext context, ILogger<LevelService> logger) : ILevelService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger _logger = logger;

        public async Task<BaseResponse<List<GetLevelsResponse>>> GetLevels(GetLevelsRequests request)
        {
            var query = _context.Levels.AsQueryable();
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(x => x.LevelName.ToLower().Contains(request.SearchTerm.ToLower()) 
                || x.LevelCode.ToLower().Contains(request.SearchTerm.ToLower()));
            }
            var levels = await query.ToListAsync();

            var response = levels.Select(x => new GetLevelsResponse
            {
                Id = x.Id,
                LevelCode = x.LevelCode,
                LevelName = x.LevelName
            }).ToList();
            return BaseResponse<List<GetLevelsResponse>>.Success(200, "success", response);
        }
    }
}
