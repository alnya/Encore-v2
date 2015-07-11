using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Encore.Domain.Entities
{
    public class Site : EntityBase
    {
        public string Name { get; set; }

        public string Type { get; set; }
    }
}
