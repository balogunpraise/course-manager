namespace Core.Application.Dtos.Responses
{
    public class GetLevelsResponse  
    {
        public Guid Id { get; set; }
        public string LevelName { get; set; }
        public string LevelCode { get; set; }
    }
}
