using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Domain.Enums;

namespace CMS.Domain.Entities
{
    public class Content
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
        public Languages ContentLanguage { get; set; }
        public required ICollection<Variant> Variants { get; set; }
    }
}
