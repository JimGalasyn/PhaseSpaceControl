using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Media.Media3D;
using System.Diagnostics;
using System.Windows.Markup;

namespace PhaseSpaceControlLib
{
    //[TemplatePart( Name = "PART_Trackport3D", Type = typeof( Trackport3D ) )] 
    [TemplatePart( Name = "PART_Viewport3D", Type = typeof( Viewport3D ) )]
    [TemplatePart( Name = "PART_ScreenSpaceLines3D", Type = typeof( ScreenSpaceLines3D ) )] 
    public class PhaseSpace3DControl : Control
    {
        static Model3D _sphereModel;

        private static string _sphereFileName = "sphere02.xaml";
        /// <summary>
        /// The name of the Trackport3D template part that renders the 3D content.
        /// </summary>
        private const string trackportName = "PART_Trackport3D";

        /// <summary>
        /// Reference to the Trackport3D declared in the control template.
        /// Assigned in the OnApplyTemplate method.
        /// </summary>
        private Trackport3D _trackport;

        private const string viewportName = "PART_Viewport3D";
        private Viewport3D _viewport;
        private RotateTransform3D _rotation;
        private RotateTransform3D _rotation_xAxis;
        private RotateTransform3D _rotation_yAxis;
        private RotateTransform3D _rotation_zAxis;


        private const string screenSpaceLinesName = "PART_ScreenSpaceLines3D";
        private ScreenSpaceLines3D _screenSpaceLines;

        private Canvas _poincareCanvas;
        private WriteableBitmap _poincareBitmap;
        private Window _poincareWindow;
        
        private List<double> _measurements;
        private double _minValue = double.MaxValue;
        private double _maxValue = double.MinValue;
        private double _average = 0;


        static PhaseSpace3DControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata( 
                typeof( PhaseSpace3DControl ), 
                new FrameworkPropertyMetadata( typeof( PhaseSpace3DControl ) ) );

            //LoadBlockModel();
        }

        public PhaseSpace3DControl()
        {
            this._measurements = new List<double>();
            //this._screenSpaceLines = new ScreenSpaceLines3D();
        }

        public double MinValue
        {
            get
            {
                return this._minValue;
            }
        }

        public double MaxValue
        {
            get
            {
                return this._maxValue;
            }
        }

        public double Average
        {
            get
            {
                return this._average;
            }
        }
        
        public double ViewAngle
        {
            get { return (double)GetValue( ViewAngleProperty ); }
            set { SetValue( ViewAngleProperty, value ); }
        }

        public static readonly DependencyProperty ViewAngleProperty =
            DependencyProperty.Register( 
            "ViewAngle", 
            typeof( double ), 
            typeof( PhaseSpace3DControl ),
            new PropertyMetadata( new PropertyChangedCallback( OnViewAngleChanged ) ) );


