using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP_WPF.Models;

namespace TSP_WPF.ViewModels
{
    public class CityViewModel
    {
        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public CityViewModel(int id, double x, double y)
        {
            Id = id;
            X = x;
            Y = y;
        }
    }
}
