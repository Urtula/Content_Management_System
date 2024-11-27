using CMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Application.DTO
{
    public class VariantDTO
    {
        public required string Language { get; set; }
        public required string ImageUrl { get; set; }
        public int ContentId { get; set; }
    }
}
