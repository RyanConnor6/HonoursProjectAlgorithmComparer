using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

// RYAN CONNOR - 40437041
// HONOURS PROJECT
// SHORTEST PATH ALGORITHM COMPARER
// IMPLEMENTATION STARTED 21/11/22
// LAST UPDATE 30/11/22

//Main Program Page for Honours Project
namespace HonoursProjectAlgorithmComparer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //Variables for grid
        public List<StackPanel> panelList = new();
        public Grid panelGrid = new Grid();

        //Start and goal nodes
        private Node? first;
        private Node? last;

        //Connection manager
        private ConnectionManager cm;

        //Grid size
        private int size = 0;

        //Currernt mode eg. place wall, move start point etc.
        private string mode = "null";

        //Stackpanel variables
        private StackPanel? laststp;
        private Brush lastStpColourStart = Brushes.MintCream;
        private Brush lastStpColourEnd = Brushes.MintCream;

        //Comparison variables
        private string currentRun = "";
        private double currentTime = 0;
        private int currentExplored = 0;
        private int currentSize = 0;
        private string bestRun = "";
        private double bestTime = 9999;
        private int bestExplored = 9999;
        private int bestSize = 9999;

        //Stats variables
        private double aStarTime = 9999;
        private double aStarAverage = 9999;
        private int aStarSize = 9999;
        private int aStarExplored = 9999;
        private int aStarRuns = 9999;
        private double dijkstraTime = 9999;
        private double dijkstraAverage = 9999;
        private int dijkstraSize = 9999;
        private int dijkstraExplored = 9999;
        private int dijkstraRuns = 9999;
        private double breadthFirstTime = 9999;
        private double breadthFirstAverage = 9999;
        private int breadthFirstSize = 9999;
        private int breadthFirstExplored = 9999;
        private int breadthFirstRuns = 9999;
        private double greedyBestFirstTime = 9999;
        private double greedyBestFirstAverage = 9999;
        private int greedyBestFirstSize = 9999;
        private int greedyBestFirstExplored = 9999;
        private int greedyBestFirstRuns = 9999;
        private int algSelected = 0;

        //Cancellation token
        private CancellationTokenSource? cts;

        //File variables
        int noFiles = 0;


        //Main Window Initialise
        public MainWindow()
        {
            //Initialise window
            InitializeComponent();

            //Update size and files no
            noFiles = 0;
            size = 20;

            //Get files
            var folder = @"C:\[0] HonsProj\[0] VS Proj\HonoursProjectAlgorithmComparer\HonoursProjectAlgorithmComparer\bin\Debug\net6.0-windows";
            var txtFiles = Directory.GetFiles(folder, $"GridSize{size:D2}LayoutNo*.txt").ToList();

            //Add files to combo box for selection
            foreach (string tfile in txtFiles)
            {
                string[] words = tfile.Split(@"\");
                var myFile = words[words.Length - 1];
                LayoutBox.Items.Add(myFile);
                noFiles++;
            }

            //Start on item 0
            LayoutBox.SelectedIndex = 0;

            //Start basic variables as null
            first = null;
            last = null;
            mode = "null";

            //Retrieve size from combobox and create grid
            ComboBoxItem myItem = (ComboBoxItem)comboBox1.SelectedItem;
            int myOption = comboBox1.Items.IndexOf(myItem);
            switch (myOption)
            {
                case 0:
                    size = 20;
                    creategrid(size);
                    break;
                case 1:
                    size = 25;
                    creategrid(size);
                    break;
                case 2:
                    size = 30;
                    creategrid(size);
                    break;
                case 3:
                    size = 35;
                    creategrid(size);
                    break;
                case 4:
                    size = 40;
                    creategrid(size);
                    break;
                case 5:
                    size = 45;
                    creategrid(size);
                    break;
            }

            //Create connections manager
            cm = new ConnectionManager(size, this);

            //Place start and goal node near centre of grid
            first = cm.NodesList[(size * (size / 2)) + 1];
            last = cm.NodesList[(size * (size / 2)) + size - 2];
        }

        //Start Button clicked
        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            //Stop any running instances of a path searcher
            ctsStop();

            //Reset variables
            first = null;
            last = null;
            mode = "null";

            //Retrieve grid size from combobox and create grid
            ComboBoxItem myItem = (ComboBoxItem)comboBox1.SelectedItem;
            int myOption = comboBox1.Items.IndexOf(myItem);
            switch (myOption)
            {
                case 0:
                    size = 20;
                    creategrid(size);
                    break;
                case 1:
                    size = 25;
                    creategrid(size);
                    break;
                case 2:
                    size = 30;
                    creategrid(size);
                    break;
                case 3:
                    size = 35;
                    creategrid(size);
                    break;
                case 4:
                    size = 40;
                    creategrid(size);
                    break;
                case 5:
                    size = 45;
                    creategrid(size);
                    break;
            }

            //reset load box
            LayoutBox.Items.Clear();
            noFiles = 0;

            //get files from folder
            var folder = @"C:\[0] HonsProj\[0] VS Proj\HonoursProjectAlgorithmComparer\HonoursProjectAlgorithmComparer\bin\Debug\net6.0-windows";
            var txtFiles = Directory.GetFiles(folder, $"GridSize{size:D2}LayoutNo*.txt").ToList();

            //Add files to combo box
            foreach (string tfile in txtFiles)
            {
                string[] words = tfile.Split(@"\");
                var myFile = words[words.Length - 1];
                LayoutBox.Items.Add(myFile);
                noFiles++;
            }

            //Start on item 0
            LayoutBox.SelectedIndex = 0;

            //Create new connection manager
            cm = new ConnectionManager(size, this);

            //Place start and goal nodes near centre of grid
            first = cm.NodesList[(size * (size / 2)) + 1];
            last = cm.NodesList[(size * (size / 2)) + size - 2];
        }

        //Create the grid
        public void creategrid(int size)
        {
            //reset labels
            resetLabels();

            //Setup panel grid
            panelGrid.Children.Clear();
            panelGrid.RowDefinitions.Clear();
            panelGrid.ColumnDefinitions.Clear();
            this.canContainer.Children.Clear();
            panelList.Clear();
            panelGrid.Width = canContainer.Width;
            panelGrid.Height = canContainer.Height;
            panelGrid.HorizontalAlignment = HorizontalAlignment.Center;
            panelGrid.VerticalAlignment = VerticalAlignment.Center;
            panelGrid.ShowGridLines = false;

            //Define the Columns
            for (int i = 0; i < size; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                panelGrid.ColumnDefinitions.Add(cd);
                RowDefinition rd = new RowDefinition();
                panelGrid.RowDefinitions.Add(rd);
            }

            //Create Stack Panels
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    StackPanel stp = new StackPanel();
                    stp.Background = Brushes.MintCream;
                    stp.Name = "c" + (j + 1) + "c" + (i + 1);
                    Grid.SetColumn(stp, j);
                    Grid.SetRow(stp, i);
                    stp.MouseMove += PlacementControl;
                    stp.MouseDown += PlacementControl2;
                    panelGrid.Children.Add(stp);
                    panelList.Add(stp);
                }
            }

            //Add to canvas
            panelGrid.ShowGridLines = true;
            this.canContainer.Children.Add(panelGrid);

            //Place start and goal nodes near centre of grid
            panelList[(size * (size / 2)) + 1].Background = Brushes.Green;
            panelList[(size * (size / 2)) + size - 2].Background = Brushes.Red;
        }

        //Update colour on grid
        public void updatecol(int ID, Brush color)
        {
            //Update panel relating to node ID
            panelList[ID - 1].Background = color;
        }

        //Update placement of element on grid
        private void PlacementControl(object sender, RoutedEventArgs e)
        {
            //Get panel
            StackPanel stp = (StackPanel)sender;

            //Draws elements when mouse is held
            bool mouseIsDown = System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed;
            if (mouseIsDown)
            {
                //Stop any running instances of a path searcher
                ctsStop();

                //If an algorithm was run remove visualised search
                if (mode.Equals("running"))
                {
                    resetSearchVisualisation();
                    switchMode(stp);

                    //reset labels
                    resetLabels();
                }

                //If the panel isnt the same as was on on previous call, change colour
                if (stp != laststp)
                {
                    UpdatePanelBasedOnMode(stp);
                }

                //Update last visited stackpanel
                laststp = stp;
            }
            else
            {
                //If mouse isnt held and no algorithm is running switchmode based on hover
                if (mode.Equals("running")!=true)
                {
                    switchMode(stp);
                    lastStpColourStart = Brushes.MintCream;
                    lastStpColourEnd = Brushes.MintCream;
                }
            }
        }

        //Update placement of element on grid
        private void PlacementControl2(object sender, RoutedEventArgs e)
        {
            //Adds elements when clicked
            StackPanel stp = (StackPanel)sender;

            //Stop any running instances of a path searcher
            ctsStop();

            //If an algorithm was run remove visualised search
            if (mode.Equals("running"))
            {
                resetSearchVisualisation();
                switchMode(stp);

                //reset labels
                resetLabels();
            }

            //Update panel colour
            switchMode(stp);
            UpdatePanelBasedOnMode(stp);
        }

        //Update panel based upon mode
        private void UpdatePanelBasedOnMode(StackPanel stp)
        {
            //Get coordinate from stackpanel name
            string[] subs = stp.Name.Split('c');

            //If start mode, move start
            if (mode.Equals("placestart"))
            {
                int x = Int32.Parse(subs[1]);
                int y = Int32.Parse(subs[2]);
                if (first != null)
                {
                    updatecol(first.NodeID, lastStpColourStart);
                }

                var counter = 0;
                foreach (Node n in cm.NodesList)
                {
                    if (n.CoordinateX == x && n.CoordinateY == y)
                    {
                        if (n.NodeID != last!.NodeID)
                        {
                            lastStpColourStart = panelList[counter].Background;
                            updatecol(n.NodeID, Brushes.Green);
                            first = n;
                        }
                        else
                        {
                            updatecol(first!.NodeID, Brushes.Green);
                        }
                    }
                    counter++;
                }
            }
            //If end mode, move end
            else if (mode.Equals("placeend"))
            {
                int x = Int32.Parse(subs[1]);
                int y = Int32.Parse(subs[2]);
                if (last != null)
                {
                    updatecol(last.NodeID, lastStpColourEnd);
                }

                var counter = 0;
                foreach (Node n in cm.NodesList)
                {
                    if (n.CoordinateX == x && n.CoordinateY == y)
                    {
                        if (n.NodeID != first!.NodeID)
                        {
                            lastStpColourEnd = panelList[counter].Background;
                            updatecol(n.NodeID, Brushes.Red);
                            last = n;
                        }
                        else
                        {
                            updatecol(last!.NodeID, Brushes.Red);
                        }
                    }
                    counter++;
                }
            }
            //If wall mode, place wall
            else if (mode.Equals("placewall"))
            {
                int x = Int32.Parse(subs[1]);
                int y = Int32.Parse(subs[2]);

                foreach (Node n in cm.NodesList)
                {
                    if (n.CoordinateX == x && n.CoordinateY == y)
                    {
                        if (stp.Background == Brushes.MintCream)
                        {
                            updatecol(n.NodeID, Brushes.Black);
                        }
                    }
                }
            }
            //If remove wall mode, remove wall
            else if (mode.Equals("removewall"))
            {
                int x = Int32.Parse(subs[1]);
                int y = Int32.Parse(subs[2]);

                foreach (Node n in cm.NodesList)
                {
                    if (n.CoordinateX == x && n.CoordinateY == y)
                    {
                        if (stp.Background == Brushes.Black)
                        {
                            updatecol(n.NodeID, Brushes.MintCream);
                        }
                    }
                }
            }
        }

        //Run the algorithm
        private void runBtn_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem myItem = (ComboBoxItem)comboboxRun.SelectedItem;
            int myOption = comboboxRun.Items.IndexOf(myItem);

            int runAmount = 1;
            switch (myOption)
            {
                case 0:
                    runAmount = 1;
                    runAlgorithm(runAmount);
                    return;
                case 1:
                    runAmount = 5;
                    runAlgorithm(runAmount);
                    return;
                case 2:
                    runAmount = 10;
                    runAlgorithm(runAmount);
                    return;
                case 3:
                    runAmount = 20;
                    runAlgorithm(runAmount);
                    return;
            }
        }

        private async void runAlgorithm(int runAmount)
        {
            for (int i = 0; i < runAmount; i++)
            {
                //Setup grid for run
                setupForRun();

                //Get algorithm from combobox
                ComboBoxItem myItem2 = (ComboBoxItem)comboBox2.SelectedItem;
                int myOption2 = comboBox2.Items.IndexOf(myItem2);

                //Reset parents
                foreach (Node a in cm.NodesList)
                {
                    a.Parent = null!;
                }

                //Create cancellation token
                cts = new CancellationTokenSource();

                //Run speed for algorithms
                var runSpeed = 10;

                //Run correct mode
                if (myOption2 == 0)
                {
                    currentRun = "A*";
                    AStarRunner runAStar = new AStarRunner(cm);
                    await runAStar.algRun(first!, last!, cts.Token, runSpeed);
                    await pathMade();
                }
                if (myOption2 == 1)
                {
                    currentRun = "Dijkstra's";
                    DijkstraRunner runDijkstra = new DijkstraRunner(cm);
                    await runDijkstra.algRun(first!, last!, cts.Token, runSpeed);
                    await pathMade();
                }
                if (myOption2 == 2)
                {
                    currentRun = "Breadth First";
                    BreadthFirstRunner runBreadthFirst = new BreadthFirstRunner(cm);
                    await runBreadthFirst.algRun(first!, last!, cts.Token, runSpeed);
                    await pathMade();
                }
                if (myOption2 == 3)
                {
                    currentRun = "Greedy Best First";
                    BestFirstRunner runBestFirst = new BestFirstRunner(cm);
                    await runBestFirst.algRun(first!, last!, cts.Token, runSpeed);
                    await pathMade();
                }
            }
        }

        //Set up grid for run
        private void setupForRun()
        {
            //Stop any running instances of a path searcher
            ctsStop();

            //If no connection manager throw error
            if (cm == null)
            {
                MessageBox.Show("ERROR: Please create a grid");
                return;
            }

            //Set mode to run
            mode = "running";

            //Construct node network
            bool diagonalAllowed = true;
            if ((bool)Diagonal.IsChecked! == false)
            {
                diagonalAllowed = false;
            }

            //Get distance type from combobox
            ComboBoxItem myItem = (ComboBoxItem)comboBox3.SelectedItem;
            int myOption = comboBox3.Items.IndexOf(myItem);

            //Construct new network using options
            cm.ConstructNetwork(diagonalAllowed, myOption);

            //Keep list of walls
            List<StackPanel> walls = new();

            //Fully set up grid for algorithm
            var iterator = 0;
            foreach (StackPanel stp in panelList)
            {
                if (stp.Background == Brushes.Black)
                {
                    foreach (Node n in cm.NodesList[iterator].ConnectedNodes)
                    {
                        n.ConnectedNodes.Remove(cm.NodesList[iterator]);
                    }
                    cm.NodesList[iterator].ConnectedNodes.Clear();
                    walls.Add(stp);
                }
                if (stp.Background == Brushes.Green)
                {
                    string[] subs = stp.Name.Split('c');
                    int x = Int32.Parse(subs[1]);
                    int y = Int32.Parse(subs[2]);
                    foreach (Node n in cm.NodesList)
                    {
                        if (n.CoordinateX == x && n.CoordinateY == y)
                        {
                            first = n;
                        }
                    }
                }
                if (stp.Background == Brushes.Red)
                {
                    string[] subs = stp.Name.Split('c');
                    int x = Int32.Parse(subs[1]);
                    int y = Int32.Parse(subs[2]);
                    foreach (Node n in cm.NodesList)
                    {
                        if (n.CoordinateX == x && n.CoordinateY == y)
                        {
                            last = n;
                        }
                    }
                }
                iterator++;
            }
            foreach (StackPanel stp in panelList)
            {
                stp.Background = Brushes.MintCream;
            }

            foreach (StackPanel stp in walls)
            {
                stp.Background = Brushes.Black;
            }

            //Show start and goal
            updatecol(first!.NodeID, Brushes.Green);
            updatecol(last!.NodeID, Brushes.Red);
        }

        //Reset tiles that were coloured by the search
        private void resetSearchVisualisation()
        {
            foreach (StackPanel stp in panelList)
            {
                if (stp.Background == Brushes.GreenYellow || stp.Background == Brushes.LightGreen || stp.Background == Brushes.Khaki)
                {
                    stp.Background = Brushes.MintCream;
                }
            }
        }

        //Switch mode based on which stackpanel is hovered over
        private void switchMode(StackPanel stp)
        {
            if (stp.Background == Brushes.Green)
            {
                mode = "placestart";
            }
            if (stp.Background == Brushes.Red)
            {
                mode = "placeend";
            }
            if (stp.Background == Brushes.MintCream)
            {
                mode = "placewall";
            }
            if (stp.Background == Brushes.Black)
            {
                mode = "removewall";
            }
        }

        //Show results of the run
        public void showResults(double time, int pathSize)
        {    
            algLabel.Content = "Algorithm Name: " + currentRun;
            currentTime = time;
            algLabel2.Content = "Run Time: " + time.ToString("0.00");
            currentSize = pathSize;
            algLabel4.Content = "Path Size: " + pathSize.ToString();

            if (currentSize <= bestSize)
            {
                if(currentSize < bestSize)
                {
                    bestRun = currentRun;
                    bestTime = currentTime;
                    bestExplored = currentExplored;
                    bestSize = currentSize;

                    algLabel5.Content = "Algorithm Name: " + bestRun;
                    algLabel6.Content = "Run Time: " + bestTime.ToString("0.00");
                    algLabel7.Content = "Nodes Explored: " + bestExplored.ToString();
                    algLabel8.Content = "Path Size: " + bestSize;
                }
                if (currentTime <=bestTime)
                {
                    bestRun = currentRun;
                    bestTime = currentTime;
                    bestExplored = currentExplored;
                    bestSize = currentSize;

                    algLabel5.Content = "Algorithm Name: " + bestRun;
                    algLabel6.Content = "Run Time: " + bestTime.ToString("0.00");
                    algLabel7.Content = "Nodes Explored: " + bestExplored.ToString();
                    algLabel8.Content = "Path Size: " + bestSize;
                }
            }

            if (currentRun == "A*")
            {
                if (aStarTime != 9999)
                {
                    aStarRuns++;
                    aStarTime = aStarTime + currentTime;
                    aStarAverage = aStarTime / aStarRuns;
                }
                else
                {
                    aStarRuns = 1;
                    aStarTime = currentTime;
                    aStarAverage = aStarTime / aStarRuns;
                }
                aStarSize = currentSize;
                aStarExplored = currentExplored;
                if (algSelected == 0)
                {
                    Ststs.Text = "Mean Run Time: " + aStarAverage.ToString("0.00") + " (" + aStarRuns + " Runs)" + System.Environment.NewLine + "Nodes Explored: " + aStarExplored + System.Environment.NewLine + "path Size: " + aStarSize;
                }
            }
            else if (currentRun == "Dijkstra's")
            {
                if (dijkstraTime != 9999)
                {
                    dijkstraRuns++;
                    dijkstraTime = dijkstraTime + currentTime;
                    dijkstraAverage = dijkstraTime / dijkstraRuns;
                }
                else
                {
                    dijkstraRuns = 1;
                    dijkstraTime = currentTime;
                    dijkstraAverage = dijkstraTime / dijkstraRuns;
                }
                dijkstraSize = currentSize;
                dijkstraExplored = currentExplored;
                if (algSelected == 1)
                {
                    Ststs.Text = "Mean Run Time: " + dijkstraAverage.ToString("0.00") + " (" + dijkstraRuns + " Runs)" + System.Environment.NewLine + "Nodes Explored: " + dijkstraExplored + System.Environment.NewLine + "path Size: " + dijkstraSize;
                }
            }
            else if (currentRun == "Breadth First")
            {
                if (breadthFirstTime != 9999)
                {
                    breadthFirstRuns++;
                    breadthFirstTime = breadthFirstTime + currentTime;
                    breadthFirstAverage = breadthFirstTime / breadthFirstRuns;
                }
                else
                {
                    breadthFirstRuns = 1;
                    breadthFirstTime = currentTime;
                    breadthFirstAverage = breadthFirstTime / breadthFirstRuns;
                }
                breadthFirstSize = currentSize;
                breadthFirstExplored = currentExplored;
                if (algSelected == 2)
                {
                    Ststs.Text = "Mean Run Time: " + breadthFirstAverage.ToString("0.00") + " (" + breadthFirstRuns + " Runs)" + System.Environment.NewLine + "Nodes Explored: " + breadthFirstExplored + System.Environment.NewLine + "path Size: " + breadthFirstSize;
                }
            }
            else
            {
                if (greedyBestFirstTime != 9999)
                {
                    greedyBestFirstRuns++;
                    greedyBestFirstTime = greedyBestFirstTime + currentTime;
                    greedyBestFirstAverage = greedyBestFirstTime / greedyBestFirstRuns;
                }
                else
                {
                    greedyBestFirstRuns = 1;
                    greedyBestFirstTime = currentTime;
                    greedyBestFirstAverage = greedyBestFirstTime / greedyBestFirstRuns;
                }
                greedyBestFirstSize = currentSize;
                greedyBestFirstExplored = currentExplored;
                if (algSelected == 3)
                {
                    Ststs.Text = "Mean Run Time: " + greedyBestFirstAverage.ToString("0.00") + " (" + greedyBestFirstRuns + " Runs)" + System.Environment.NewLine + "Nodes Explored: " + greedyBestFirstExplored + System.Environment.NewLine + "path Size: " + greedyBestFirstSize;
                }
            }
        }

        //Show current algorithm being run statistics
        public void showStatsOnRun(Stopwatch time, int nodesExplored)
        {
            algLabel.Content = "Algorithm Name: " + currentRun;
            algLabel2.Content = "Run Time: " + time.Elapsed.TotalSeconds.ToString("0.00");
            currentSize = 0;
            algLabel3.Content = "Nodes Explored: " + nodesExplored;
            currentExplored = nodesExplored;
            algLabel4.Content = "Path Size: ";
        }

        //Update path size box as it is being constructed
        public void UpdatePathSize(int pathSize)
        {
            algLabel4.Content = "Path Size: " + pathSize;
        }

        //Reset all labels
        public void resetLabels()
        {
            algLabel.Content = "Algorithm Name: ";
            algLabel2.Content = "Run Time: ";
            algLabel3.Content = "Nodes Explored: ";
            algLabel4.Content = "Path Size: ";
            algLabel5.Content = "Algorithm Name: ";
            algLabel6.Content = "Run Time: ";
            algLabel7.Content = "Nodes Explored: ";
            algLabel8.Content = "Path Size: ";

            currentRun = "";
            currentTime = 0;
            currentSize = 0;
            bestRun = "";
            bestTime = 9999;
            bestSize = 9999;

            aStarTime = 9999;
            aStarAverage = 9999;
            aStarExplored = 9999;
            aStarSize = 9999;
            aStarRuns = 9999;
            dijkstraTime = 9999;
            dijkstraAverage = 9999;
            dijkstraExplored = 9999;
            dijkstraSize = 9999;
            dijkstraRuns = 9999;
            breadthFirstTime = 9999;
            breadthFirstAverage = 9999;
            breadthFirstExplored = 9999;
            breadthFirstSize = 9999;
            breadthFirstRuns = 9999;
            greedyBestFirstTime = 9999;
            greedyBestFirstAverage = 9999;
            greedyBestFirstExplored = 9999;
            greedyBestFirstSize = 9999;
            greedyBestFirstRuns = 9999;

            Ststs.Text = "This algorithm hasn't been run on this problem yet, please run the algorithm to view its stats";
        }

        //Stop algorithm running
        public void ctsStop()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }
        }

        //Bulk run all algorithms
        private void BulkRun(object sender, RoutedEventArgs e)
        {
            ComboBoxItem myItem = (ComboBoxItem)comboboxRun.SelectedItem;
            int myOption = comboboxRun.Items.IndexOf(myItem);

            int runAmount = 1;
            switch (myOption)
            {
                case 0:
                    runAmount = 1;
                    runBulkAmount(runAmount);
                    return;
                case 1:
                    runAmount = 5;
                    runBulkAmount(runAmount);
                    return;
                case 2:
                    runAmount = 10;
                    runBulkAmount(runAmount);
                    return;
                case 3:
                    runAmount = 20;
                    runBulkAmount(runAmount);
                    return;
            }
        }

        private async void runBulkAmount(int runAmount)
        {
            for (int i = 0; i < runAmount; i++)
            {
                //Setup grid for running
                setupForRun();

                //Reset parents
                foreach (Node a in cm.NodesList)
                {
                    a.Parent = null!;
                }

                //Create cancellation token
                cts = new CancellationTokenSource();

                //Run speed for algorithms
                var runSpeed = 10;

                currentRun = "A*";
                AStarRunner runAStar = new AStarRunner(cm);
                await runAStar.algRun(first!, last!, cts.Token, runSpeed);
                await pathMade();

                //Setup grid for running
                setupForRun();

                //Create cancellation token
                cts = new CancellationTokenSource();

                currentRun = "Dijkstra's";
                DijkstraRunner runDijkstra = new DijkstraRunner(cm);
                await runDijkstra.algRun(first!, last!, cts.Token, runSpeed);
                await pathMade();

                //Setup grid for running
                setupForRun();

                //Create cancellation token
                cts = new CancellationTokenSource();

                currentRun = "Breadth First";
                BreadthFirstRunner runBreadthFirst = new BreadthFirstRunner(cm);
                await runBreadthFirst.algRun(first!, last!, cts.Token, runSpeed);
                await pathMade();

                //Setup grid for running
                setupForRun();

                //Create cancellation token
                cts = new CancellationTokenSource();

                currentRun = "Greedy Best First";
                BestFirstRunner runBestFirst = new BestFirstRunner(cm);
                await runBestFirst.algRun(first!, last!, cts.Token, runSpeed);
                await pathMade();
            }
        }

        //Stall until path is built, when path is built show it for a few seconds
        private async Task pathMade()
        {
            while(currentSize == 0)
            {
                await Task.Delay(10);
            }
            await Task.Delay(1000);
            return;
        }

        //Reset labels on diagonal checked
        private void Diagonal_Checked(object sender, RoutedEventArgs e)
        {
            ctsStop();
            resetSearchVisualisation();
            resetLabels();
        }

        //Save layout
        private async void Save(object sender, RoutedEventArgs e)
        {
            //Save all tiles in a new file
            using StreamWriter file = new($"GridSize{size:D2}LayoutNo{noFiles:D2}.txt");
            noFiles++;
            foreach (StackPanel stp in panelList)
            {
                if (stp.Background == Brushes.MintCream)
                {
                    await file.WriteLineAsync("MintCream");
                }
                if (stp.Background == Brushes.Black)
                {
                    await file.WriteLineAsync("Black");
                }
                if (stp.Background == Brushes.Green)
                {
                    await file.WriteLineAsync("Green");
                }
                if (stp.Background == Brushes.Red)
                {
                    await file.WriteLineAsync("Red");
                }
                if (stp.Background == Brushes.LightGreen)
                {
                    await file.WriteLineAsync("MintCream");
                }
                if (stp.Background == Brushes.GreenYellow)
                {
                    await file.WriteLineAsync("MintCream");
                }
            }

            //Clear layoutbox
            LayoutBox.Items.Clear();
            noFiles = 0;

            //Get all relevant files
            var folder = @"C:\[0] HonsProj\[0] VS Proj\HonoursProjectAlgorithmComparer\HonoursProjectAlgorithmComparer\bin\Debug\net6.0-windows";
            var txtFiles = Directory.GetFiles(folder, $"GridSize{size:D2}LayoutNo*.txt").ToList();

            //Add each file to combo box
            foreach (string tfile in txtFiles)
            {
                string[] words = tfile.Split(@"\");
                var myFile = words[words.Length - 1];
                LayoutBox.Items.Add(myFile);
                noFiles++;
            }

            //Start on item 0
            LayoutBox.SelectedIndex = 0;
        }

        //Load file
        private void Load(object sender, RoutedEventArgs e)
        {
            //reset labels
            resetLabels();

            //Get file name
            int myOption = LayoutBox.Items.IndexOf(LayoutBox.SelectedItem);

            //Only continue if there is a file
            if (LayoutBox.SelectedItem != null)
            {
                //Open selected file
                StreamReader sr = new StreamReader($"GridSize{size:D2}LayoutNo{myOption:D2}.txt");

                //Read saved data
                var line = sr.ReadLine();
                var counter = 0;
                if (line!.Equals("MintCream"))
                {
                    panelList[counter].Background = Brushes.MintCream;
                }
                if (line!.Equals("Black"))
                {
                    panelList[counter].Background = Brushes.Black;
                }
                if (line!.Equals("Green"))
                {
                    panelList[counter].Background = Brushes.Green;
                }
                if (line!.Equals("Red"))
                {
                    panelList[counter].Background = Brushes.Red;
                }
                counter++;

                //Continue reading
                while (line != null)
                {
                    //Read the next line
                    line = sr.ReadLine();

                    //Change grid
                    if (line != null)
                    {
                        if (line.Equals("MintCream"))
                        {
                            panelList[counter].Background = Brushes.MintCream;
                        }
                        if (line.Equals("Black"))
                        {
                            panelList[counter].Background = Brushes.Black;
                        }
                        if (line.Equals("Green"))
                        {
                            panelList[counter].Background = Brushes.Green;
                        }
                        if (line.Equals("Red"))
                        {
                            panelList[counter].Background = Brushes.Red;
                        }
                        counter++;
                    }
                }
                //close file
                sr.Close();

                //Setup grid for running
                setupForRun();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShowBtn_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem myItem = (ComboBoxItem)ComboBoxDetailed.SelectedItem;
            int myOption = ComboBoxDetailed.Items.IndexOf(myItem);
            switch (myOption)
            {
                case 0:
                    algSelected = myOption;
                    if (aStarTime != 9999)
                    {
                        statsNameLabel.Content = "A* Algorithm";
                        Ststs.Text = "Mean Run Time: " + aStarAverage.ToString("0.00") + " (" + aStarRuns + " Runs)" + System.Environment.NewLine + "Nodes Explored: " + aStarExplored + System.Environment.NewLine + "path Size: " + aStarSize;
                    }
                    else
                    {
                        statsNameLabel.Content = "A* Algorithm";
                        Ststs.Text = "This algorithm hasn't been run on this problem yet, please run the algorithm to view its stats";
                    }
                    break;
                case 1:
                    algSelected = myOption;
                    if (dijkstraTime != 9999)
                    {
                        statsNameLabel.Content = "Dijkstra Algorithm";
                        Ststs.Text = "Mean Run Time: " + dijkstraAverage.ToString("0.00") + " (" + dijkstraRuns + " Runs)" + System.Environment.NewLine + "Nodes Explored: " + dijkstraExplored + System.Environment.NewLine + "path Size: " + dijkstraSize;
                    }
                    else
                    {
                        statsNameLabel.Content = "Dijkstra Algorithm";
                        Ststs.Text = "This algorithm hasn't been run on this problem yet, please run the algorithm to view its stats";
                    }
                    break;
                case 2:
                    algSelected = myOption;
                    if (breadthFirstTime != 9999)
                    {
                        statsNameLabel.Content = "Breadth First Algorithm";
                        Ststs.Text = "Mean Run Time: " + breadthFirstAverage.ToString("0.00") + " (" + breadthFirstRuns + " Runs)" + System.Environment.NewLine + "Nodes Explored: " + breadthFirstExplored + System.Environment.NewLine + "path Size: " + breadthFirstSize;
                    }
                    else
                    {
                        statsNameLabel.Content = "Breadth First Algorithm";
                        Ststs.Text = "This algorithm hasn't been run on this problem yet, please run the algorithm to view its stats";
                    }
                    break;
                case 3:
                    algSelected = myOption;
                    if (greedyBestFirstTime != 9999)
                    {
                        statsNameLabel.Content = "Greedy Best First Algorithm";
                        Ststs.Text = "Mean Run Time: " + greedyBestFirstAverage.ToString("0.00") + " (" + greedyBestFirstRuns + " Runs)" + System.Environment.NewLine + "Nodes Explored: " + greedyBestFirstExplored + System.Environment.NewLine + "path Size: " + greedyBestFirstSize;
                    }
                    else
                    {
                        statsNameLabel.Content = "Greedy Best First Algorithm";
                        Ststs.Text = "This algorithm hasn't been run on this problem yet, please run the algorithm to view its stats";
                    }
                    break;
            }
        }
    }
}
