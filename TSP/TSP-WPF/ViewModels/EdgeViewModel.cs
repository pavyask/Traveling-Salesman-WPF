using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSP_WPF.Models;

namespace TSP_WPF.ViewModels
{
    public class EdgeViewModel
    {
        private readonly CityViewModel _city1;
        private readonly CityViewModel _city2;

        public double X1 => _city1.X;
        public double Y1 => _city1.Y;
        public double X2 => _city2.X;
        public double Y2 => _city2.Y;

        public EdgeViewModel(CityViewModel city1, CityViewModel city2)
        {
            _city1 = city1;
            _city2 = city2;
        }
    }
}
