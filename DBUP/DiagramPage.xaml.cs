using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DBUP
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class DiagramPage : Page
    {
        public static double[] paramsBPTP_Diagram;
        public static double[] paramsBITP_Diagram;
        public static double[] paramsBTPPDn_Diagram;
        static Diagram diagram;
        public static Canvas canvasForSave;
        public DiagramPage()
        {
            InitializeComponent();
            canvasForSave = canvas;
            diagram = new Diagram(280, 280, 500, canvas);
            diagram.DrawEmptyDiagram();

            PrepareDataForDiagram();

            //paramsBPTP_Diagram = new double[] { 0.875, 0.625, 1, 1, 0.75, 0.8333, 0.875, 0.875, 1, 0.6875, 0.5833, 0.6667, 0.4166, 1, 0.75, 0.625, 0.625, 0.75, 1, 0.75, 0.75, 0.5625, 0.5625, 0.5, 0.75, 0.625, 0.75, 0.58, 0.625, 0.4, 0.75, 0.43, 0.75, 0.625 };
            //paramsBITP_Diagram = new double[] { 0.4, 0.525, 0.9, 0, 0.75, 0.8333, 0.875, 0.875, 1, 0.6875, 0.5833, 0.6667, 0.4166, 1, 0.75, 0.625, 0.625, 0.75, 1, 0.75, 0.75, 0.5625, 0.5625, 0.5, 0.75, 0.625, 0.75, 0.58, 0.625, 0.4, 0.75, 0.43, 0.75, 0.625 };
            //paramsBTPPDn_Diagram = new double[] { 0.4, 0.525, 0.49, 0, 0.75, 0.8333, 0.875, 0.875, 1, 0.6875, 0.5833, 0.6667, 0.4166, 1, 0.75, 0.625, 0.625, 0.75, 1, 0.75, 0.75, 0.5625, 0.5625, 0.5, 0.75, 0.625, 0.75, 0.58, 0.625, 0.4, 0.75, 0.43, 0.75, 0.625 };
            //MainWindow.EV_1 = 0.453;
            //MainWindow.EV_2 = 0.89;
            //MainWindow.EV_3 = 0.22;
            //MainWindow.EV_R = 0.79;
        }

        private void rbDiagramGroup_Checked(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            stkGroup.IsEnabled = true;
            int index = -1;
            for (int i = 0; i < stkGroup.Children.Count; i++)
            {
                RadioButton r = (RadioButton)stkGroup.Children[i];
                if(r.IsChecked == true)
                    index = Int32.Parse(r.Tag.ToString());
            }

            switch (index)
            {
                case 0:
                    diagram.DrawGroupDiagram(paramsBPTP_Diagram);
                    break;
                case 1:
                    diagram.DrawGroupDiagram(paramsBITP_Diagram);
                   break;
                case 2:
                    diagram.DrawGroupDiagram(paramsBTPPDn_Diagram);
                    break;
                default:
                    diagram.DrawEmptyDiagram();
                    break;
            }
        }

        private void rbDiagramDirection_Checked(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            stkGroup.IsEnabled = false;
            diagram.DrawEvDiagram(new double[] { MainWindow.EV_1, MainWindow.EV_2, MainWindow.EV_3 });
        }

        private void rbDiagramResult_Checked(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            stkGroup.IsEnabled = false;
            diagram.DrawResultDiagram(MainWindow.EV_R);
        }

        public static void PrepareDataForDiagram()
        {
            List<double> paramsBPTP = new List<double>();
            List<double > paramsBITP = new List<double>();
            List<double> paramsBTPPDn = new List<double>();

            for (int i = 0; i < 6; i++)
            {
                paramsBPTP.Add(MainWindow.valueGroup[i].secondaryMi[0].value);
                paramsBITP.Add(MainWindow.valueGroup[i].secondaryMi[1].value);
                paramsBTPPDn.Add(MainWindow.valueGroup[i].secondaryMi[2].value);
            }
            for (int i = 6; i < 34; i++)
            {
                paramsBPTP.Add(MainWindow.valueGroup[i].value);
                paramsBITP.Add(MainWindow.valueGroup[i].value);
                paramsBTPPDn.Add(MainWindow.valueGroup[i].value);
            }

            paramsBPTP_Diagram = paramsBPTP.ToArray<double>();
            paramsBITP_Diagram = paramsBITP.ToArray<double>();
            paramsBTPPDn_Diagram = paramsBTPPDn.ToArray<double>();
        }
    }
}
