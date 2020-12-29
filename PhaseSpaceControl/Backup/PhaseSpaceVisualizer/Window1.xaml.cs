using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhaseSpaceVisualizer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void button1_Click( object sender, RoutedEventArgs e )
        {
            if( this.phaseSpace3DControl.PoincareMapZ.IsVisible )
            {
                Window w = this.phaseSpace3DControl.PoincareMapZ;
                //Window w = this.phaseSpace3DControl.PoincareMapX;
            }
            else
            {
                this.phaseSpace3DControl.PoincareMapZ.Show();
            }
            
        }
    }
}
