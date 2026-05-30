using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Entities
{
    public class Level : BaseEntity
    {
        public string LevelName { get; set; }
        public string LevelCode { get; set; }
    }
}
