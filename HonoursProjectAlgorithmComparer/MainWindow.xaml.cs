using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

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
        private Node? first;
        private Node? last;
        private ConnectionManager cm;
        private int size = 0;
        private string mode = "null";
        private StackPanel? laststp;
        private Brush lastStpColourStart = Brushes.MintCream;
        private Brush lastStpColourEnd = Brushes.MintCream;

        private string currentRun = "N/A";
        private double currentTime = 0;
        private int currentSize = 0;

        private string lastRun = "N/A";
        private double lastTime = 0;
        private int lastSize = 0;

        private string bestRun = "N/A";
        private double bestTime = 9999;
        private int bestSize = 9999;

        private CancellationTokenSource? cts;


        //Main Window Initialise
        public MainWindow()
        {
            InitializeComponent();

            first = null;
            last = null;
            mode = "null";

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

            cm = new ConnectionManager(size, this);

            first = cm.NodesList[(size * (size / 2)) + 1];
            last = cm.NodesList[(size * (size / 2)) + size - 2];
        }

        //Start Button clicked
        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            ctsStop();

            first = null;
            last = null;
            mode = "null";

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

            cm = new ConnectionManager(size, this);

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

            //Create start and end points
            panelList[(size * (size / 2)) + 1].Background = Brushes.Green;
            panelList[(size * (size / 2)) + size - 2].Background = Brushes.Red;
        }

        //Update colour on grid
        public void updatecol(int ID, Brush color)
        {
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
                ctsStop();

                //If an algorithm was run remove visualised search
                if (mode.Equals("running"))
                {
                    resetSearchVisualisation();
                    switchMode(stp);

                    //reset labels
                    resetLabels();
                }

                //If its not the same panel, change colour
                if (stp != laststp)
                {
                    UpdatePanelBasedOnMode(stp);
                }

                //Update last visited stackpanel
                laststp = stp;
            }
            else
            {
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
            ctsStop();

            if(cm == null)
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

            ComboBoxItem myItem = (ComboBoxItem)comboBox3.SelectedItem;
            int myOption = comboBox3.Items.IndexOf(myItem);

            cm.ConstructNetwork(diagonalAllowed, myOption);

            //Keep list of walls
            List<StackPanel> walls = new();

            //Fully set up grid for algorithm
            var iterator = 0;
            foreach (StackPanel stp in panelList)
            {
                if(stp.Background == Brushes.Black)
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
            var runSpeed = 10;

            //Run correct mode
            if (myOption2 == 0)
            {
                AStarRunner runAStar = new AStarRunner(cm);
                runAStar.algRun(first, last, cts.Token, runSpeed);
                lastRun = currentRun;
                currentRun = "A*";
            }
            if (myOption2 == 1)
            {
                BreadthFirstRunner runBreadthFirst = new BreadthFirstRunner(cm);
                runBreadthFirst.algRun(first, last, cts.Token, runSpeed);
                lastRun = currentRun;
                currentRun = "Breadth First";
            }
            if (myOption2 == 2)
            {
                cts = new CancellationTokenSource();
                DijkstraRunner runDijkstra = new DijkstraRunner(cm);
                runDijkstra.algRun(first, last, cts.Token, runSpeed);
                lastRun = currentRun;
                currentRun = "Dijkstra's";
            }
            if (myOption2 == 3)
            {
                BestFirstRunner runBestFirst = new BestFirstRunner(cm);
                runBestFirst.algRun(first, last, cts.Token, runSpeed);
                lastRun = currentRun;
                currentRun = "Greedy Best First";
            }
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

        public void showResults(double time, int pathSize)
        {    
            algLabel4.Content = "Last Algorithm Run: " + lastRun;
            lastTime = currentTime;
            algLabel5.Content = "Last Time Taken: " + lastTime.ToString("0.00");
            lastSize = currentSize;
            algLabel6.Content = "Last Path Size: " + lastSize;

            algLabel.Content = "Current Algorithm Run: " + currentRun;
            currentTime = time;
            algLabel2.Content = "Current Time Taken: " + time.ToString("0.00");
            currentSize = pathSize;
            algLabel3.Content = "Current Path Size Found: " + pathSize.ToString();

            if (currentSize <= bestSize)
            {
                if(currentSize < bestSize)
                {
                    bestRun = currentRun;
                    bestTime = currentTime;
                    bestSize = currentSize;

                    algLabel7.Content = "Best Algorithm Run: " + bestRun;
                    algLabel8.Content = "Best Time Taken: " + bestTime.ToString("0.00");
                    algLabel9.Content = "Best Path Size: " + bestSize;
                }
                if (currentTime <=bestTime)
                {
                    bestRun = currentRun;
                    bestTime = currentTime;
                    bestSize = currentSize;

                    algLabel7.Content = "Best Algorithm Run: " + bestRun;
                    algLabel8.Content = "Best Time Taken: " + bestTime.ToString("0.00");
                    algLabel9.Content = "Best Path Size: " + bestSize;
                }
            }
        }

        public void resetLabels()
        {
            algLabel.Content = "Current Algorithm Run: N/A";
            algLabel2.Content = "Current Time Taken: N/A";
            algLabel3.Content = "Current Path Size: N/A";
            algLabel4.Content = "Last Algorithm Run: N/A";
            algLabel5.Content = "Last Time Taken: N/A";
            algLabel6.Content = "Last Path Size: N/A";
            algLabel7.Content = "Best Algorithm Run: N/A";
            algLabel8.Content = "Best Time Taken: N/A";
            algLabel9.Content = "Best Path Size: N/A";

            currentRun = "N/A";
            currentTime = 0;
            currentSize = 0;
            lastRun = "N/A";
            lastTime = 0;
            lastSize = 0;
            bestRun = "N/A";
            bestTime = 9999;
            bestSize = 9999;
        }

        public void ctsStop()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }
        }
    }
}
