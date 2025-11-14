using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class RegisterRequestDto
    {
        public string email { get; set; }
        public string password { get; set; }
        public string fullName { get; set; }
    }
}