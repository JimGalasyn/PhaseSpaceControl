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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

using _3DTools;

namespace PhaseSpaceControlLib
{
    [TemplatePart(Name = "PART_Trackport3D", Type = typeof(Trackport3D))]
    public class PhaseSpace3DControl2 : Control
    {
        //public ScreenSpaceLines3D ScreenSpaceLines { get; set; } = new ScreenSpaceLines3D();
        Trackport3D trackport3D;

        public List<double> Measurements { get; set; } = new List<double>();

        public double MinValue { get; set; }

        public double MaxValue { get; set; }

        public double Average { get; set; }

        static PhaseSpace3DControl2()
        {
            DefaultStyleKeyProperty.OverrideMetadata( 
                typeof( PhaseSpace3DControl2 ), 
                new FrameworkPropertyMetadata( typeof( PhaseSpace3DControl2 ) ) );
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //this.screenSpaceLines3D = this.Template.Resources["_screenSpaceLines3D"] as ScreenSpaceLines3D;
            this.trackport3D = GetTemplateChild( "PART_Trackport3D" ) as Trackport3D;

        }

        public String DataFilePath
        {
            get { return (String)GetValue(DataFilePathProperty); }
            set { SetValue(DataFilePathProperty, value); }
        }

        public static readonly DependencyProperty DataFilePathProperty =
            DependencyProperty.Register(
            "DataFilePath",
            typeof(String),
            typeof(PhaseSpace3DControl2),
            new PropertyMetadata(new PropertyChangedCallback(OnDataFilePathChanged)));


        private static void OnDataFilePathChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            PhaseSpace3DControl2 psc = d as PhaseSpace3DControl2;

            if (psc.DataFilePath != null)
            {
                psc.ReadData(e.NewValue as string);
            }
        }

        public int Phase
        {
            get { return (int)GetValue(PhaseProperty); }
            set { SetValue(PhaseProperty, value); }
        }

        public static readonly DependencyProperty PhaseProperty =
            DependencyProperty.Register(
            "Phase",
            typeof(int),
            typeof(PhaseSpace3DControl2),
            new PropertyMetadata(new PropertyChangedCallback(OnPhaseChanged)));

        private static void OnPhaseChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            PhaseSpace3DControl2 psc = d as PhaseSpace3DControl2;

            psc.PopulateScreenSpaceLines();
        }


        public double Gain
        {
            get { return (double)GetValue(GainProperty); }
            set { SetValue(GainProperty, value); }
        }


        public static readonly DependencyProperty GainProperty =
            DependencyProperty.Register(
            "Gain",
            typeof(double),
            typeof(PhaseSpace3DControl2),
            new UIPropertyMetadata(1.0d));

        private void ReadData(string fullPath)
        {
            if (File.Exists(fullPath))
            {
                this.MinValue = double.MaxValue;
                this.MaxValue = double.MinValue;

                using (StreamReader sr = new StreamReader(fullPath))
                {
                    string s;
                    double sum = 0;
                    int numSamples = 0;

                    while ((s = sr.ReadLine()) != null)
                    {
                        double measurement = double.Parse(s);

                        sum += measurement;
                        numSamples++;

                        this.Measurements.Add(measurement);

                        if (measurement < this.MinValue)
                        {
                            this.MinValue = measurement;
                        }

                        if (measurement > this.MaxValue)
                        {
                            this.MaxValue = measurement;
                        }
                    }

                    if (numSamples > 0)
                    {
                        this.Average = sum / numSamples;
                    }
                }
            }
        }



        private void PopulateScreenSpaceLines()
        {
            this.trackport3D.ScreenSpaceLines.Points = new Point3DCollection();

            for (int i = 2 * this.Phase + 1; i < this.Measurements.Count; i++)
            {
                double x = this.Gain * (this.Measurements[i - 1] - this.Average);
                double y = this.Gain * (this.Measurements[i - this.Phase - 1] - this.Average);
                double z = this.Gain * (this.Measurements[i - 2 * this.Phase - 1] - this.Average);

                Point3D p = new Point3D(x, y, z);

                this.trackport3D.ScreenSpaceLines.Points.Add(p);

                x = this.Gain * (this.Measurements[i] - this.Average);
                y = this.Gain * (this.Measurements[i - this.Phase] - this.Average);
                z = this.Gain * (this.Measurements[i - 2 * this.Phase] - this.Average);

                p = new Point3D(x, y, z);

                this.trackport3D.ScreenSpaceLines.Points.Add(p);

                //this.trackport3D.Model = ScreenSpaceLines.Content;
            }
        }
    }
}
