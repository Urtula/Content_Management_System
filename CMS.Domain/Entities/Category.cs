using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public required string CategoryName { get; set; }
        public string? CategoryDesc { get; set; }
        public ICollection<User>? Users { get; set; }
        public ICollection<Content>? Contents { get; set; }
    }
}
