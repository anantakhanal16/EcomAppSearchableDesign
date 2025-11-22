using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;

namespace Application.Dtos
{
    public class UserDetailDto:User
    {
        public string useType { get; set; } 
    }
}
