using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Application.DTO
{
    public class UserDTO
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public List<int>? CategoryIDs { get; set; }
    }
}
