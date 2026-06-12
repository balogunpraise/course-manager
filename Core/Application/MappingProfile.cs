using AutoMapper;
using Core.Application.Dtos;
using Core.Application.Dtos.Requests;
using Core.Domain.Entities;

namespace Core.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Schedule, ScheduleDto>()
                .ForMember(dest => dest.CourseCode, opt => opt.MapFrom(src => src.Course.CourseCode))
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course.Title))
                .ForMember(dest => dest.LecturerName, opt => opt.MapFrom(src => $"{src.Lecturer.FirstName} {src.Lecturer.LastName}"))
                .ForMember(dest => dest.ClassroomName, opt => opt.MapFrom(src => src.Classroom.Name));

            CreateMap<CreateScheduleDto, Schedule>();
            CreateMap<UpdateScheduleDto, Schedule>();

            CreateMap<Classroom, ClassroomDto>()
                .ForMember(dest => dest.ScheduleCount, opt => opt.MapFrom(src => src.Schedules.Count(s => !s.IsDeleted)));

            CreateMap<CreateClassroomDto, Classroom>();
            CreateMap<UpdateClassroomDto, Classroom>();

            CreateMap<LecturerAvailability, LecturerAvailabilityDto>()
                .ForMember(dest => dest.LecturerName, opt => opt.MapFrom(src => $"{src.Lecturer.FirstName} {src.Lecturer.LastName}"));

            CreateMap<CreateLecturerAvailabilityDto, LecturerAvailability>();
            CreateMap<UpdateLecturerAvailabilityDto, LecturerAvailability>();
        }
    }
}