        private static void OnViewAngleChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e )
        {
            PhaseSpace3DControl psc = d as PhaseSpace3DControl;

            //RotateTransform3D transform = psc._screenSpaceLines.Transform as RotateTransform3D;
            AxisAngleRotation3D rotation = psc._rotation.Rotation as AxisAngleRotation3D;
            rotation.Angle = (double)e.NewValue;

            rotation = psc._rotation_xAxis.Rotation as AxisAngleRotation3D;
            rotation.Angle = (double)e.NewValue;

            rotation = psc._rotation_yAxis.Rotation as AxisAngleRotation3D;
            rotation.Angle = (double)e.NewValue;

            rotation = psc._rotation_zAxis.Rotation as AxisAngleRotation3D;
            rotation.Angle = (double)e.NewValue;
        }

        public String DataFilePath
        {
            get { return (String)GetValue( DataFilePathProperty ); }
            set { SetValue( DataFilePathProperty, value ); }
        }

        public static readonly DependencyProperty DataFilePathProperty =
            DependencyProperty.Register(
            "DataFilePath",
            typeof( String ),
            typeof( PhaseSpace3DControl ),
            new PropertyMetadata( new PropertyChangedCallback( OnDataFilePathChanged ) ) );


        private static void OnDataFilePathChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e )
        {
            PhaseSpace3DControl psc = d as PhaseSpace3DControl;

            if( psc.DataFilePath != null )
            {
                psc.ReadData( e.NewValue as string );
            }
        }

        public int Phase
        {
            get { return (int)GetValue( PhaseProperty ); }
            set { SetValue( PhaseProperty, value ); }
        }

        public static readonly DependencyProperty PhaseProperty =
            DependencyProperty.Register(
            "Phase",
            typeof( int ),
            typeof( PhaseSpace3DControl ),
            new PropertyMetadata( new PropertyChangedCallback( OnPhaseChanged ) ) );

        private static void OnPhaseChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e )
        {
            PhaseSpace3DControl psc = d as PhaseSpace3DControl;

            psc.PopulateScreenSpaceLines();
        }


        public double Gain
        {
            get { return (double)GetValue( GainProperty ); }
            set { SetValue( GainProperty, value ); }
        }

       
        public static readonly DependencyProperty GainProperty =
            DependencyProperty.Register(
            "Gain",
            typeof( double ),
            typeof( PhaseSpace3DControl ),
            new UIPropertyMetadata( 1.0d ) );

        private void ReadData( string fullPath )
        {
            if( File.Exists( fullPath ) )
            {
                this._minValue = double.MaxValue;
                this._maxValue = double.MinValue;

                using( StreamReader sr = new StreamReader( fullPath ) )
                {
                    string s;
                    double sum = 0;
                    int numSamples = 0;

                    while( ( s = sr.ReadLine() ) != null )
                    {
                        double measurement = double.Parse( s );

                        sum += measurement;
                        numSamples++;

                        this._measurements.Add( measurement );

                        if( measurement < this._minValue )
                        {
                            this._minValue = measurement;
                        }

                        if( measurement > this._maxValue )
                        {
                            this._maxValue = measurement;
                        }
                    }

                    if( numSamples > 0 )
                    {
                        this._average = sum / numSamples;
                    }
                }
            }
        }

        private void PopulateScreenSpaceLines()
        {
            this._screenSpaceLines.Points = new Point3DCollection();

            for( int i = 2 * this.Phase + 1; i < this._measurements.Count; i++ )
            {
                double x = this.Gain * ( this._measurements[i-1] - this._average );
                double y = this.Gain * ( this._measurements[i-this.Phase-1] - this._average );
                double z = this.Gain * ( this._measurements[i-2*this.Phase-1] - this._average );

                Point3D p = new Point3D( x, y, z );

                this._screenSpaceLines.Points.Add( p );

                x = this.Gain * ( this._measurements[i]-this._average );
                y = this.Gain * ( this._measurements[i-this.Phase] - this._average );
                z = this.Gain * ( this._measurements[i-2*this.Phase] - this._average );

                p = new Point3D( x, y, z );

                this._screenSpaceLines.Points.Add( p );
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._viewport = GetTemplateChild( viewportName ) as Viewport3D;
            this._screenSpaceLines = GetTemplateChild( screenSpaceLinesName ) as ScreenSpaceLines3D;
            this._rotation = GetTemplateChild("rotation") as RotateTransform3D;
            this._rotation_xAxis = GetTemplateChild( "rotation_xAxis" ) as RotateTransform3D;
            this._rotation_yAxis = GetTemplateChild( "rotation_yAxis" ) as RotateTransform3D;
            this._rotation_zAxis = GetTemplateChild( "rotation_zAxis" ) as RotateTransform3D;

            
            this._poincareWindow = this.Template.Resources["poincareWindow"] as Window;
            this._poincareCanvas = this._poincareWindow.Content as Canvas;

            //this._rotation = this.Template.Resources["rotation"] as RotateTransform3D;
            //this._rotation = this.Style.Resources["rotation"] as RotateTransform3D;
        }

        public Window PoincareMapZ
        {
            get
            {
                this._poincareCanvas.Children.Clear();

                for( int i = 1; i < this._screenSpaceLines.Points.Count; i++ )
                {
                    Point3D lastPoint = this._screenSpaceLines.Points[i - 1];
                    Point3D point = this._screenSpaceLines.Points[i];

                    if( ( lastPoint.Z > 0 && point.Z < 0 ) ||
                        ( lastPoint.Z < 0 && point.Z > 0 ) )
                    {
                        int zSign = Math.Sign( point.Z - lastPoint.Z );

                        Point newPoint = new Point( 
                            ( lastPoint.X - point.X ) / 2.0d, 
                            ( lastPoint.Y - point.Y ) / 2.0d );

                        Ellipse dot = new Ellipse();
                        dot.Height = 2.0;
                        dot.Width = 2.0;
                        dot.Stroke = ( zSign > 0 ) ? Brushes.Coral : Brushes.Yellow;
                        //dot.Stroke = Brushes.Coral;
                        Canvas.SetLeft( dot, 100.0d * lastPoint.X + this._poincareWindow.Width / 2.0d );
                        Canvas.SetTop( dot, 100.0d * lastPoint.Y + this._poincareWindow.Height / 2.0d );
                        

                        this._poincareCanvas.Children.Add( dot );
                    }

                }

                return this._poincareWindow;
            }
        }


        public Window PoincareMapX
        {
            get
            {
                this._poincareCanvas.Children.Clear();

                //int width = Math.Abs( (int)this._maxValue ) + Math.Abs( (int)this._minValue );
                //int height = width;

                //this._poincareBitmap = new WriteableBitmap( width, height, 96, 96, PixelFormats.Bgra32, null );

                //Image bitmapImage = this._poincareCanvas.Children[0] as Image;

                //bitmapImage.Source = this._poincareBitmap;


                //int stride = width * ( ( this._poincareBitmap.Format.BitsPerPixel + 7 ) / 8 );
                //int arraySize = stride * height;
                //byte[] pixels = new byte[arraySize];
                //int index = 0;

            
                for( int i = 1; i < this._screenSpaceLines.Points.Count; i++ )
                {
                    Point3D lastPoint = this._screenSpaceLines.Points[i - 1];
                    Point3D point = this._screenSpaceLines.Points[i];

                    if( ( lastPoint.X > 0 && point.X < 0 ) ||
                        ( lastPoint.X < 0 && point.X > 0 ) )
                    {
                        int xSign = Math.Sign( point.X - lastPoint.X );

                        Point newPoint = new Point(
                            ( lastPoint.Y - point.Y ) / 2.0d,
                            ( lastPoint.Z - point.Z ) / 2.0d );

                        //this._poincareBitmap.

                        Ellipse dot = new Ellipse();
                        dot.Height = 1;
                        dot.Width = 1;
                        dot.Stroke = ( xSign > 0 ) ? Brushes.Coral : Brushes.Blue;
                        Canvas.SetLeft( dot, 50.0d * lastPoint.Y + this.ActualWidth / 2.0d );
                        Canvas.SetTop( dot, 50.0d * lastPoint.Z + this.ActualHeight / 2.0d );


                        this._poincareCanvas.Children.Add( dot );
                    }

                    //Int32Rect rect = new Int32Rect( 0, 0, this._poincareBitmap.PixelWidth, this._poincareBitmap.PixelHeight );
                    //this._poincareBitmap.WritePixels( rect, pixels, stride, 0 );

                }

                return this._poincareWindow;
            }
        }



        private Image CreateImage()
        {
            PixelFormat pf = PixelFormats.Bgr32;
            int size = (int)(Math.Abs( this._minValue ) + Math.Abs( this._maxValue ) );
            int width = size + 10;
            int height = size + 10;
            int rawStride = ( width * pf.BitsPerPixel + 7 ) / 8;
            byte[] rawImage = new byte[rawStride * height];

            // Initialize the image with data.
            Random value = new Random();
            value.NextBytes( rawImage );

            // Create a BitmapSource.
            BitmapSource bitmap = BitmapSource.Create( width, height,
                96, 96, pf, null,
                rawImage, rawStride );

            // Create an image element;
            Image myImage = new Image();
            myImage.Width = 200;
            // Set image source.
            myImage.Source = bitmap;

            return myImage;
        }

        private static void LoadBlockModel()
        {
            try
            {
                using( FileStream fs = File.OpenRead( _sphereFileName ) )
                {
                    _sphereModel = LoadModel( fs );
                    //Model._blockModel.Freeze();
                }
            }
            catch( Exception ex )
            {
                Trace.WriteLine( ex.Message );
                Trace.Assert( false );
            }
        }

        public static Model3D LoadModel( Stream fileStream )
        {
            return ( (Model3D)XamlReader.Load( fileStream ) );
        }

    }
}
