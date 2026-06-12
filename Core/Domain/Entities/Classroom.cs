using Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Entities
{
    public class Classroom : BaseEntity
    {
        public string Name { get; set; }           
        public string Building { get; set; }
        public int Capacity { get; set; }
        public ClassroomType Type { get; set; }    
        public bool HasProjector { get; set; }
        public bool IsAvailable { get; set; }

        public List<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
