using System.ComponentModel.DataAnnotations;

namespace Core.Application.Dtos.Requests
{
    public class SessionRequest
    {
        [Required]
        public string SessionCode { get; set; }
    }
}
