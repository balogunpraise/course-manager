namespace Core.Application.Dtos.Responses
{
    public class LogingResponse
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
    }

    public class GetUserResponse
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Roles { get; set; }
    }
}
