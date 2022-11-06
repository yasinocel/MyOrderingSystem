using My.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My.Domain.Models
{
    public class Report
    {
        public Guid Id { get; set; }
        public Order? Order { get; set; }
        public string? Details { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
