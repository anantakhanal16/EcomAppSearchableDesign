using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class UserLoginResponse
    {
        public string refreshToken { get; set; }
        public string accessToken { get; set; }
    }
}