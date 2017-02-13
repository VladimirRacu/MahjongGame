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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class HelpWindow_SetImages : Window
    {
        public HelpWindow_SetImages()
        {

            InitializeComponent();


        }

        private void ImageAntique_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.SetOfImages = "antiqueSet";
            MainWindow.newGameIndexes.SetOfImages = 0;
            MainWindow.newSetOfPiecesSelected = true;
            Close();
        }

        private void ImageClassic_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.SetOfImages = "classicSet";
            MainWindow.newGameIndexes.SetOfImages = 1;
            MainWindow.newSetOfPiecesSelected = true;
            Close();
        }

        private void ImageGOT_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.SetOfImages = "gameOfThronesSet";
            MainWindow.newGameIndexes.SetOfImages = 2;
            MainWindow.newSetOfPiecesSelected = true;
            Close();
        }

        private void ImageClassicWooden_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.SetOfImages = "classicWoodenSet";
            MainWindow.newGameIndexes.SetOfImages = 3;
            MainWindow.newSetOfPiecesSelected = true;
            Close();
        }
        private void ImageChinaPlayingCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.SetOfImages = "chinesePlayingCardSet";
            MainWindow.newGameIndexes.SetOfImages = 4;
            MainWindow.newSetOfPiecesSelected = true;
            Close();
        }
        private void ImageSpainPlayingCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.SetOfImages = "spanishPlayingCardSet";
            MainWindow.newGameIndexes.SetOfImages = 5;
            MainWindow.newSetOfPiecesSelected = true;
            Close();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }



    }
}
