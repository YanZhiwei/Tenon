using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenon.FluentValidation.AspNetCore.Extensions.Models
{
    public class ValidationError
    {
        public int? Code { get; set; }

        public string Message { get; set; }
    }
}
