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
    /// Interaction logic for HelpWindow_Backgrounds.xaml
    /// </summary>
    public partial class HelpWindow_Backgrounds : Window
    {
        public HelpWindow_Backgrounds()
        {
            InitializeComponent();
        }

        private void background1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.BackgroundCode = "Standard.jpg";
            MainWindow.newGameIndexes.BackgroundCode = 0;
            MainWindow.newBackgroundSelected = true;
            Close();
        }

        private void background2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.BackgroundCode = "Chinese_Bamboo.jpg";
            MainWindow.newGameIndexes.BackgroundCode = 1;
            MainWindow.newBackgroundSelected = true;
            Close();
        }

        private void background3_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.BackgroundCode = "Cartoon_Bridge.jpg";
            MainWindow.newGameIndexes.BackgroundCode = 2;
            MainWindow.newBackgroundSelected = true;
            Close();
        }

        private void background4_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.BackgroundCode = "Lake_and_pagoda.jpg";
            MainWindow.newGameIndexes.BackgroundCode = 3;
            MainWindow.newBackgroundSelected = true;
            Close();
        }

        private void background5_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.BackgroundCode = "Game_of_thrones.jpg";
            MainWindow.newGameIndexes.BackgroundCode = 4;
            MainWindow.newBackgroundSelected = true;
            Close();
        }
        private void background6_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.BackgroundCode = "Leaves.jpg";
            MainWindow.newGameIndexes.BackgroundCode = 5;
            MainWindow.newBackgroundSelected = true;
            Close();
        }
        private void background7_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.BackgroundCode = "Fantastic_Winter.jpg";
            MainWindow.newGameIndexes.BackgroundCode = 6;
            MainWindow.newBackgroundSelected = true;
            Close();
        }
        private void background8_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.BackgroundCode = "Green_Place.jpg";
            MainWindow.newGameIndexes.BackgroundCode = 7;
            MainWindow.newBackgroundSelected = true;
            Close();
        }
        private void background9_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.BackgroundCode = "Chinese_Paper.jpg";
            MainWindow.newGameIndexes.BackgroundCode = 8;
            MainWindow.newBackgroundSelected = true;
            Close();
        }
        private void background10_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.BackgroundCode = "Rest_Place.jpg";
            MainWindow.newGameIndexes.BackgroundCode = 9;
            MainWindow.newBackgroundSelected = true;
            Close();
        }
        private void background11_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.BackgroundCode = "Chinese_Parchment.jpg";
            MainWindow.newGameIndexes.BackgroundCode = 10;
            MainWindow.newBackgroundSelected = true;
            Close();
        }
        private void background12_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.newGame.BackgroundCode = "Nature.jpg";
            MainWindow.newGameIndexes.BackgroundCode = 11;
            MainWindow.newBackgroundSelected = true;
            Close();
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
