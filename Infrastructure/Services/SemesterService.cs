using Core.Application.Dtos.Requests;
using Core.Application.Dtos.Responses;
using Core.Application.Interfaces.ServicesAbstractions;
using Core.Domain.Entities;
using Infrastructure.data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class SemesterService(ApplicationDbContext context) : ISemesterService
    {
        private readonly ApplicationDbContext _context = context;


        public async Task<BaseResponse<bool>> CreateAcademicSession(SessionRequest request)
        {
            var alreadyExistion = await _context.AcademicSessions.AnyAsync(x => x.SessionCode == request.SessionCode);
            if (alreadyExistion) return BaseResponse<bool>.Failure(message: "Session already exists");

            var session = new AcademicSession
            {
                SessionCode = request.SessionCode,
                CreatedAt = DateTime.UtcNow,
            };
            await _context.AcademicSessions.AddAsync(session);
            await _context.SaveChangesAsync();
            return BaseResponse<bool>.Success(200, "sucess", true);
        } 


        public async Task<BaseResponse<List<SessionResponse>>> GetAcademicSessions()
        {
            var sessions = await _context.AcademicSessions.OrderByDescending(x => x.CreatedAt).ToListAsync();
            var res = sessions?.Select(x => new SessionResponse
            {
                Id = x.Id,
                SessionCode = x.SessionCode,
            }).ToList() ?? [];
            return BaseResponse<List<SessionResponse>>.Success(200, "success", res);
        }
    }
}
