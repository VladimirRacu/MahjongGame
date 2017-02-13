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
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.Media;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;
using Microsoft.Win32;
using System.Xml.Linq;
using System.Xml;

namespace MahjongGame
{
    [Serializable()]
    public class GameSetting
    {
        public GameSetting(string structure, string backgroundCode, string setOfImages, string complexity)
        {
            Structure = structure;
            BackgroundCode = backgroundCode;
            SetOfImages = setOfImages;
            Complexity = complexity;
        }
        public GameSetting(){}
        public string Structure { get; set; }
        public string BackgroundCode { get; set; }
        public string SetOfImages { get; set; }
        public string Complexity { get; set; }
    }
    [Serializable()]
    public class GameSettingIndexes
    {
        public GameSettingIndexes(int structure, int backgroundCode, int setOfImages, int complexity)
        {
            Structure = structure;
            BackgroundCode = backgroundCode;
            SetOfImages = setOfImages;
            Complexity = complexity;
        }
        public GameSettingIndexes(){}

        public int Structure { get; set; }
        public int BackgroundCode { get; set; }
        public int SetOfImages { get; set; }
        public int Complexity { get; set; }
    }
    
    public class OneTile
    {
        public OneTile(string btnName, int state, string shortPath, string pathToImage, int coordonateLeftFromMargin, int coordonateTopFromMargin, int layerNumber)
        {
            BtnName = btnName;          //  the same as each buttonsOnGrid in gridStructureClassic.Children
            State = state;              //  STATE_MATCHED = 0  /   STATE_HAS_EDGE = 1  /   STATE_HAS_2NEIGHB = 2
            ShortPath = shortPath;      //  short name for faster comparison between buttons
            PathToImage = pathToImage;            
            CoordonateLeftFromMargin = coordonateLeftFromMargin;
            CoordonateTopFromMargin = coordonateTopFromMargin;
            LayerNumber = layerNumber;  //  structure's layer
        }

        public OneTile(){}

        public string BtnName { get; set; }
        public int State { get; set; }
        public string ShortPath { get; set; }
        public string PathToImage { get; set; }        
        public int CoordonateLeftFromMargin { get; set; }
        public int CoordonateTopFromMargin { get; set; }
        public int LayerNumber { get; set; }
    }

    public class OneStep
    {
        public OneStep(string firstBtnName, string secondBtnName, string layerOfTiles)
        {
            FirstBtnName = firstBtnName;
            SecondBtnName = secondBtnName;
            LayerOfTiles = layerOfTiles;
        }

        public string FirstBtnName { get; set; }
        public string SecondBtnName { get; set; }
        public string LayerOfTiles { get; set; }

        public OneStep(){}
    }

    public class HintPair
    {
        public HintPair(string _btn1, string _btn2)
        {
            Btn1 = _btn1;
            Btn2 = _btn2;
        }
        public string Btn1 { get; set; }
        public string Btn2 { get; set; }
    }

    public class SerializeDictionaryClass
    {
        public Dictionary<string, List<string>> LikeProperties { get; set; }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int NUMBER_OF_TILES = 144;
        const int WIDTH_OF_TILE = 70;
        const int HEIGHT_OF_TILE = 90;
        const int DISTANCE_BETWEEN_TILES = 0;
        const int TILE_SHIFT_ON_NEW_LAYER = 10;
        const int STATE_MATCHED = 0;
        const int STATE_HAS_EDGE= 1;
        const int STATE_HAS_2NEIGHB = 2;
        public static string defaultBackground = @"pack://application:,,,/MahjongGame;component/MahjongImages/Backgrounds/Standard.jpg";
        string pathToImagesSet = @"pack://application:,,,/MahjongGame;component/MahjongImages";
            //System.IO.Path.GetFullPath(@"..\..\MahjongImages");
        string pathToBackground = @"pack://application:,,,/MahjongGame;component/MahjongImages/Backgrounds/";
            //System.IO.Path.GetFullPath(@"..\..\MahjongImages\Backgrounds\");
        string pathToFaceDown = @"pack://application:,,,/MahjongGame;component/MahjongImages/";
            //System.IO.Path.GetFullPath(@"..\..\MahjongImages\");
        string pathToSoundFile_Legend = @"pack://application:,,,/MahjongGame;component/MahjongSounds/Legend.mid";
            //System.IO.Path.GetFullPath(@"..\..\MahjongSounds\Legend.mid");
        public static GameSetting newGame;
        public static GameSettingIndexes newGameIndexes;
        DoubleAnimation animation = new DoubleAnimation();
        public static BitmapImage mainWindowBackgroundImage, structureBackgroundImage;
        public static bool newGameStarted = false;
        public static bool newStructureSelected = false;
        public static bool newSetOfPiecesSelected = false;        
        public static bool newBackgroundSelected = false;
        List<string> initialList = new List<string>();
        List<string> randomizedList = new List<string>();
        List<HintPair> hintList = new List<HintPair>();
        static Dictionary<string, List<string>> listOfRandomizedLayers = new Dictionary<string, List<string>>();   // listOfTiles layer, list of corresponded randomized Images(full path to image file )
        static List<OneStep> stepsList = new List<OneStep>();     // pairs of tiles, matched by player
        static List<OneStep> strategyToWin = new List<OneStep>();  // keep the strategy to win the game
        static List<OneTile> listOfTiles = new List<OneTile>();
        public static Dictionary<int, string> listGameComplexity = new Dictionary<int, string>();
        Random random = new Random();
        OneTile firstSelection = null;
        int displayHint = 0;
        Button firstSelectedButton = null;
        MediaPlayer mediaPlayer = new MediaPlayer();
        static string GameSettingsPath = System.IO.Path.GetFullPath(@"..\..\SavedFiles\SerializationOverview_GameSetting.xml");
        static string GameSettingIndexesPath = System.IO.Path.GetFullPath(@"..\..\SavedFiles\SerializationOverview_GameSettingIndexes.xml");
        static string TileListFilePath = System.IO.Path.GetFullPath(@"..\..\SavedFiles\SerializationOverview_OneTile.xml");
        static string StepsListPath = System.IO.Path.GetFullPath(@"..\..\SavedFiles\SerializationOverview_stepsList.xml");
        static string ListOfRandomizedLayersPath = System.IO.Path.GetFullPath(@"..\..\SavedFiles\SerializationOverview_listOfRandomizedLayers.xml");

        static Func<int, int> Factorial = x => x < 0 ? -1 : x == 1 || x == 0 ? 1 : x * Factorial(x - 1);



        public MainWindow()
        {
            Mouse.OverrideCursor = Cursors.Pen;
            newGame = new GameSetting("Square", "Standard.jpg", "classicSet", "Simple. All tiles available");
            newGameIndexes = new GameSettingIndexes(0, 0, 0, 0); 
            listGameComplexity.Add(0, "Simple. All tiles available");
            listGameComplexity.Add(1, "Medium. Usual rules");
            listGameComplexity.Add(2, "Hard. Unavailable tiles are hidden");

            InitializeComponent();
            //gridStructureClassic.Visibility = Visibility.Visible;
            buttonTest.Visibility = Visibility.Visible;

            gridStructureClassic.Visibility = Visibility.Hidden;

            var mainWindowBackgroundImageUri = new Uri(@defaultBackground, UriKind.Absolute);
            MainMenuWindow.Background = new ImageBrush(new BitmapImage(mainWindowBackgroundImageUri));

            mainWindowBackgroundImage = new BitmapImage(mainWindowBackgroundImageUri);
        }

        /////////////////////////////////////////
        /// METHODS//////////////////////////////
        /////////////////////////////////////////

        private void initNewGame() 
        {
            pathToFaceDown = System.IO.Path.GetFullPath(@"..\..\MahjongImages\");
            initializeTiles();

            listOfRandomizedLayers = new Dictionary<string, List<string>>();
            stepsList = new List<OneStep>();

            foreach (var buttonsOnGrid in gridStructureClassic.Children)
                if (buttonsOnGrid is Button)
                {
                    Button _button = (Button)buttonsOnGrid;
                    if (_button.Name.Substring(0, 2).Equals("bt"))
                    {
                        _button.Opacity = 1;
                        _button.IsEnabled = true;
                        _button.Visibility = Visibility.Visible;
                        if (newGameIndexes.Complexity > 1)
                        {
                            int index = listOfTiles.FindIndex(s => s.BtnName == _button.Name);
                            listOfTiles[index].State = STATE_HAS_2NEIGHB;
                        }
                        else 
                        {
                            int index = listOfTiles.FindIndex(s => s.BtnName == _button.Name);
                            listOfTiles[index].State = STATE_HAS_EDGE;
                        }
                    }
                }
        }

        private void initializeTiles()
        {
            if (newGame.Structure == "Square")
            {
                stopSoundFile();
                drawStructureSquare();
                playSoundFile(pathToSoundFile_Legend);
            }
            if (newGame.Structure == "Pyramid")
            {
                stopSoundFile();
                drawStructurePyramid();
                playSoundFile(pathToSoundFile_Legend);
            }
            if (newGame.Structure == "Bridge")
            {
                stopSoundFile();
                drawStructureBridge();
                playSoundFile(pathToSoundFile_Legend);
            }
            if (newGame.Structure == "VenetianBridges")
            {
                stopSoundFile();
                drawStructureVenetianBridges();
                playSoundFile(pathToSoundFile_Legend);
            }
        }

        private void drawStructureSquare()
        {
            Button myButton = new Button();
            OneTile tile;
            listOfTiles = new List<OneTile>();
            int leftMargin = 0, topMargin = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 18; j++)
                {
                    myButton = new Button();
                    myButton.Style = (Style)FindResource("btnTiles");
                    myButton.Name = "bt" + (i * 18 + j);
                    myButton.HorizontalAlignment = HorizontalAlignment.Left;
                    myButton.VerticalAlignment = VerticalAlignment.Top;
                    myButton.Width = WIDTH_OF_TILE;
                    myButton.Height = HEIGHT_OF_TILE;
                    myButton.BorderThickness = new Thickness(1, 1, 5, 1);
                    myButton.Effect =
                        new DropShadowEffect
                        {
                            Color = new Color { A = 0, R = 0, G = 0, B = 0 },
                            Direction = 315,
                            ShadowDepth = 8,
                            Opacity = 1
                        };
                    leftMargin = 25 + j * (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES);
                    topMargin = 5 + i * (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES);
                    myButton.Margin = new Thickness(leftMargin, topMargin, 0, 0);
                    myButton.Click += (sender, events) =>
                    {
                        btnTiles_Click(sender, events);
                    };
                    myButton.MouseRightButtonDown += (sender, events) =>
                    {
                        btnTiles_MouseRightButtonDown(sender, events);
                    };
                    if (gridStructureClassic.FindName(myButton.Name) != null)
                    {
                        Button _button1 = gridStructureClassic.FindName(myButton.Name) as Button;
                        gridStructureClassic.UnregisterName(myButton.Name);
                        gridStructureClassic.Children.Remove(_button1);
                    }
                    gridStructureClassic.RegisterName(myButton.Name, myButton);
                    gridStructureClassic.Children.Add(myButton);
                    tile = new OneTile(myButton.Name, STATE_HAS_2NEIGHB, "", "", leftMargin, topMargin, 0);
                    listOfTiles.Add(tile);
                }
            }
        }

