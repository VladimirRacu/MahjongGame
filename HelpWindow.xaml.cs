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
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            string helpText = "Mahjong solitaire is a solitaire matching game that uses a set of mahjong tiles rather than cards. " + 
                    " It is also known as Shanghai solitaire, electronic or computerized mahjong, solitaire mahjong and simply as mahjong. " + 
                    " The tiles come from the four-player game mahjong." +
                    "\n\n Play" +
                    "\n The 144 tiles are arranged in a special pattern(structure) with their faces upwards. " + 
                        "A tile is said to be open or exposed if it can be moved either left or right without disturbing other tiles. " + 
                        "The goal is to match open pairs of identical tiles and remove them from the board, exposing the tiles under them for play. "  + 
                        "The game is finished when all pairs of tiles have been removed from the board or when there are no exposed pairs remaining." +
                        "Tiles that are below other tiles cannot be seen. But by repeated undos and/or restarts which some programs offer, one gradually gets more and more information. " + 
                        "Sometimes, tiles are only partially covered by other tiles, and the extent to which such tiles can be distinguished depends on the actual tile set. " +
                        "\n\n Variations" + 
                        "\n Games offer extra options, such as: "+
                        "\n1. Shuffling the tiles" + 
                        "\n2. Changing the tile set and patterns from the traditional tiles to flowers, jewels or other items that may be easier to match up at a glance" + 
                        "\n3. Playing a series of different layouts with varying levels of difficulty" + 
                        "\n These games also have an optional time limit. ";
            
            InitializeComponent();
            txtBoxHelpWindow.Text = helpText;
        }
    }
}
