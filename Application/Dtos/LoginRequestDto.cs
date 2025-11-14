using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class LoginRequestDto
    {
        [Required]
        public string email { get; set; }

        [Required]
        public string password { get; set; }
    }
}