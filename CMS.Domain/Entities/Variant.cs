using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Entities
{
    public class Variant
    {
        public int Id { get; set; }
        public required string Language { get; set; }
        public required string ImageUrl { get; set; }
        public int ContentId { get; set; }
    }
}
