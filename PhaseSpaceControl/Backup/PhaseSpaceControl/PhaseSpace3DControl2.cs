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

using _3DTools;

namespace PhaseSpaceControlLib
{
    public class PhaseSpace3DControl2 : Control
    {
        ScreenSpaceLines3D _screenSpaceLines3D;
        Trackport3D _trackport3D;


        static PhaseSpace3DControl2()
        {
            DefaultStyleKeyProperty.OverrideMetadata( 
                typeof( PhaseSpace3DControl2 ), 
                new FrameworkPropertyMetadata( typeof( PhaseSpace3DControl2 ) ) );
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._screenSpaceLines3D = this.Template.Resources["_screenSpaceLines3D"] as ScreenSpaceLines3D;
            this._trackport3D = GetTemplateChild( "PART_Trackport3D" ) as Trackport3D;

        }
    }
}
