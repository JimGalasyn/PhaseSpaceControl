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

namespace PhaseSpaceControlLib
{
    [TemplatePart( Name="PART_Polyline", Type = typeof( Polyline ) )]
    [TemplatePart( Name = "PART_Border", Type = typeof( Border ) )]
    [TemplatePart( Name = "PART_Grid", Type = typeof( Grid ) )]
    public class PhaseSpaceControl : Control
    {
        ///////////////////////////////////////////////////////////////////////
        #region Fields

        /// <summary>
        /// The name of the Polyline template part that traces the signal.
        /// </summary>
        private const string polylineName = "PART_Polyline";

        /// <summary>
        /// Reference to the Polyline declared in the control template.
        /// Assigned in the OnApplyTemplate method.
        /// </summary>
        private Polyline _signalPolyline;

        private List<double> _measurements;
        private double _minValue = double.MaxValue;
        private double _maxValue = double.MinValue;
        private double _average = 0;

        #endregion

        static PhaseSpaceControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata( 
                typeof( PhaseSpaceControl ), 
                new FrameworkPropertyMetadata( typeof( PhaseSpaceControl ) ) );
        }

        public PhaseSpaceControl()
        {
            this._measurements = new List<double>();
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


        public String DataFilePath
        {
            get { return (String)GetValue( DataFilePathProperty ); }
            set { SetValue( DataFilePathProperty, value ); }
        }
        
        public static readonly DependencyProperty DataFilePathProperty =
            DependencyProperty.Register( 
            "DatFilePath", 
            typeof( String ), 
            typeof( PhaseSpaceControl ),
            new PropertyMetadata( new PropertyChangedCallback( OnDataFilePathChanged ) ) );


        private static void OnDataFilePathChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e )
        {
            PhaseSpaceControl psc = d as PhaseSpaceControl;

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
            typeof( PhaseSpaceControl ),
            new PropertyMetadata( new PropertyChangedCallback( OnPhaseChanged ) ) );

        private static void OnPhaseChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e )
        {
            PhaseSpaceControl psc = d as PhaseSpaceControl;

            psc.PopulateScreenSpaceLines();
        }


        public double Gain
        {
            get { return (double)GetValue( GainProperty ); }
            set { SetValue( GainProperty, value ); }
        }

        // Using a DependencyProperty as the backing store for Gain.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GainProperty =
            DependencyProperty.Register( 
            "Gain", 
            typeof( double ), 
            typeof( PhaseSpaceControl ), 
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
            this._signalPolyline.Points = new PointCollection();

            double xCenter = this.ActualWidth / 2;
            double yCenter = this.ActualHeight / 2;

            for( int i = this.Phase; i < this._measurements.Count; i++ )
            {
                double x = this.Gain * ( this._measurements[i] - this._average )  + xCenter;
                double y = this.Gain * ( this._measurements[i - this.Phase] - this._average ) + yCenter;

                Point p = new Point( x, y );

                this._signalPolyline.Points.Add( p );
            }

            //        this._pl.Points.Clear();
            //this._pl.Points.Add( this._lastPoint );
            //this._pl.Stroke = this.signalBrush;

            //int deltaSamples = 5;
            //double amp = 3;



            //for( int i = deltaSamples; i < 1024 + deltaSamples; i++ )
            //{
            //    byte b1 = levels.GetWaveform( AudioChannel.Left, i );
            //    byte b0 = levels.GetWaveform( AudioChannel.Left, i - deltaSamples );

            //    //Point p = new Point((double)i, (double)(b+centerlineY));
            //    Point p = new Point( amp * (double)( b1 ), amp * (double)( b0 ) );

            //    this._pl.Points.Add( p );
            //}

            //this._lastPoint = this._pl.Points[this._pl.Points.Count - 1];


        }

        protected override void OnRender( DrawingContext drawingContext )
        {
            //if( this._signalPolyline.Points.Count == 0 )
            {
                this.PopulateScreenSpaceLines();
            }

            base.OnRender( drawingContext );
        }
       

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._signalPolyline = GetTemplateChild( polylineName ) as Polyline;
            

        }


    }
}
