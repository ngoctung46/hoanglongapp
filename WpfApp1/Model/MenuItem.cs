using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Model
{
    public class MenuItem
    {
        public string Name { get; set; }
        public object Content { get; set; }
        public Thickness MarginRequirement { get; set; }
        public ScrollBarVisibility HorizontalScrollBarVisibilityRequirement { get; set; }
        public ScrollBarVisibility VerticalScrollBarVisibilityRequirement { get; set; }
    }
}