using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamSystem.Models
{
    [Serializable]
    public class ErrorQUESTION
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Answer { get; set; }
        public int CategoryId { get; set; }
        public CATEGORY Category { get; set; }
        public List<CHOISE> Choises { get; set; }
    }
}
