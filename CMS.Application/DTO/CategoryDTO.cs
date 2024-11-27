using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Application.DTO
{
    public class CategoryDTO
    {
        public required string CategoryName { get; set; }
        public string? CategoryDesc { get; set; }
        public ICollection<ContentDTO>? Contents { get; set; }

    }
}
