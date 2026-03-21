using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPortal.Domain.Models
{
    
    public class State
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<City> Cities { get; set; } = new List<City>();
    }

    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int StateId { get; set; }
        public State State { get; set; }
    }

}
