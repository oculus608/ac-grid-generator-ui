using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcGridGeneratorUi
{
    public class Car
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Car(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
