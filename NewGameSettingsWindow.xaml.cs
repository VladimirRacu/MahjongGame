using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MahjongGame
{
    /// <summary>
    /// Interaction logic for NewGameSettingsWindow.xaml
    /// </summary>

    public partial class NewGameSettingsWindow : Window
    {
        public GameSetting CurrentItem { get; set; }
        public GameSettingIndexes CurrentItemIndexes { get; set; }
        public string CurrentGameComplexity { get; set; }

        public NewGameSettingsWindow()
        {
            InitializeComponent();
            //CurrentItem = new GameSetting("Square", "Standard.jpg", "classicSet", 1);  
            helpBTN.Style = (Style)FindResource("helpBTNStyle");
            helpBTN2.Style = (Style)FindResource("helpBTNStyle");
            helpBTN3.Style = (Style)FindResource("helpBTNStyle");
            helpBTN4.Style = (Style)FindResource("helpBTNStyle");
            CurrentItem = MainWindow.newGame;
            CurrentItemIndexes = MainWindow.newGameIndexes;
            cbxListOfStructures.SelectedIndex = CurrentItemIndexes.Structure;
            cbxListOfBackgroundImages.SelectedIndex = CurrentItemIndexes.BackgroundCode;
            cbxListOfSetsPieces.SelectedIndex = CurrentItemIndexes.SetOfImages;
            if (CurrentItemIndexes.Complexity == 0) 
            {
                cbxGameComplexity.SelectedIndex = 0;
            }
            else 
            {
                cbxGameComplexity.SelectedIndex = CurrentItemIndexes.Complexity - 1;
            }
           
            ImageBrush imageBrush = new ImageBrush(MahjongGame.MainWindow.mainWindowBackgroundImage);
            imageBrush.Stretch = Stretch.None;
            this.Background = imageBrush;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnStartGame_Click(object sender, RoutedEventArgs e)
        {
            CurrentItem.Structure = cbxListOfStructures.SelectionBoxItem.ToString();
            CurrentItem.SetOfImages = cbxListOfSetsPieces.SelectionBoxItem.ToString();
            CurrentItem.BackgroundCode = cbxListOfBackgroundImages.SelectionBoxItem.ToString();
            CurrentItem.Complexity = cbxGameComplexity.SelectionBoxItem.ToString();

            MainWindow.newGameStarted = true;
            Close();
        }

        /////////////////////////////////////////
        ///BROWSE BUTTON EVENT HANDLERS///////////
        /////////////////////////////////////////


        private void helpBTN2_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow_SetImages newHelpWindow = new HelpWindow_SetImages();
            newHelpWindow.Owner = this;
            newHelpWindow.ShowDialog();
            if (MainWindow.newSetOfPiecesSelected)
            {
                cbxListOfSetsPieces.SelectedIndex = MainWindow.newGameIndexes.SetOfImages;
                CurrentItem.SetOfImages = cbxListOfSetsPieces.SelectionBoxItem.ToString();
                MainWindow.newSetOfPiecesSelected = false;
            }
        }

        private void helpBTN_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow_Structure newHelpWindow = new HelpWindow_Structure();
            newHelpWindow.Owner = this;
            newHelpWindow.ShowDialog();
            if (MainWindow.newStructureSelected)
            {
                cbxListOfStructures.SelectedIndex = MainWindow.newGameIndexes.Structure;
                CurrentItem.Structure = cbxListOfStructures.SelectionBoxItem.ToString();
                MainWindow.newStructureSelected = false;
            }
        }

        private void helpBTN3_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow_Backgrounds newHelpWindow = new HelpWindow_Backgrounds();
            newHelpWindow.Owner = this;
            newHelpWindow.ShowDialog();
            if (MainWindow.newBackgroundSelected)
            {
                cbxListOfBackgroundImages.SelectedIndex = MainWindow.newGameIndexes.BackgroundCode;
                CurrentItem.BackgroundCode = cbxListOfBackgroundImages.SelectionBoxItem.ToString();
                MainWindow.newBackgroundSelected = false;
            }
        }
    }
}
