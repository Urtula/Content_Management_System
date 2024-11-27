using CMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Application.DTO
{
    public class ContentDTO
    {
        public int Id { get; set; } 
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required string CategoryName { get; set; }
        public Languages ContentLanguage { get; set; }
        public ICollection<VariantDTO>? Variants { get; set; }

    }
}
