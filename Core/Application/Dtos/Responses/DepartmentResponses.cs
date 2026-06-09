namespace Core.Application.Dtos.Responses
{
    public class GetDepartmentsResponse 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid FacultyId { get; set; }
    }

    public class GetFacultiesResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