        private void drawStructurePyramid()
        {
            Button myButton = new Button();
            OneTile tile;
            listOfTiles = new List<OneTile>();
            bool createNewBtn = false;
            int leftMargin = 0, topMargin = 0;
            int count = 0;
            for (int z = 0; z < 6; z++)
            {
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        myButton = new Button();
                        myButton.Style = (Style)FindResource("btnTiles");
                        if (
                            (z == 0) &&
                                (
                                    ((i < 3) && (j != 0) && (j != 8)) ||
                                    (i == 3) ||
                                    ((i > 3) && (j != 0) && (j != 8))
                                )
                            )
                        {
                            myButton.Name = "bt" + count;
                            count++;
                            createNewBtn = true;
                        }
                        else
                            if (
                                (z > 0) && (z < 4) &&
                                    (
                                        (i < (7 - z)) && (j != 0) && (j < (8 - z))
                                    )
                                )
                            {
                                myButton.Name = "bt" + count;
                                count++;
                                createNewBtn = true;
                            }
                            else
                                if (
                                    (z == 4) &&
                                        (
                                            ((i == 0) && (j != 0) && (j < 5)) ||
                                            ((i == 3) && (j != 0) && (j < 5)) ||
                                            ((i == 1) && (j == 1)) ||
                                            ((i == 1) && (j == 4)) ||
                                            ((i == 2) && (j == 1)) ||
                                            ((i == 2) && (j == 4))
                                        )
                                    )
                                {
                                    myButton.Name = "bt" + count;
                                    count++;
                                    createNewBtn = true;
                                }
                                else
                                    if (
                                        (z == 5) &&
                                            (
                                                ((i == 0) && (j == 1)) ||
                                                ((i == 0) && (j == 4)) ||
                                                ((i == 3) && (j == 1)) ||
                                                ((i == 3) && (j == 4))
                                            )
                                        )
                                    {
                                        myButton.Name = "bt" + count;
                                        count++;
                                        createNewBtn = true;
                                    }
                        if (createNewBtn)
                        {
                            myButton.HorizontalAlignment = HorizontalAlignment.Left;
                            myButton.VerticalAlignment = VerticalAlignment.Top;
                            myButton.Width = WIDTH_OF_TILE;
                            myButton.Height = HEIGHT_OF_TILE;
                            myButton.BorderThickness = new Thickness(1, 1, 5, 1);

                            myButton.Effect =
                                new DropShadowEffect
                                {
                                    Color = new Color { A = 0, R = 0, G = 0, B = 0 },
                                    Direction = 315,
                                    ShadowDepth = 8,
                                    Opacity = 1
                                };
                            if (z < 4)
                            {
                                leftMargin = 375 + j * (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES) +
                                    z * (int)(WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES) / 2;
                                topMargin = 75 + i * (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES) +
                                    z * (int)(HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES) / 2;
                            }
                            else
                            {
                                leftMargin = 375 + j * (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES) +
                                    3 * (int)(WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES) / 2;
                                topMargin = 75 + i * (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES) +
                                    3 * (int)(HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES) / 2;
                            }
                            myButton.Margin = new Thickness(leftMargin - z * TILE_SHIFT_ON_NEW_LAYER, topMargin - z * TILE_SHIFT_ON_NEW_LAYER, 0, 0);
                            myButton.Click += (sender, events) =>
                            {
                                btnTiles_Click(sender, events);
                            };
                            myButton.MouseRightButtonDown += (sender, events) =>
                            {
                                btnTiles_MouseRightButtonDown(sender, events);
                            };

                            if (gridStructureClassic.FindName(myButton.Name) != null)
                            {
                                Button _button1 = gridStructureClassic.FindName(myButton.Name) as Button;
                                gridStructureClassic.UnregisterName(myButton.Name);
                                gridStructureClassic.Children.Remove(_button1);

                            }
                            gridStructureClassic.RegisterName(myButton.Name, myButton);
                            gridStructureClassic.Children.Add(myButton);
                            myButton.Opacity = 1;
                            myButton.IsEnabled = true;
                            myButton.Visibility = Visibility.Visible;
                            tile = new OneTile(myButton.Name, STATE_HAS_2NEIGHB, "", "", leftMargin, topMargin, z);
                            listOfTiles.Add(tile);
                            createNewBtn = false;
                        }
                    }
                }
            }
        }

        private void drawStructureBridge()
        {
            Button myButton = new Button();
            OneTile tile;
            listOfTiles = new List<OneTile>();
            bool createNewBtn = false;
            int leftMargin = 0, topMargin = 0;
            int count = 0;
            for (int z = 0; z < 11; z++)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        myButton = new Button();
                        myButton.Style = (Style)FindResource("btnTiles");
                        if (
                            (z < 4) &&
                                (
                                    ((i == 0) &&
                                            ((j >= z) && (j < 4)) ||
                                            ((j > 10) && (j <= 14 - z)))
                                            ||
                                    ((i == 1) && (j < 14 - z))
                                            ||
                                    ((i == 2) &&
                                            ((j >= z) && (j < 4)) ||
                                            ((j > 10) && (j <= 14 - z)))
                                )
                            )
                        {
                            myButton.Name = "bt" + count;
                            count++;
                            createNewBtn = true;
                        }
                        else
                            if (
                                (z >= 4) && (z < 9) && (i == 1) && (j <= 14 - z)
                                )
                            {
                                myButton.Name = "bt" + count;
                                count++;
                                createNewBtn = true;
                            }
                            else
                                if (
                                    ((z == 9) && (i == 1) && (j > 0) && (j < 5))
                                    ||
                                    ((z == 10) && (i == 1) && (j == 2))
                                    )
                                {
                                    myButton.Name = "bt" + count;
                                    count++;
                                    createNewBtn = true;
                                }
                        if (createNewBtn)
                        {
                            myButton.HorizontalAlignment = HorizontalAlignment.Left;
                            myButton.VerticalAlignment = VerticalAlignment.Top;
                            myButton.Width = WIDTH_OF_TILE;
                            myButton.Height = HEIGHT_OF_TILE;
                            myButton.BorderThickness = new Thickness(1, 1, 5, 1);

                            myButton.Effect =
                                new DropShadowEffect
                                {
                                    Color = new Color { A = 0, R = 0, G = 0, B = 0 },
                                    Direction = 315,
                                    ShadowDepth = 8,
                                    Opacity = 1
                                };
                            if ((z < 4) && (i != 1))
                            {
                                leftMargin = 175 + j * (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES);
                                topMargin = 275 + i * (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES);
                            }
                            else
                                if (z < 4)
                                {
                                    leftMargin = 175 + j * (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES) +
                                        z * (int)(WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES) / 2;
                                    topMargin = 275 + i * (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES);
                                }
                                else
                                {
                                    leftMargin = 175 + j * (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES) +
                                        z * (int)(WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES) / 2;
                                    topMargin = 275 + i * (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES);
                                }
                            myButton.Margin = new Thickness(leftMargin - z * TILE_SHIFT_ON_NEW_LAYER, topMargin - z * TILE_SHIFT_ON_NEW_LAYER, 0, 0);
                            myButton.Click += (sender, events) =>
                            {
                                btnTiles_Click(sender, events);
                            };
                            myButton.MouseRightButtonDown += (sender, events) =>
                            {
                                btnTiles_MouseRightButtonDown(sender, events);
                            };

                            if (gridStructureClassic.FindName(myButton.Name) != null)
                            {
                                Button _button1 = gridStructureClassic.FindName(myButton.Name) as Button;
                                gridStructureClassic.UnregisterName(myButton.Name);
                                gridStructureClassic.Children.Remove(_button1);

                            }

                            gridStructureClassic.RegisterName(myButton.Name, myButton);
                            gridStructureClassic.Children.Add(myButton);
                            myButton.Opacity = 1;
                            myButton.IsEnabled = true;
                            myButton.Visibility = Visibility.Visible;
                            tile = new OneTile(myButton.Name, STATE_HAS_2NEIGHB, "", "", leftMargin, topMargin, z);
                            listOfTiles.Add(tile);
                            createNewBtn = false;
                        }
                    }
                }
            }
        }

        private void drawStructureVenetianBridges()
        {
            Button myButton = new Button();
            OneTile tile;
            listOfTiles = new List<OneTile>();
            bool createNewBtn = false;
            int leftMargin = 0, topMargin = 0;
            int count = 0;
            for (int z = 0; z < 10; z++)
            {
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        myButton = new Button();
                        myButton.Style = (Style)FindResource("btnTiles");
                        if (
                            (z == 0) &&
                                (
                                    (((i == 0) || (i == 3) || (i == 6))
                                        &&
                                        ((j < 6) || (j > 8)))
                                    ||
                                    (((i == 1) || (i == 2) || (i == 4) || (i == 5))
                                        &&
                                        ((j == 1) || (j == 13)))
                                )
                            )
                        {
                            myButton.Name = "bt" + count;
                            count++;
                            createNewBtn = true;
                        }
                        else
                            if (
                                (z == 1) &&
                                    (
                                        (((i == 0) || (i == 6))
                                            &&
                                            ((((j > 0) && (j < 4)) || (j == 5))
                                            ||
                                            (((j > 10) && (j < 14)) || (j == 9))))
                                        ||
                                        ((i == 3)
                                            &&
                                            (((j < 4) || (j == 5))
                                            ||
                                            ((j > 10) || (j == 9))))
                                        ||
                                        (((i == 1) || (i == 2) || (i == 4) || (i == 5))
                                            &&
                                            ((j == 1) || (j == 13)))
                                    )
                                )
                            {
                                myButton.Name = "bt" + count;
                                count++;
                                createNewBtn = true;
                            }
                            else
                                if (
                                    (z == 2) &&
                                        (
                                            ((i == 0) || (i == 3) || (i == 6))
                                            &&
                                            ((j == 3) || (j == 5) || (j == 8) || (j == 11))
                                        )
                                    )
                                {
                                    myButton.Name = "bt" + count;
                                    count++;
                                    createNewBtn = true;
                                }
                                else
                                    if (
                                        (z == 3) &&
                                            (
                                                ((i == 0) || (i == 3) || (i == 6))
                                                &&
                                                ((j == 3) || (j == 6) || (j == 8) || (j == 10))
                                            )
                                        )
                                    {
                                        myButton.Name = "bt" + count;
                                        count++;
                                        createNewBtn = true;
                                    }
                                    else
                                        if (
                                            (z == 4) &&
                                                (
                                                    ((i == 0) || (i == 3) || (i == 6))
                                                    &&
                                                    ((j == 4) || (j == 6) || (j == 7) || (j == 10))
                                                )
                                            )
                                        {
                                            myButton.Name = "bt" + count;
                                            count++;
                                            createNewBtn = true;
                                        }
                                        else
                                            if (
                                            ((z == 5) || (z == 6)) &&
                                                (
                                                    ((i == 0) || (i == 3) || (i == 6))
                                                    &&
                                                    ((j == z - 1) || (j == 9))
                                                )
                                            )
                                            {
                                                myButton.Name = "bt" + count;
                                                count++;
                                                createNewBtn = true;
                                            }
                                            else
                                                if (
                                                ((z == 7) || (z == 8)) &&
                                                    (
                                                        ((i == 0) || (i == 3) || (i == 6))
                                                        &&
                                                        ((j == z - 2) || (j == 8))
                                                    )
                                                )
                                                {
                                                    myButton.Name = "bt" + count;
                                                    count++;
                                                    createNewBtn = true;
                                                }
                                                else
                                                    if (
                                                    (z == 9) &&
                                                        (
                                                            ((i == 0) || (i == 3) || (i == 6))
                                                            &&
                                                            ((j == 6) || (j == 7))
                                                        )
                                                    )
                                                    {
                                                        myButton.Name = "bt" + count;
                                                        count++;
                                                        createNewBtn = true;
                                                    }
                        if (createNewBtn)
                        {
                            myButton.HorizontalAlignment = HorizontalAlignment.Left;
                            myButton.VerticalAlignment = VerticalAlignment.Top;
                            myButton.Width = WIDTH_OF_TILE;
                            myButton.Height = HEIGHT_OF_TILE;
                            myButton.BorderThickness = new Thickness(1, 1, 5, 1);

                            myButton.Effect =
                                new DropShadowEffect
                                {
                                    Color = new Color { A = 0, R = 0, G = 0, B = 0 },
                                    Direction = 315,
                                    ShadowDepth = 8,
                                    Opacity = 1
                                };
                            if (z < 2)
                            {
                                leftMargin = 100 + j * (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES);
                                topMargin = 100 + i * (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES);
                            }
                            else
                                if (
                                    ((z == 2) && ((j == 3) || (j == 11)))
                                    ||
                                    ((z == 4) && ((j == 4) || (j == 10)))
                                    )
                                {
                                    leftMargin = 100 + j * (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES);
                                    topMargin = 100 + i * (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES);
                                }
                                else
                                    if (
                                        ((z == 2) && ((j == 5) || (j == 8)))
                                        ||
                                        ((z == 4) && ((j == 6) || (j == 7)))
                                        )
                                    {
                                        leftMargin = 100 + j * (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES) +
                                            (int)(WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES) / 2;
                                        topMargin = 100 + i * (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES);
                                    }
                                    else
                                        if ((z == 3) && ((j == 6) || (j == 8)))
                                        {
                                            leftMargin = 100 + j * (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES);
                                            topMargin = 100 + i * (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES);
                                        }
                                        else
                                            if ((z == 3) && ((j == 3) || (j == 10)))
                                            {
                                                leftMargin = 100 + j * (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES) +
                                                    (int)(WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES) / 2;
                                                topMargin = 100 + i * (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES);
                                            }
                                            else
                                                if (
                                                    ((z == 5) && ((j == 4) || (j == 9)))
                                                    ||
                                                    ((z == 7) && ((j == 5) || (j == 8)))
                                                    ||
                                                    ((z == 9) && ((j == 6) || (j == 7)))
                                                    )
                                                {
                                                    leftMargin = 100 + j * (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES) +
                                                        (int)(WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES) / 2;
                                                    topMargin = 100 + i * (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES);
                                                }
                                                else
                                                    if (
                                                        ((z == 6) && ((j == 5) || (j == 9)))
                                                        ||
                                                        ((z == 8) && ((j == 6) || (j == 8)))
                                                        )
                                                    {
                                                        leftMargin = 100 + j * (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES);
                                                        topMargin = 100 + i * (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES);
                                                    }
                            myButton.Margin = new Thickness(leftMargin - z * (TILE_SHIFT_ON_NEW_LAYER - 5), topMargin - z * (TILE_SHIFT_ON_NEW_LAYER + 4), 0, 0);
                            myButton.Click += (sender, events) =>
                            {
                                btnTiles_Click(sender, events);
                            };
                            myButton.MouseRightButtonDown += (sender, events) =>
                            {
                                btnTiles_MouseRightButtonDown(sender, events);
                            };

                            if (gridStructureClassic.FindName(myButton.Name) != null)
                            {
                                Button _button1 = gridStructureClassic.FindName(myButton.Name) as Button;
                                gridStructureClassic.UnregisterName(myButton.Name);
                                gridStructureClassic.Children.Remove(_button1);

                            }

                            gridStructureClassic.RegisterName(myButton.Name, myButton);
                            gridStructureClassic.Children.Add(myButton);
                            myButton.Opacity = 1;
                            myButton.IsEnabled = true;
                            myButton.Visibility = Visibility.Visible;
                            tile = new OneTile(myButton.Name, STATE_HAS_2NEIGHB, "", "", leftMargin, topMargin, z);
                            listOfTiles.Add(tile);
                            createNewBtn = false;
                        }
                    }
                }
            }
        }

        private void randomizeTilesWithStrategy()
        {
            initialList = new List<string>();
            string tempPathToImagesSet = pathToImagesSet + "\\" + newGame.SetOfImages;
            // add images from bamboo folder (each x4)
            string addittionalPath = tempPathToImagesSet + @"\bamboo\bamboo";
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j <= 9; j++)
                {
                    initialList.Add(addittionalPath + j + ".png");
                }
            }
            // add images from man folder (each x4)
            addittionalPath = tempPathToImagesSet + @"\man\man";
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j <= 9; j++)
                {
                    initialList.Add(addittionalPath + j + ".png");
                }
            }
            // add images from pin folder (each x4)
            addittionalPath = tempPathToImagesSet + @"\pin\pin";
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j <= 9; j++)
                {
                    initialList.Add(addittionalPath + j + ".png");
                }
            }
            // add images from flowers folder (each just one)
            addittionalPath = tempPathToImagesSet + @"\flowers\";
            for (int i = 0; i < 1; i++)
            {
                initialList.Add(addittionalPath + "Bamboo.png");
                initialList.Add(addittionalPath + "Chrysanthemum.png");
                initialList.Add(addittionalPath + "Orchid.png");
                initialList.Add(addittionalPath + "Plum.png");
            }
            // add images from seasons folder (each just one)
            addittionalPath = tempPathToImagesSet + @"\seasons\";
            for (int i = 0; i < 1; i++)
            {
                initialList.Add(addittionalPath + "Autumn.png");
                initialList.Add(addittionalPath + "Spring.png");
                initialList.Add(addittionalPath + "Summer.png");
                initialList.Add(addittionalPath + "Winter.png");
            }
            // add images from dragons folder (each x4)
            addittionalPath = tempPathToImagesSet + @"\dragons\";
            for (int i = 0; i < 4; i++)
            {
                initialList.Add(addittionalPath + "dragonGreen.png");
                initialList.Add(addittionalPath + "dragonRed.png");
                initialList.Add(addittionalPath + "dragonWhite.png");
            }

            // add images from winds folder (each x4)
            addittionalPath = tempPathToImagesSet + @"\winds\";
            for (int i = 0; i < 4; i++)
            {
                initialList.Add(addittionalPath + "windEast.png");
                initialList.Add(addittionalPath + "windNorth.png");
                initialList.Add(addittionalPath + "windSouth.png");
                initialList.Add(addittionalPath + "windWest.png");
            }
            // add faceDown image for hard complexity game
            pathToFaceDown += newGame.SetOfImages + "\\faceDown.png";

            var mainMenuWindowBackgroundImageUri = new Uri(pathToBackground + newGame.BackgroundCode, UriKind.Absolute);
            MainMenuWindow.Background = new ImageBrush(new BitmapImage(mainMenuWindowBackgroundImageUri));

            List<int> indexes = new List<int>(); // temp list of indexes. Will be used to check images which were added to randomizedList
            randomizedList = new List<string>();
            strategyToWin = new List<OneStep>();
            for (int i = 0; i < NUMBER_OF_TILES; i++)
            {
                indexes.Add(i);
            } // initiate the full list - 144 elements

            for (int j = 0; j < NUMBER_OF_TILES; j++)
            {
                int selectedItem = random.Next(0, indexes.Count);
                int selectedItem2 = 0;
                string tempStr = initialList[indexes[selectedItem]];
                randomizedList.Add(tempStr);
                initialList[indexes[selectedItem]] = "";
                indexes.RemoveAt(selectedItem);   
                if (tempStr.Contains(@"\seasons\"))
                {
                    selectedItem2 = initialList.FindIndex(x => x.Contains(@"\seasons\"));
                    tempStr = initialList[selectedItem2];
                }
                else
                    if (tempStr.Contains(@"\flowers\"))
                    {
                        selectedItem2 = initialList.FindIndex(x => x.Contains(@"\flowers\"));
                        tempStr = initialList[selectedItem2];

                    }
                    else selectedItem2 = initialList.FindIndex(x => x.Equals(tempStr));
                randomizedList.Add(tempStr);
                selectedItem = indexes.FindIndex(x => x.Equals(selectedItem2));
                initialList[indexes[selectedItem]] = "";
                indexes.RemoveAt(selectedItem);
                j++;
            }

            string tempWord = "layer" + listOfRandomizedLayers.Count;
            listOfRandomizedLayers.Add(tempWord, randomizedList);
            lblStepsShow.Content = "" + stepsList.Count + "(" + (listOfRandomizedLayers.Count - 1) + ")";
        }

        private void randomizeTiles()
        {
            initialList = new List<string>();
            string tempPathToImagesSet = pathToImagesSet + "\\" + newGame.SetOfImages;
            // add images from bamboo folder (each x4)
            string addittionalPath = tempPathToImagesSet + @"\bamboo\bamboo";
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j <= 9; j++)
                {
                    initialList.Add(addittionalPath + j + ".png");
                }
            }
            // add images from man folder (each x4)
            addittionalPath = tempPathToImagesSet + @"\man\man";
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j <= 9; j++)
                {
                    initialList.Add(addittionalPath + j + ".png");
                }
            }
            // add images from pin folder (each x4)
            addittionalPath = tempPathToImagesSet + @"\pin\pin";
            for (int i = 0; i < 4; i++)
            {
                for (int j = 1; j <= 9; j++)
                {
                    initialList.Add(addittionalPath + j + ".png");
                }
            }
            // add images from flowers folder (each just one)
            addittionalPath = tempPathToImagesSet + @"\flowers\";
            for (int i = 0; i < 1; i++)
            {
                initialList.Add(addittionalPath + "Bamboo.png");
                initialList.Add(addittionalPath + "Chrysanthemum.png");
                initialList.Add(addittionalPath + "Orchid.png");
                initialList.Add(addittionalPath + "Plum.png");
            }
            // add images from seasons folder (each just one)
            addittionalPath = tempPathToImagesSet + @"\seasons\";
            for (int i = 0; i < 1; i++)
            {
                initialList.Add(addittionalPath + "Autumn.png");
                initialList.Add(addittionalPath + "Spring.png");
                initialList.Add(addittionalPath + "Summer.png");
                initialList.Add(addittionalPath + "Winter.png");
            }
            // add images from dragons folder (each x4)
            addittionalPath = tempPathToImagesSet + @"\dragons\";
            for (int i = 0; i < 4; i++)
            {
                initialList.Add(addittionalPath + "dragonGreen.png");
                initialList.Add(addittionalPath + "dragonRed.png");
                initialList.Add(addittionalPath + "dragonWhite.png");
            }

            // add images from winds folder (each x4)
            addittionalPath = tempPathToImagesSet + @"\winds\";
            for (int i = 0; i < 4; i++)
            {
                initialList.Add(addittionalPath + "windEast.png");
                initialList.Add(addittionalPath + "windNorth.png");
                initialList.Add(addittionalPath + "windSouth.png");
                initialList.Add(addittionalPath + "windWest.png");
            }
            // add faceDown image for hard complexity game
            pathToFaceDown += newGame.SetOfImages + "\\faceDown.png";

            var mainMenuWindowBackgroundImageUri = new Uri(pathToBackground + newGame.BackgroundCode, UriKind.Absolute);
            MainMenuWindow.Background = new ImageBrush(new BitmapImage(mainMenuWindowBackgroundImageUri));


            List<int> indexes = new List<int>(); // temp list of indexes. Will be used to check images which were added to randomizedList
            randomizedList = new List<string>();
            strategyToWin = new List<OneStep>();

            for (int i = 0; i < NUMBER_OF_TILES; i++)
            {
                indexes.Add(i);
            } // initiate the full list - 144 elements

            for (int j = 0; j < NUMBER_OF_TILES; j++)
            {
                int selectedItem = random.Next(0, indexes.Count);
                randomizedList.Add(initialList[indexes[selectedItem]]);
                indexes.RemoveAt(selectedItem);
            }
            string tempWord = "layer" + listOfRandomizedLayers.Count;
            listOfRandomizedLayers.Add(tempWord, randomizedList);
        }

        private void assignRandomizedListToListOfTiles()
        {
            randomizedList = new List<string>();
            foreach (string str in listOfRandomizedLayers.LastOrDefault().Value) 
            {
                randomizedList.Add(str);
            }

            Image brush = new Image();
            int count = 0;
            foreach (var buttonsOnGrid in gridStructureClassic.Children)
                if (buttonsOnGrid is Button)
                {
                    Button _button = (Button)buttonsOnGrid;
                    if (_button.Name.Substring(0, 2).Equals("bt"))
                    {
                        if (randomizedList[count] != "")
                        {
                            brush = new Image();
                            brush.Source = new BitmapImage(new Uri(randomizedList[count]));
                            brush.Stretch = Stretch.Fill;
                            int newSearch = listOfTiles.FindIndex(s => s.BtnName == _button.Name);

                            listOfTiles[newSearch].PathToImage = randomizedList[count];
                            string shortPath;
                            if (randomizedList[count].Contains(@"\seasons\"))
                            {
                                shortPath = "seasons";

                            }
                            else
                                if (randomizedList[count].Contains(@"\flowers\"))
                                {
                                    shortPath = "flowers";

                                }
                                else shortPath = randomizedList[count].Substring(randomizedList[count].Length - 10, 6);

                            listOfTiles[newSearch].ShortPath = shortPath;

                            _button.Content = brush;
                            _button.Visibility = Visibility.Visible;                            
                        }
                        count++;
                    }
                }
        }

        private void assignRandomizedListToListOfTilesWithStrategy()
        {
            strategyToWin = new List<OneStep>();
            OneTile tempTile1, tempTile2 = new OneTile();
            OneStep tempStep = null;
            List<string> tempAvailableTiles = new List<string>();

            checkButtonsStates();

            foreach (OneTile buttonsOnList in listOfTiles)
            {
                if (buttonsOnList.State == STATE_HAS_EDGE)
                {
                    tempAvailableTiles.Add(buttonsOnList.BtnName);
                }
            }

            var checkVar = listOfTiles.FindAll(s => s.State != STATE_MATCHED);
            while (checkVar.Count != 0)
            {
                int countOfAvailableTiles = listOfTiles.FindAll(x => x.State == STATE_HAS_EDGE).Count;
                if (countOfAvailableTiles >= 2)
                {
                    int selectedItem = random.Next(0, countOfAvailableTiles);

                    int resultedIndex = -1;
                    for (int k = 0; k < listOfTiles.Count; k++)
                    {
                        if (listOfTiles[k].State == STATE_HAS_EDGE) resultedIndex++;
                        if (resultedIndex == selectedItem) { selectedItem = k; break; }
                    }
                    tempTile1 = listOfTiles[selectedItem];
                    tempTile1.State = STATE_MATCHED;
                    tempStep = new OneStep();
                    tempStep.FirstBtnName = tempTile1.BtnName;

                    countOfAvailableTiles = listOfTiles.FindAll(x => x.State == STATE_HAS_EDGE).Count;

                    selectedItem = random.Next(0, countOfAvailableTiles);
                    resultedIndex = -1;
                    for (int k = 0; k < listOfTiles.Count; k++)
                    {
                        if (listOfTiles[k].State == STATE_HAS_EDGE) resultedIndex++;
                        if (resultedIndex == selectedItem) { selectedItem = k; break; }
                    }
                    tempTile2 = listOfTiles[selectedItem];

                    tempTile2.State = STATE_MATCHED;
                    tempStep.SecondBtnName = tempTile2.BtnName;
                    strategyToWin.Add(tempStep);
                    checkButtonsStates();
                }
                else
                {
                    MessageBox.Show("Error on initialize the game. The program will try to restart the drawing of the structure.");
                    strategyToWin = new List<OneStep>();
                    tempAvailableTiles = new List<string>();
                    foreach (OneTile buttonsOnList in listOfTiles)
                    {
                        buttonsOnList.State = STATE_HAS_2NEIGHB;
                    }
                    checkButtonsStates();
                    foreach (OneTile buttonsOnList in listOfTiles)
                    {
                        if (buttonsOnList.State == STATE_HAS_EDGE)
                        {
                            tempAvailableTiles.Add(buttonsOnList.BtnName);
                        }
                    }
                }
                checkVar = listOfTiles.FindAll(s => s.State != STATE_MATCHED);

            }
            int count = 0;
            //strategyToWin.Reverse();

            foreach (OneStep stepOnList in strategyToWin)
            {
                string tempTileName1 = stepOnList.FirstBtnName;
                string tempTileName2 = stepOnList.SecondBtnName;

                int tempIndex = listOfTiles.FindIndex(x => x.BtnName == tempTileName1);
                listOfTiles[tempIndex].PathToImage = randomizedList[count];
                string shortPath = randomizedList[count];
                if (shortPath.Contains(@"\seasons\"))
                {
                    shortPath = "seasons";
                }
                else
                    if (shortPath.Contains(@"\flowers\"))
                    {
                        shortPath = "flowers";
                    }
                    else shortPath = shortPath.Substring(shortPath.Length - 10, 6);
                listOfTiles[tempIndex].ShortPath = shortPath;
                listOfTiles[tempIndex].State = STATE_HAS_2NEIGHB;
                count++;

                tempIndex = listOfTiles.FindIndex(x => x.BtnName == tempTileName2);
                listOfTiles[tempIndex].PathToImage = randomizedList[count];
                shortPath = randomizedList[count];
                if (shortPath.Contains(@"\seasons\"))
                {
                    shortPath = "seasons";
                }
                else
                    if (shortPath.Contains(@"\flowers\"))
                    {
                        shortPath = "flowers";
                    }
                    else shortPath = shortPath.Substring(shortPath.Length - 10, 6);
                listOfTiles[tempIndex].ShortPath = shortPath;
                listOfTiles[tempIndex].State = STATE_HAS_2NEIGHB;
                count++;
            }
            checkButtonsStates();

            Image brush = new Image();
            count = 0;
            foreach (var buttonsOnGrid in gridStructureClassic.Children)
            {
                if (buttonsOnGrid is Button)
                {
                    Button _button = (Button)buttonsOnGrid;
                    if (_button.Name.Substring(0, 2).Equals("bt"))
                    {
                        brush = new Image();
                        brush.Source = new BitmapImage(new Uri(listOfTiles[count].PathToImage));
                        brush.Stretch = Stretch.Fill;
                        _button.Content = brush;
                        _button.Visibility = Visibility.Visible;
                        count++;
                    }
                }
            }

        }

        //private List<OneTile> strategyPathCheckUnavailableTilesForNextPair(List<OneTile> paramTempListOfTiles, OneTile tempTile)
        //{
        //    List<OneTile> tileOnSideNeighBtns = new List<OneTile>();
        //    int leftDistance = tempTile.CoordonateLeftFromMargin;
        //    int topDistance = tempTile.CoordonateTopFromMargin;
        //    int leftNeighBtn = leftDistance - (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES);
        //    int rightNeighBtn = leftDistance + (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES);
        //    int topNeighBtn = topDistance - (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES);
        //    int bottomNeighBtn = topDistance + (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES);
        //    tileOnSideNeighBtns = paramTempListOfTiles.FindAll(s =>
        //        ((s.CoordonateLeftFromMargin.CompareTo(rightNeighBtn) > 0) ||
        //        (s.CoordonateLeftFromMargin.CompareTo(leftNeighBtn) < 0))
        //        &&
        //        (
        //            (s.CoordonateTopFromMargin.CompareTo(topNeighBtn) > 0) &&
        //            (s.CoordonateTopFromMargin.CompareTo(topDistance) <= 0)
        //            ||
        //            (s.CoordonateTopFromMargin.CompareTo(bottomNeighBtn) < 0) &&
        //            (s.CoordonateTopFromMargin.CompareTo(topDistance) >= 0)
        //        )
        //        &&
        //        (s.LayerNumber.Equals(tempTile.LayerNumber))
        //        &&
        //        (s.State == STATE_HAS_EDGE));
        //    //OneTile tempVar = listOfTiles.Find(s =>
        //    //    (
        //    //        s.CoordonateLeftFromMargin.Equals(leftDistance) &&
        //    //        s.CoordonateTopFromMargin.Equals(topDistance) &&
        //    //        s.LayerNumber.Equals(tempTile.LayerNumber + 1) &&
        //    //        s.State == STATE_HAS_2NEIGHB
        //    //    ));
        //    //if (tempVar != null) paramTempListOfTiles.Add(tempVar);
        //    return tileOnSideNeighBtns;
        //}

        //private void checkButtonsStatesForStrategy()
        //{
        //    foreach (OneTile buttonsOnList in tempListOfTiles)
        //    {
        //        int index = tempListOfTiles.LastIndexOf(buttonsOnList);
        //        int leftDistance = buttonsOnList.CoordonateLeftFromMargin;
        //        int topDistance = buttonsOnList.CoordonateTopFromMargin;
        //        int leftNeighBtn = leftDistance - (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES);
        //        int rightNeighBtn = leftDistance + (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES);
        //        int topNeighBtn = topDistance - (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES);
        //        int bottomNeighBtn = topDistance + (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES);
        //        List<OneTile> tileOverBtns = new List<OneTile>();
        //        List<OneTile> tileLeftNeighBtns = new List<OneTile>();
        //        List<OneTile> tileRightNeighBtns = new List<OneTile>();
        //        tileOverBtns = tempListOfTiles.FindAll(s =>
        //            ((
        //                (s.CoordonateLeftFromMargin.CompareTo(leftNeighBtn) > 0) &&
        //                (s.CoordonateLeftFromMargin.CompareTo(leftDistance) <= 0) &&
        //                (s.CoordonateTopFromMargin.CompareTo(topNeighBtn) > 0) &&
        //                (s.CoordonateTopFromMargin.CompareTo(topDistance) <= 0)
        //            ) || (
        //                (s.CoordonateLeftFromMargin.CompareTo(leftNeighBtn) > 0) &&
        //                (s.CoordonateLeftFromMargin.CompareTo(leftDistance) <= 0) &&
        //                (s.CoordonateTopFromMargin.CompareTo(bottomNeighBtn) < 0) &&
        //                (s.CoordonateTopFromMargin.CompareTo(topDistance) >= 0)
        //            ) || (
        //                (s.CoordonateLeftFromMargin.CompareTo(rightNeighBtn) < 0) &&
        //                (s.CoordonateLeftFromMargin.CompareTo(leftDistance) >= 0) &&
        //                (s.CoordonateTopFromMargin.CompareTo(topNeighBtn) > 0) &&
        //                (s.CoordonateTopFromMargin.CompareTo(topDistance) <= 0)
        //            ) || (
        //                (s.CoordonateLeftFromMargin.CompareTo(rightNeighBtn) < 0) &&
        //                (s.CoordonateLeftFromMargin.CompareTo(leftDistance) >= 0) &&
        //                (s.CoordonateTopFromMargin.CompareTo(bottomNeighBtn) < 0) &&
        //                (s.CoordonateTopFromMargin.CompareTo(topDistance) >= 0)
        //            ) || (
        //                (s.CoordonateLeftFromMargin.Equals(leftDistance)) &&
        //                (s.CoordonateTopFromMargin.Equals(topDistance))
        //            )) &&
        //            (s.LayerNumber.Equals(tempListOfTiles[index].LayerNumber + 1))
        //            &&
        //            (s.State != STATE_MATCHED));
        //        if (tileOverBtns.Count > 0)
        //        {
        //            tempListOfTiles[index].State = STATE_HAS_2NEIGHB;
        //            continue;
        //        }
        //        else
        //        {
        //            tileLeftNeighBtns = tempListOfTiles.FindAll(s =>
        //                (s.CoordonateLeftFromMargin.CompareTo(leftNeighBtn) >= 0) &&
        //                (s.CoordonateLeftFromMargin.CompareTo(leftDistance) <= 0) &&
        //                (
        //                    (s.CoordonateTopFromMargin.CompareTo(topNeighBtn) > 0) &&
        //                    (s.CoordonateTopFromMargin.CompareTo(topDistance) <= 0)
        //                     ||
        //                    (s.CoordonateTopFromMargin.CompareTo(bottomNeighBtn) < 0) &&
        //                    (s.CoordonateTopFromMargin.CompareTo(topDistance) >= 0)
        //                )
        //                &&
        //                (s.LayerNumber.Equals(tempListOfTiles[index].LayerNumber))
        //                &&
        //                (s.State == STATE_MATCHED));
        //            if (tileLeftNeighBtns.Count > 0)
        //            {
        //                if (tempListOfTiles[index].State != STATE_MATCHED) tempListOfTiles[index].State = STATE_HAS_EDGE;
        //            }
        //            else
        //            {
        //                tileRightNeighBtns = tempListOfTiles.FindAll(s =>
        //                    (s.CoordonateLeftFromMargin.CompareTo(rightNeighBtn) <= 0) &&
        //                    (s.CoordonateLeftFromMargin.CompareTo(leftDistance) >= 0)
        //                    &&
        //                    (
        //                        (s.CoordonateTopFromMargin.CompareTo(topNeighBtn) > 0) &&
        //                        (s.CoordonateTopFromMargin.CompareTo(topDistance) <= 0)
        //                         ||
        //                        (s.CoordonateTopFromMargin.CompareTo(bottomNeighBtn) < 0) &&
        //                        (s.CoordonateTopFromMargin.CompareTo(topDistance) >= 0)
        //                    )
        //                    &&
        //                    (s.LayerNumber.Equals(tempListOfTiles[index].LayerNumber))
        //                    &&
        //                    (s.State == STATE_MATCHED));
        //                if (tileRightNeighBtns.Count > 0)
        //                {
        //                    if (tempListOfTiles[index].State != STATE_MATCHED) tempListOfTiles[index].State = STATE_HAS_EDGE;
        //                }
        //            }
        //        }
        //    }
        //}

        private void checkButtonsStates()
        {
            foreach (OneTile buttonsOnList in listOfTiles)
            {
                int index = listOfTiles.LastIndexOf(buttonsOnList);
                int leftDistance = buttonsOnList.CoordonateLeftFromMargin;
                int topDistance = buttonsOnList.CoordonateTopFromMargin;
                int leftNeighBtn = leftDistance - (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES);
                int rightNeighBtn = leftDistance + (WIDTH_OF_TILE + DISTANCE_BETWEEN_TILES);
                int topNeighBtn = topDistance - (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES);
                int bottomNeighBtn = topDistance + (HEIGHT_OF_TILE + DISTANCE_BETWEEN_TILES);
                List<OneTile> tileOverBtns = new List<OneTile>();
                List<OneTile> tileLeftNeighBtns = new List<OneTile>();
                List<OneTile> tileRightNeighBtns = new List<OneTile>();
                tileOverBtns = listOfTiles.FindAll(s =>
                    ((
                        (s.CoordonateLeftFromMargin.CompareTo(leftNeighBtn) > 0) &&
                        (s.CoordonateLeftFromMargin.CompareTo(leftDistance) <= 0) &&
                        (s.CoordonateTopFromMargin.CompareTo(topNeighBtn) > 0) &&
                        (s.CoordonateTopFromMargin.CompareTo(topDistance) <= 0)
                    )  ||  (
                        (s.CoordonateLeftFromMargin.CompareTo(leftNeighBtn) > 0) &&
                        (s.CoordonateLeftFromMargin.CompareTo(leftDistance) <= 0) &&
                        (s.CoordonateTopFromMargin.CompareTo(bottomNeighBtn) < 0) &&
                        (s.CoordonateTopFromMargin.CompareTo(topDistance) >= 0)
                    )   ||  (
                        (s.CoordonateLeftFromMargin.CompareTo(rightNeighBtn) < 0) &&
                        (s.CoordonateLeftFromMargin.CompareTo(leftDistance) >= 0) &&
                        (s.CoordonateTopFromMargin.CompareTo(topNeighBtn) > 0) &&
                        (s.CoordonateTopFromMargin.CompareTo(topDistance) <= 0)
                    )   ||  (
                        (s.CoordonateLeftFromMargin.CompareTo(rightNeighBtn) < 0) &&
                        (s.CoordonateLeftFromMargin.CompareTo(leftDistance) >= 0) &&
                        (s.CoordonateTopFromMargin.CompareTo(bottomNeighBtn) < 0) &&
                        (s.CoordonateTopFromMargin.CompareTo(topDistance) >= 0)
                    )   ||  (
                        (s.CoordonateLeftFromMargin.Equals(leftDistance)) &&
                        (s.CoordonateTopFromMargin.Equals(topDistance))
                    ))  &&
                    (s.LayerNumber.Equals(listOfTiles[index].LayerNumber + 1))
                    &&
                    (s.State != STATE_MATCHED));
                if (tileOverBtns.Count > 0)
                {
                    listOfTiles[index].State = STATE_HAS_2NEIGHB;
                    continue;
                }
                else
                {
                    tileLeftNeighBtns = listOfTiles.FindAll(s =>
                        (s.CoordonateLeftFromMargin.CompareTo(leftNeighBtn) >= 0) &&
                        (s.CoordonateLeftFromMargin.CompareTo(leftDistance) <= 0) &&
                        (
                            ((s.CoordonateTopFromMargin.CompareTo(topNeighBtn) > 0) &&
                            (s.CoordonateTopFromMargin.CompareTo(topDistance) <= 0)
                            )   ||  (
                            (s.CoordonateTopFromMargin.CompareTo(bottomNeighBtn) < 0) &&
                            (s.CoordonateTopFromMargin.CompareTo(topDistance) >= 0))
                        )
                        &&
                        (s.LayerNumber.Equals(listOfTiles[index].LayerNumber))
                        &&
                        (s.State != STATE_MATCHED));
                    if (tileLeftNeighBtns.Count > 1)
                    {
                        bool tileIsBlocked = false;
                        foreach (OneTile tempTile in tileLeftNeighBtns)
                        {
                            if (tempTile.BtnName != buttonsOnList.BtnName)
                            {
                                if (tempTile.State != STATE_MATCHED)
                                {
                                    tileIsBlocked = true;
                                    continue;
                                }
                            }
                        }
                        if (tileIsBlocked)
                        {
                            listOfTiles[index].State = STATE_HAS_2NEIGHB;
                        }
                        else
                        {
                            if (tileLeftNeighBtns.Count != 1)
                            {
                                listOfTiles[index].State = STATE_HAS_EDGE;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (listOfTiles[index].State != STATE_MATCHED) listOfTiles[index].State = STATE_HAS_EDGE;
                        continue;
                    }

                    tileRightNeighBtns = listOfTiles.FindAll(s =>
                        (s.CoordonateLeftFromMargin.CompareTo(rightNeighBtn) <= 0) &&
                        (s.CoordonateLeftFromMargin.CompareTo(leftDistance) >= 0)
                        &&
                        (
                            (s.CoordonateTopFromMargin.CompareTo(topNeighBtn) > 0) &&
                            (s.CoordonateTopFromMargin.CompareTo(topDistance) <= 0)
                            ||
                            (s.CoordonateTopFromMargin.CompareTo(bottomNeighBtn) < 0) &&
                            (s.CoordonateTopFromMargin.CompareTo(topDistance) >= 0)
                        )
                        &&
                        (s.LayerNumber.Equals(listOfTiles[index].LayerNumber))
                        &&
                        (s.State != STATE_MATCHED));
                    if (tileRightNeighBtns.Count > 1)
                    {
                        bool tileIsBlocked = false;
                        foreach (OneTile tempTile in tileRightNeighBtns)
                        {
                            if (tempTile.BtnName != buttonsOnList.BtnName)
                            {
                                if (tempTile.State != STATE_MATCHED)
                                {
                                    tileIsBlocked = true;
                                    break;
                                }
                            }
                        }
                        if (tileIsBlocked)
                        {
                            listOfTiles[index].State = STATE_HAS_2NEIGHB;
                        }
                        else
                        {
                            if (tileLeftNeighBtns.Count != 1)
                            {
                                listOfTiles[index].State = STATE_HAS_EDGE;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if (listOfTiles[index].State != STATE_MATCHED) listOfTiles[index].State = STATE_HAS_EDGE;
                        continue;
                    }
                }
            }
        }
        
        private void checkAvailableMoves() 
        {
            // complexity should be '= 2' (Medium) or '= 3' (Hard)
            int count = 0;
            Dictionary<string, string> tempAvailableTilesList = new Dictionary<string, string>();
            foreach (var buttonsOnGrid in gridStructureClassic.Children)
                if (buttonsOnGrid is Button)
                {
                    Button _button = (Button)buttonsOnGrid;
                    
                    if (_button.Name.Substring(0, 2).Equals("bt"))
                    {
                        int index = listOfTiles.FindIndex(s => s.BtnName == _button.Name);
                        if(listOfTiles[index].State == STATE_HAS_EDGE)
                        {
                            tempAvailableTilesList.Add(_button.Name,listOfTiles[index].ShortPath);
                        }
                    }
                }
            
            Dictionary<string,List<string>> groups = tempAvailableTilesList.GroupBy(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Select(i => i.Key).ToList());
            HintPair tempHintPair = null;
            hintList = new List<HintPair>();

            while (groups.Count > 0)
            {
                if(groups.First().Value.Count > 1)
                {
                    if(groups.First().Value.Count == 2)
                    {
                        tempHintPair = new HintPair(groups.First().Value[0],groups.First().Value[1]);
                        hintList.Add(tempHintPair);
                        count++;
                    }
                    else
                        if (groups.First().Value.Count == 3)
                        {
                            tempHintPair = new HintPair(groups.First().Value[0], groups.First().Value[1]);
                            hintList.Add(tempHintPair);
                            tempHintPair = new HintPair(groups.First().Value[0], groups.First().Value[2]);
                            hintList.Add(tempHintPair);
                            tempHintPair = new HintPair(groups.First().Value[1], groups.First().Value[2]);
                            hintList.Add(tempHintPair);
                            count+=3;
                        }
                        else
                            if (groups.First().Value.Count == 4)
                            {
                                tempHintPair = new HintPair(groups.First().Value[0], groups.First().Value[1]);
                                hintList.Add(tempHintPair);
                                tempHintPair = new HintPair(groups.First().Value[0], groups.First().Value[2]);
                                hintList.Add(tempHintPair);
                                tempHintPair = new HintPair(groups.First().Value[0], groups.First().Value[3]);
                                hintList.Add(tempHintPair);
                                tempHintPair = new HintPair(groups.First().Value[1], groups.First().Value[2]);
                                hintList.Add(tempHintPair);
                                tempHintPair = new HintPair(groups.First().Value[1], groups.First().Value[3]);
                                hintList.Add(tempHintPair);
                                tempHintPair = new HintPair(groups.First().Value[2], groups.First().Value[3]);
                                hintList.Add(tempHintPair);
                                count += 6;
                            }
                }
                groups.Remove(groups.First().Key);
            }
            lblMovesShow.Content = "" + count;
            if (count == 0)
            {
                buttonHint.IsEnabled = false;
                menuHint.IsEnabled = false;
            }
            else
            {
                buttonHint.IsEnabled = true;
                menuHint.IsEnabled = true;
            }
        }

        private void showAvailableButtons()
        {
            foreach (var buttonsOnGrid in gridStructureClassic.Children)
                if (buttonsOnGrid is Button)
                {
                    Button _button = (Button)buttonsOnGrid;
                    if (_button.Name.Substring(0, 2).Equals("bt"))
                    {
                        int index = listOfTiles.FindIndex(s => s.BtnName == _button.Name);
                        
                        if (listOfTiles[index].State == STATE_HAS_2NEIGHB)
                        {
                            if (newGameIndexes.Complexity == 3) 
                            {
                                Image brush = new Image();
                                brush = new Image();
                                brush.Source = new BitmapImage(new Uri(pathToFaceDown));
                                brush.Stretch = Stretch.Fill;
                                _button.Content = brush;
                            }
                            _button.IsEnabled = false;
                        }
                        else
                        {
                            if (listOfTiles[index].State == STATE_HAS_EDGE)
                            {
                                if (newGameIndexes.Complexity == 3)
                                {
                                    Image brush = new Image();
                                    brush = new Image();
                                    int newSearch = listOfTiles.FindIndex(s => s.BtnName == _button.Name);
                                    brush.Source = new BitmapImage(new Uri(listOfTiles[newSearch].PathToImage));
                                    brush.Stretch = Stretch.Fill;
                                    _button.Content = brush;
                                }
                                _button.IsEnabled = true;
                            }
                            else 
                            {
                                _button.Visibility = Visibility.Hidden;
                            }
                        }
                    }

                }
        }

        
        private void shuffleRandomizeTiles()
        {
            initialList = new List<string>();
            randomizedList = new List<string>();
            foreach (var buttonsOnGrid in gridStructureClassic.Children)
                if (buttonsOnGrid is Button)
                {
                    Button _button = (Button)buttonsOnGrid;

                    if (_button.Name.Substring(0, 2).Equals("bt"))
                    {
                        int index = listOfTiles.FindIndex(s => s.BtnName == _button.Name);
                        if (listOfTiles[index].State > STATE_MATCHED)
                        {
                            initialList.Add(listOfTiles[index].PathToImage);
                        }
                    }
                }

            List<int> indexes = new List<int>(); // temp list of indexes. Will be used to check images which were added to randomizedList
            int countAvailableTiles = initialList.Count;
            for (int i = 0; i < countAvailableTiles; i++)
            {
                indexes.Add(i);
            }
            
            Image brush = new Image();
            string layerNumber = listOfRandomizedLayers.LastOrDefault().Key;
            foreach (var buttonsOnGrid in gridStructureClassic.Children)
                if (buttonsOnGrid is Button)
                {
                    Button _button = (Button)buttonsOnGrid;
                    if (_button.Name.Substring(0, 2).Equals("bt"))
                    {
                        int newSearch = listOfTiles.FindIndex(s => s.BtnName == _button.Name);

                        if (listOfTiles[newSearch].State != STATE_MATCHED)
                        {
                            int selectedItem = random.Next(0, indexes.Count);
                            brush = new Image();
                            brush.Source = new BitmapImage(new Uri(initialList[indexes[selectedItem]]));
                            brush.Stretch = Stretch.Fill;
                            string currentPath = initialList[indexes[selectedItem]];
                            randomizedList.Add(currentPath);

                            indexes.RemoveAt(selectedItem);
                            _button.Content = brush;

                            listOfTiles[newSearch].PathToImage = currentPath;

                            string shortPath;
                            if (currentPath.Contains(@"\seasons\"))
                            {
                                shortPath = "seasons";

                            }
                            else
                                if (currentPath.Contains(@"\flowers\"))
                                {
                                    shortPath = "flowers";

                                }
                                else shortPath = currentPath.Substring(currentPath.Length - 10, 6);

                            listOfTiles[newSearch].ShortPath = shortPath;
                        }
                        else
                        {
                            randomizedList.Add("");
                            listOfTiles[newSearch].PathToImage = "";
                            listOfTiles[newSearch].ShortPath = "";
                        }
                    }
                }
            string tempWord = "layer" + listOfRandomizedLayers.Count;
            listOfRandomizedLayers.Add(tempWord, randomizedList);
        }

        
        private void reOpenGame()
        {
            pathToFaceDown = System.IO.Path.GetFullPath(@"..\..\MahjongImages\");
            pathToFaceDown += newGame.SetOfImages + "\\faceDown.png";

            List<OneTile> tempListOfTiles = new List<OneTile>();
            tempListOfTiles = listOfTiles;
            initializeTiles();
            foreach (OneTile item in tempListOfTiles) 
            {
                if (item.PathToImage == "") { listOfTiles.FindLast(s => s.BtnName == item.BtnName).State = 0; continue; }
                if (item.PathToImage != "") 
                {
                    int index = listOfTiles.FindIndex(s => s.BtnName == item.BtnName);
                    listOfTiles[index].State = item.State;
                    listOfTiles[index].PathToImage = item.PathToImage;
                    listOfTiles[index].ShortPath = item.ShortPath;
                    continue; 
                }
            }


            var mainMenuWindowBackgroundImageUri = new Uri(pathToBackground + newGame.BackgroundCode, UriKind.Absolute);
            MainMenuWindow.Background = new ImageBrush(new BitmapImage(mainMenuWindowBackgroundImageUri));      

            showAvailableButtons();
            checkAvailableMoves();
            lblStepsShow.Content = "" + stepsList.Count + "(" + (listOfRandomizedLayers.Count - 1) + ")";
            if (hintList.Count == 0)
            {
                buttonHint.IsEnabled = false;
                menuHint.IsEnabled = false;
            }
            else
            {
                buttonHint.IsEnabled = true;
                menuHint.IsEnabled = true;
            }

            gridStructureClassic.Visibility = Visibility.Visible;

        }
 
        private void implementChangesForNewGame(GameSettingIndexes _newGameIndexes)
        {
            if (_newGameIndexes.Complexity > 1)
            {
                randomizeTilesWithStrategy();
                assignRandomizedListToListOfTilesWithStrategy();
                checkButtonsStates();
                checkAvailableMoves();
            }
            else
            {
                randomizeTiles();
                assignRandomizedListToListOfTiles();
            }
            showAvailableButtons();
        }       

        private void checkedButtonBorder(Button _button) 
        {
            var animation = new DoubleAnimation
            {
                To = 0.2,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = TimeSpan.FromSeconds(0.3),
                FillBehavior = FillBehavior.Stop
            };

            animation.Completed += (s, a) => _button.Opacity = 0.6;

            _button.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        private void uncheckedButtonBorder(Button _button) 
        {
            _button.Opacity = 1;
        }

        private void flashButton(Button _button)
        {
            var animation = new DoubleAnimation
            {
                To = 0.2,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = TimeSpan.FromSeconds(0.4),
                FillBehavior = FillBehavior.Stop
            };

            animation.Completed += (s, a) => _button.Opacity = 1;

            _button.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        private void changeStateToZero(Button _button) 
        {
            int index = listOfTiles.FindIndex(s => s.BtnName == _button.Name);
            listOfTiles[index].State = STATE_MATCHED;

        }

        private void restoreButton(Button _button, int _index)
        {
            _button.Visibility = Visibility.Visible;
            listOfTiles[_index].State = STATE_HAS_EDGE;
        }

        private void hideButton(OneStep _step)
        {
            int index1 = listOfTiles.FindIndex(s => s.BtnName == _step.FirstBtnName);
            Button _button1 = gridStructureClassic.FindName(_step.FirstBtnName) as Button;
            int index2 = listOfTiles.FindIndex(s => s.BtnName == _step.SecondBtnName);
            Button _button2 = gridStructureClassic.FindName(_step.SecondBtnName) as Button;

            _button1.Opacity = 1;
            _button2.Opacity = 1;
            changeStateToZero(_button1);
            changeStateToZero(_button2);
            _button1.Visibility = Visibility.Hidden;
            _button2.Visibility = Visibility.Hidden;
        }

        private void applyMatchedPairFromStepListToRestoredLayer()
        {
            List<OneStep> tempStepsList = new List<OneStep>();
            string tempLayer= stepsList.LastOrDefault().LayerOfTiles;
            foreach (OneStep step in stepsList) 
            {                
                if(step.LayerOfTiles == tempLayer)
                {
                    tempStepsList.Add(step);
                }
            }
            foreach (OneStep step in tempStepsList)
            {
                if (step != tempStepsList.LastOrDefault())
                {
                    hideButton(step);
                }
                else
                {
                    int index1 = listOfTiles.FindIndex(s => s.BtnName == step.FirstBtnName);
                    Button _button1 = gridStructureClassic.FindName(step.FirstBtnName) as Button;
                    int index2 = listOfTiles.FindIndex(s => s.BtnName == step.SecondBtnName);
                    Button _button2 = gridStructureClassic.FindName(step.SecondBtnName) as Button;
                    _button1.Opacity = 1;
                    _button2.Opacity = 1;
                    listOfTiles[index1].State = STATE_HAS_EDGE;
                    listOfTiles[index2].State = STATE_HAS_EDGE;
                    _button1.Visibility = Visibility.Visible;
                    _button2.Visibility = Visibility.Visible;
                }
            }
        }

        /////////////////////////////////////////
        /// METHODS END//////////////////////////
        /////////////////////////////////////////
        /// XML CODE////////////////////////////
        /////////////////////////////////////////

        public static void WriteXML()
        {

            System.Xml.Serialization.XmlSerializer GameSettingsWriter =
     new System.Xml.Serialization.XmlSerializer(typeof(GameSetting));

            System.IO.FileStream gameSettingsXML = System.IO.File.Create(GameSettingsPath);

            GameSettingsWriter.Serialize(gameSettingsXML, newGame);
            gameSettingsXML.Close();

            //////////////////////////////////////////////////

            System.Xml.Serialization.XmlSerializer GameSettingIndexesWriter =
            new System.Xml.Serialization.XmlSerializer(typeof(GameSettingIndexes));

            System.IO.FileStream GameSettingIndexesXML = System.IO.File.Create(GameSettingIndexesPath);

            GameSettingIndexesWriter.Serialize(GameSettingIndexesXML, newGameIndexes);
            GameSettingIndexesXML.Close();

            //////////////////////////////////////////////////OneTile firstSelection

            System.Xml.Serialization.XmlSerializer tileListWriter =
                new System.Xml.Serialization.XmlSerializer(typeof(List<OneTile>));

            System.IO.FileStream tileListFileStream = System.IO.File.Create(TileListFilePath);

            tileListWriter.Serialize(tileListFileStream, listOfTiles);
            tileListFileStream.Close();

            ////////////////////////////////////////////////////

            System.Xml.Serialization.XmlSerializer stepsListWriter =
               new System.Xml.Serialization.XmlSerializer(typeof(List<OneStep>));


            System.IO.FileStream stepsListXML = System.IO.File.Create(StepsListPath);

            stepsListWriter.Serialize(stepsListXML, stepsList);
            stepsListXML.Close();

            ////////////////////////////////////////////////////
            SerializeDictionaryClass sample = new SerializeDictionaryClass();
            sample.LikeProperties = listOfRandomizedLayers;

            var result = new XElement("DataItem",
                             sample.LikeProperties.Select(kvp =>
                                new XElement(kvp.Key,
                                  kvp.Value.Select(value => new XElement("value", value)))));
            result.Save(ListOfRandomizedLayersPath);            

        }
        
        public void ReadXML()
        {
            //TODO: Finish off these methods with a working filepicker 


            System.Xml.Serialization.XmlSerializer gameSettingReader =
       new System.Xml.Serialization.XmlSerializer(typeof(GameSetting));

            System.IO.StreamReader GameSettingFile = new System.IO.StreamReader(GameSettingsPath);
            newGame = (GameSetting)gameSettingReader.Deserialize(GameSettingFile);
            GameSettingFile.Close();

             /////////////////////////////////////////////////////////////////////////////////////////////////////

            System.Xml.Serialization.XmlSerializer gameSettingIndexesReader =
      new System.Xml.Serialization.XmlSerializer(typeof(GameSettingIndexes));

            System.IO.StreamReader gameSettingIndexesFile = new System.IO.StreamReader(GameSettingIndexesPath);
            newGameIndexes = (GameSettingIndexes)gameSettingIndexesReader.Deserialize(gameSettingIndexesFile);
            gameSettingIndexesFile.Close();

            ////////////////////////////////////////////////////////////////////////////////////////////////////

            System.Xml.Serialization.XmlSerializer tileListReader =
            new System.Xml.Serialization.XmlSerializer(typeof(List<OneTile>));

            System.IO.StreamReader tileListFile = new System.IO.StreamReader(TileListFilePath);
            listOfTiles = (List<OneTile>)tileListReader.Deserialize(tileListFile);
            tileListFile.Close();

            ////////////////////////////////////////////////////////////////////////////////////////////////////

            System.Xml.Serialization.XmlSerializer stepsListReader =
            new System.Xml.Serialization.XmlSerializer(typeof(List<OneStep>));

            System.IO.StreamReader stepsListFile = new System.IO.StreamReader(StepsListPath);
            stepsList = (List<OneStep>)stepsListReader.Deserialize(stepsListFile);
            stepsListFile.Close();

            ////////////////////////////////////////////////////////////////////////////////////////////////////

            listOfRandomizedLayers = new Dictionary<string, List<string>>();


            SerializeDictionaryClass newDict = new SerializeDictionaryClass();
            newDict.LikeProperties = XElement.Load(ListOfRandomizedLayersPath).Elements().ToDictionary(
                                          e => e.Name.LocalName,
                                          e => e.Elements().Select(v => (string)v).ToList());


            foreach(var item in newDict.LikeProperties)
            {
                listOfRandomizedLayers.Add(item.Key,item.Value);
            }

            ///////////////////////////////////////////////////VR - INIT GAME CODE /////////////////////////////////////////////////////

            reOpenGame();
        }

    /////////////////////////////////////////
    /////////////////////////////////////////
    //////////////XML CODE END///////////////
    /////////////////////////////////////////

    /////////////////////////////////////////
    /////////////////////////////////////////
    /// PLAY WAV FILES///////////////////////
    /////////////////////////////////////////

        
        private void playSoundFile(string pathToSound)
        {
            mediaPlayer.Open(new Uri(pathToSound));
            mediaPlayer.Play();
        }
        
        private void stopSoundFile()
    {
        mediaPlayer.Stop();
    }

        /////////////////////////////////////////
        /// PLAY WAV FILES END///////////////////
        /////////////////////////////////////////





        /////////////////////////////////////////
        /// EVENT HANDLERS///////////////////////
        /////////////////////////////////////////

        private void menuNew_Click(object sender, RoutedEventArgs e)
        {
            NewGameSettingsWindow newGameSettingWindow = new NewGameSettingsWindow();
            newGameSettingWindow.Owner = this;
            newGameSettingWindow.CurrentItem = newGame;


            newGameSettingWindow.cbxListOfStructures.SelectedIndex = newGameIndexes.Structure;
            newGameSettingWindow.cbxListOfBackgroundImages.SelectedIndex = newGameIndexes.BackgroundCode;
            newGameSettingWindow.cbxListOfSetsPieces.SelectedIndex = newGameIndexes.SetOfImages;
            if (newGameIndexes.Complexity == 0)
            {
                newGameSettingWindow.cbxGameComplexity.SelectedIndex = 0;
            }
            else
            {
                newGameSettingWindow.cbxGameComplexity.SelectedIndex = newGameIndexes.Complexity - 1;
            }

            newGameSettingWindow.ShowDialog();
            if (newGameStarted)
            {
                newGame.Structure = newGameSettingWindow.CurrentItem.Structure;
                newGame.BackgroundCode = newGameSettingWindow.CurrentItem.BackgroundCode + ".jpg";
                newGame.SetOfImages = newGameSettingWindow.CurrentItem.SetOfImages + "Set";
                newGame.Complexity = newGameSettingWindow.CurrentItem.Complexity;
                newGameIndexes.Structure = newGameSettingWindow.cbxListOfStructures.SelectedIndex;
                newGameIndexes.BackgroundCode = newGameSettingWindow.cbxListOfBackgroundImages.SelectedIndex;
                newGameIndexes.SetOfImages = newGameSettingWindow.cbxListOfSetsPieces.SelectedIndex;
                newGameIndexes.Complexity = newGameSettingWindow.cbxGameComplexity.SelectedIndex + 1;

                gridStructureClassic.Visibility = Visibility.Visible;
                initNewGame();
                implementChangesForNewGame(newGameIndexes);

                if (newGameIndexes.Complexity > 1)
                {
                    buttonShuffle.Visibility = Visibility.Visible;
                    buttonShuffle.Style = (Style)FindResource("otherBTN");
                    buttonUndo.Visibility = Visibility.Visible;
                    buttonUndo.Style = (Style)FindResource("otherBTN");
                    buttonHint.Visibility = Visibility.Visible;
                    buttonHint.Style = (Style)FindResource("otherBTN");

                    lblMoves.Visibility = Visibility.Visible;
                    lblMovesShow.Visibility = Visibility.Visible;
                    lblSteps.Visibility = Visibility.Visible;
                    lblStepsShow.Visibility = Visibility.Visible;

                    menuShuffle.IsEnabled = true;
                    if(hintList.Count != 0 ) menuHint.IsEnabled = true;
                }
                else
                {
                    buttonShuffle.Visibility = Visibility.Hidden;
                    buttonUndo.Visibility = Visibility.Hidden;
                    buttonHint.Visibility = Visibility.Hidden;

                    lblMoves.Visibility = Visibility.Hidden;
                    lblMovesShow.Visibility = Visibility.Hidden;
                    lblSteps.Visibility = Visibility.Hidden;
                    lblStepsShow.Visibility = Visibility.Hidden;

                    menuShuffle.IsEnabled = false;
                    menuHint.IsEnabled = false;
                }
                newGameStarted = false;
                buttonUndo.IsEnabled = false;
                menuUndo.IsEnabled = false;
            }
        }

        private void menuOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ReadXML();
                if (newGameIndexes.Complexity > 1)
                {
                    buttonShuffle.Visibility = Visibility.Visible;
                    buttonShuffle.Style = (Style)FindResource("otherBTN");
                    buttonUndo.Visibility = Visibility.Visible;
                    buttonUndo.Style = (Style)FindResource("otherBTN");
                    buttonHint.Visibility = Visibility.Visible;
                    buttonHint.Style = (Style)FindResource("otherBTN");

                    lblMoves.Visibility = Visibility.Visible;
                    lblMovesShow.Visibility = Visibility.Visible;
                    lblSteps.Visibility = Visibility.Visible;
                    lblStepsShow.Visibility = Visibility.Visible;

                    menuShuffle.IsEnabled = true;
                    if (hintList.Count != 0) menuHint.IsEnabled = true;
                }
                else
                {
                    buttonShuffle.Visibility = Visibility.Hidden;
                    buttonUndo.Visibility = Visibility.Hidden;
                    buttonHint.Visibility = Visibility.Hidden;

                    lblMoves.Visibility = Visibility.Hidden;
                    lblMovesShow.Visibility = Visibility.Hidden;
                    lblSteps.Visibility = Visibility.Hidden;
                    lblStepsShow.Visibility = Visibility.Hidden;

                    menuShuffle.IsEnabled = false;
                    menuHint.IsEnabled = false;
                }
                if (stepsList.Count > 0) 
                { 
                    buttonUndo.IsEnabled = true; 
                    menuUndo.IsEnabled = true;
                } 
                else 
                { 
                    buttonUndo.IsEnabled = false; 
                    menuUndo.IsEnabled = false;
                }
                newGameStarted = false;
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(" " + ex);
            }
        }

        private void menuSaveAs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WriteXML();
            }
            catch (Exception ex)
            {
                MessageBox.Show(" " + ex);
            }
            
            
        }

        private void menuBackground_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow_Backgrounds newHelpWindow = new HelpWindow_Backgrounds();
            newHelpWindow.Owner = this;
            newHelpWindow.ShowDialog();
            if (newBackgroundSelected)
            {
                var structureBackgroundImageUri = new Uri(pathToBackground + newGame.BackgroundCode, UriKind.Absolute);
                this.Background = new ImageBrush(new BitmapImage(structureBackgroundImageUri));
                newBackgroundSelected = false;
            }
        }

        private void menuReplay_Click(object sender, RoutedEventArgs e)
        {
            while (listOfRandomizedLayers.Count > 1)
            {
                listOfRandomizedLayers.Remove(listOfRandomizedLayers.Last().Key);
            }
            stepsList = new List<OneStep>();
            lblStepsShow.Content = "" + stepsList.Count + "(" + (listOfRandomizedLayers.Count - 1) + ")";
            //assignRandomizedListToListOfTiles();
            assignRandomizedListToListOfTilesWithStrategy();
            foreach (OneTile tempTile in listOfTiles)
            {
                if (tempTile.State != STATE_HAS_2NEIGHB)
                {
                    tempTile.State = STATE_HAS_2NEIGHB;
                }
            }
            checkButtonsStates();
            checkAvailableMoves();
            showAvailableButtons();
            buttonUndo.IsEnabled = false;
            menuUndo.IsEnabled = false;
            menuReplay.IsEnabled = false;
            firstSelectedButton = null;
            firstSelection = null;
        }
 
        private void buttonUndo_Click(object sender, RoutedEventArgs e)
        {
            if (stepsList.LastOrDefault().LayerOfTiles != listOfRandomizedLayers.LastOrDefault().Key)
            {
                string lastMatchedPairLayer = stepsList.LastOrDefault().LayerOfTiles;                
                string tempIndex = listOfRandomizedLayers.LastOrDefault().Key;
                while (tempIndex != lastMatchedPairLayer)
                {
                    listOfRandomizedLayers.Remove(tempIndex);
                    tempIndex = listOfRandomizedLayers.LastOrDefault().Key;
                }
                lblStepsShow.Content = "" + stepsList.Count + "(" + (listOfRandomizedLayers.Count - 1) + ")";
                //assignRandomizedListToListOfTiles();
                assignRandomizedListToListOfTilesWithStrategy();
                checkButtonsStates();
                applyMatchedPairFromStepListToRestoredLayer();
            }
            else
            {
                int index = listOfTiles.FindIndex(s => s.BtnName == stepsList.LastOrDefault().FirstBtnName);
                Button _button = gridStructureClassic.FindName(stepsList.LastOrDefault().FirstBtnName) as Button;
                restoreButton((Button)_button, index);

                index = listOfTiles.FindIndex(s => s.BtnName == stepsList.LastOrDefault().SecondBtnName);
                _button = (Button)gridStructureClassic.FindName(stepsList.LastOrDefault().SecondBtnName);
                restoreButton((Button)_button, index);
            }

            stepsList.RemoveAt(stepsList.Count - 1);
            lblStepsShow.Content = "" + stepsList.Count + "(" + (listOfRandomizedLayers.Count - 1) + ")";
            checkButtonsStates();
            checkAvailableMoves();
            showAvailableButtons();
            if (stepsList.Count == 0)
            {
                buttonUndo.IsEnabled = false;
                menuUndo.IsEnabled = false;
                menuReplay.IsEnabled = false;
            }
            firstSelectedButton = null;
            firstSelection = null;
        }

        private void buttonHint_Click(object sender, RoutedEventArgs e)
        {
            int countHints = hintList.Count;
            if (displayHint >= countHints)
            {
                displayHint = displayHint % countHints;
            }
            Button _button = gridStructureClassic.FindName(hintList[displayHint].Btn1) as Button;
            Button _button2 = gridStructureClassic.FindName(hintList[displayHint].Btn2) as Button;
            flashButton(_button);
            flashButton(_button2);
            displayHint++;
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnTiles_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            buttonHint.IsEnabled = true;
            menuHint.IsEnabled = true;
            if (button != null)
            {
                int index = listOfTiles.FindIndex(s => s.BtnName == button.Name);
                if (firstSelection != null)
                {
                    if (firstSelectedButton.Name != button.Name)
                    {
                        if (firstSelection.ShortPath == listOfTiles[index].ShortPath)
                        {
                            firstSelectedButton.Opacity = 1;
                            button.Opacity = 1;
                            changeStateToZero(firstSelectedButton);
                            changeStateToZero(button);
                            OneStep tempVar = new OneStep(firstSelectedButton.Name, button.Name, listOfRandomizedLayers.LastOrDefault().Key);
                            stepsList.Add(tempVar);
                            lblStepsShow.Content = "" + stepsList.Count + "(" + (listOfRandomizedLayers.Count - 1) + ")";
                            firstSelectedButton.Visibility = Visibility.Hidden;
                            button.Visibility = Visibility.Hidden;
                            firstSelectedButton = null;
                            firstSelection = null;
                        }
                        else
                        {
                            firstSelectedButton.Opacity = 1;
                            button.Opacity = 1;
                            uncheckedButtonBorder(firstSelectedButton);
                            firstSelectedButton = button;
                            firstSelection = listOfTiles[index];
                            checkedButtonBorder(button);
                        }
                    }
                    buttonUndo.IsEnabled = true;
                    menuUndo.IsEnabled = true;
                    menuReplay.IsEnabled = true;
                }
                else
                {
                    firstSelectedButton = sender as Button;
                    string tempName = firstSelectedButton.Name;
                    firstSelection = listOfTiles.Find(s => s.BtnName == tempName);
                    checkedButtonBorder(firstSelectedButton);
                }
                if (newGameIndexes.Complexity > 1)
                {
                    checkButtonsStates();
                    checkAvailableMoves();
                }
                showAvailableButtons();
            }
        }

        private void btnTiles_MouseRightButtonDown(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                int index = listOfTiles.FindIndex(s => s.BtnName == button.Name);
                cm.Items[0] = "Button: \t\t" + button.Name;
                string tempStr = listOfTiles[index].PathToImage.Substring(58);
                cm.Items[1] = "ImagePath: \t" + tempStr;
                cm.Items[3] = "State: \t\t" + listOfTiles[index].State;
                cm.Items[4] = "Layer: \t\t" + listOfTiles[index].LayerNumber;
            }
            cm.MouseLeave += (sender2, events) =>
            {
                cm.Items[0] = "Button: \t\t" + "No info";
                cm.Items[1] = "ImagePath: \t" + "No info";
                cm.Items[3] = "State: \t\t" + "No info";
                cm.Items[4] = "Layer: \t\t" + "No info";
            };
        }

        private void changeSetOfImagesInListOfRandomizedLayers(String oldSetOfImages, String newSetOfImages)
        {
            Dictionary<string, List<string>> tempListOfLayers = new Dictionary<string, List<string>>(); 
            foreach (var str in listOfRandomizedLayers)
            {
                List<string> tempListStrings = new List<string>();
                foreach (string pathString in str.Value)
                {
                    if(pathString.Contains(oldSetOfImages))
                    {
                        tempListStrings.Add(pathString.Replace(oldSetOfImages, newSetOfImages));
                    }
                }
                tempListOfLayers.Add(str.Key, tempListStrings);
            }
            listOfRandomizedLayers = new Dictionary<string, List<string>>(tempListOfLayers);
        }

        private void changeSetOfImagesInListOfTiles(String oldSetOfImages, String newSetOfImages)
        {
            foreach (var str in listOfTiles)
            {
                string pathString = str.PathToImage;
                if (pathString.Contains(oldSetOfImages))
                {
                    str.PathToImage = str.PathToImage.Replace(oldSetOfImages, newSetOfImages);
                }
            }
        }


        private void changeSetOfImagesOnGrid() 
        {
            Image brush = new Image();
            int count = 0;
            foreach (var buttonsOnGrid in gridStructureClassic.Children)
                if (buttonsOnGrid is Button)
                {
                    Button _button = (Button)buttonsOnGrid;
                    if (_button.Name.Substring(0, 2).Equals("bt"))
                    {
                        brush = new Image();
                        brush.Source = new BitmapImage(new Uri(listOfTiles[count].PathToImage));
                        brush.Stretch = Stretch.Fill;
                        _button.Content = brush;
                        count++;
                    }
                }
        }

        private void buttonShuffle_Click(object sender, RoutedEventArgs e)
        {
            bool check = false;
            while (!check)
            {
                shuffleRandomizeTiles();
                //checkButtonsStates();
                checkAvailableMoves();
                if (hintList.Count != 0) check = true;
            }
            showAvailableButtons();
            lblStepsShow.Content = "" + stepsList.Count + "(" + (listOfRandomizedLayers.Count - 1) + ")";
        }

        private void menuSetImages_Click(object sender, RoutedEventArgs e)
        {
            String oldSetOfImages = newGame.SetOfImages;
            HelpWindow_SetImages newHelpWindow = new HelpWindow_SetImages();
            newHelpWindow.Owner = this;
            newHelpWindow.ShowDialog();
            
            if (newSetOfPiecesSelected)
            {
                String newSetOfImages = newGame.SetOfImages;
                changeSetOfImagesInListOfRandomizedLayers(oldSetOfImages, newSetOfImages);
                changeSetOfImagesInListOfTiles(oldSetOfImages, newSetOfImages);
                changeSetOfImagesOnGrid();
                showAvailableButtons();

                newSetOfPiecesSelected = false;
            }
        }

        private void menuMainHelp_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow newHelpWindow = new HelpWindow();
            newHelpWindow.Owner = this;
            newHelpWindow.ShowDialog();
        }

        private void buttonTest_Click(object sender, RoutedEventArgs e)
        {
            string tempStr = "";
            int count = 0;
            foreach (OneStep tempStep in strategyToWin)
            {
                int index1 = listOfTiles.FindIndex(s => s.BtnName == tempStep.FirstBtnName);
                int index2 = listOfTiles.FindIndex(s => s.BtnName == tempStep.SecondBtnName);
                string stateTile1 = "";
                string stateTile2 = "";
                if (listOfTiles[index1].State == STATE_MATCHED)
                {
                    stateTile1 = String.Format("\b{0}", tempStep.FirstBtnName);
                }
                else
                {
                    stateTile1 = tempStep.FirstBtnName;
                }
                if (listOfTiles[index2].State == STATE_MATCHED)
                {
                    stateTile2 = String.Format("\b{0}", tempStep.SecondBtnName);
                }
                else
                {
                    stateTile2 = tempStep.SecondBtnName;
                }
                if (tempStep.FirstBtnName.Length < 4)
                {
                    tempStr += stateTile1 + ",   " + stateTile2;
                }
                else
                {
                    if (tempStep.FirstBtnName.Length < 5)
                    {
                        tempStr += stateTile1 + ",  " + stateTile2;
                    }
                    else
                    {
                        tempStr += stateTile1 + ", " + stateTile2;
                    }
                }
                if (tempStep.SecondBtnName.Length < 4)
                {
                    tempStr += ";   ";
                }
                else
                {
                    if (tempStep.SecondBtnName.Length < 5)
                    {
                        tempStr += ";  ";
                    }
                    else
                    {
                        tempStr += "; ";
                    }
                }
                if (count % 2 == 0)
                {
                    tempStr += "\t";
                }
                else
                {
                    tempStr += "\n";
                }
                count++;
            }
            MessageBox.Show(tempStr);
        }

        /////////////////////////////////////////
        /// EVENT HANDLERS END///////////////////
        /////////////////////////////////////////       
    }
}
