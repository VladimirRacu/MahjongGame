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
using System.Windows.Shapes;

namespace MahjongGame
{
    /// <summary>
    /// Interaction logic for HelpWindow_Structure.xaml
    /// </summary>
    public partial class HelpWindow_Structure : Window
    {
        public HelpWindow_Structure()
        {
            InitializeComponent();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.Structure = "Square";
            MainWindow.newGameIndexes.Structure = 0;
            MainWindow.newStructureSelected = true;
            Close();
        }

        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.Structure = "Pyramid";
            MainWindow.newGameIndexes.Structure = 1;
            MainWindow.newStructureSelected = true;
            Close();
        }

        private void Image_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.Structure = "Bridge";
            MainWindow.newGameIndexes.Structure = 2;
            MainWindow.newStructureSelected = true;
            Close();
        }

        private void Image_MouseDown_3(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.Structure = "VenetianBridges";
            MainWindow.newGameIndexes.Structure = 3;
            MainWindow.newStructureSelected = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
