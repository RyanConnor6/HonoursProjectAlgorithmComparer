using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
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
        List<StackPanel> panelList = new();
        Grid panelGrid = new Grid();
        Node first;
        Node last;
        TableHandler th;
        int size = 0;
        string mode = "null";
        StackPanel laststp;

        //Main Window Initialise
        public MainWindow()
        {
            InitializeComponent();
        }

        //Start Button clicked
        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            first = null;
            last = null;
            mode = "null";

            ComboBoxItem myItem = (ComboBoxItem)comboBox1.SelectedItem;
            string value = myItem.Content.ToString();
            string[] subs = value.Split('x');
            size = Int32.Parse(subs[0]);
            creategrid(size);

            th = new TableHandler(size);
        }

        //Create the grid
        public void creategrid(int size)
        {
            //Setup panel grid
            panelGrid.Children.Clear();
            panelGrid.RowDefinitions.Clear();
            panelGrid.ColumnDefinitions.Clear();
            this.canContainer.Children.Clear();
            panelList.Clear();
            panelGrid.Width = 750;
            panelGrid.Height = 750;
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
                //If an algorithm was run remove visualised search
                if (mode.Equals("running"))
                {
                    resetSearchVisualisation();
                    switchMode(stp);
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
                }
            }
        }

        //Update placement of element on grid
        private void PlacementControl2(object sender, RoutedEventArgs e)
        {
            //Adds elements when clicked
            StackPanel stp = (StackPanel)sender;

            //If an algorithm was run remove visualised search
            if (mode.Equals("running"))
            {
                resetSearchVisualisation();
            }

            //Update panel colour
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
                    updatecol(first.NodeID, Brushes.MintCream);
                }

                foreach (Node n in th.NodesList)
                {
                    if (n.CoordinateX == x && n.CoordinateY == y)
                    {
                        updatecol(n.NodeID, Brushes.Green);
                        first = n;
                    }
                }
            }
            //If end mode, move end
            else if (mode.Equals("placeend"))
            {
                int x = Int32.Parse(subs[1]);
                int y = Int32.Parse(subs[2]);
                if (last != null)
                {
                    updatecol(last.NodeID, Brushes.MintCream);
                }

                foreach (Node n in th.NodesList)
                {
                    if (n.CoordinateX == x && n.CoordinateY == y)
                    {
                        updatecol(n.NodeID, Brushes.Red);
                        last = n;
                    }
                }
            }
            //If wall mode, place wall
            else if (mode.Equals("placewall"))
            {
                int x = Int32.Parse(subs[1]);
                int y = Int32.Parse(subs[2]);

                foreach (Node n in th.NodesList)
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

                foreach (Node n in th.NodesList)
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
            //Set mode to run
            mode = "running";

            //Construct node network
            bool diagonalAllowed = true;
            if ((bool)Diagonal.IsChecked == false) 
            { 
                diagonalAllowed = false;
            }
            th.ConstructNetwork(diagonalAllowed);

            //Keep list of walls
            List<StackPanel> walls = new();

            //Fully set up grid for algorithm
            var iterator = 0;
            foreach (StackPanel stp in panelList)
            {
                if(stp.Background == Brushes.Black)
                {
                    foreach (Node n in th.NodesList[iterator].ConnectedNodes)
                    {
                        n.ConnectedNodes.Remove(th.NodesList[iterator]);
                    }
                    th.NodesList[iterator].ConnectedNodes.Clear();
                    walls.Add(stp);
                }
                if (stp.Background == Brushes.Green)
                {
                    string[] subs = stp.Name.Split('c');
                    int x = Int32.Parse(subs[1]);
                    int y = Int32.Parse(subs[2]);
                    foreach (Node n in th.NodesList)
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
                    foreach (Node n in th.NodesList)
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
            updatecol(first.NodeID, Brushes.Green);
            updatecol(last.NodeID, Brushes.Red);

            //Create a watch to track time
            var watch = System.Diagnostics.Stopwatch.StartNew();

            //Disable buttons when running
            startBtn.IsEnabled = false;
            runBtn.IsEnabled = false;

            //Get algorithm from combobox
            ComboBoxItem myItem = (ComboBoxItem)comboBox2.SelectedItem;
            string value = myItem.Content.ToString();
            char run = value[0];

            //Reset parents
            foreach (Node a in th.NodesList)
            {
                a.Parent = null;
            }

            //Run correct mode
            if (run == 'A')
            {
                AStarRunner runAStar = new AStarRunner(first, last, th);
            }
            if (run == 'B')
            {
                BreadthFirstRunner runBreadthFirst = new BreadthFirstRunner(first, last, th);
            }
            if (run == 'D')
            {
                DijkstraRunner runDijkstra = new DijkstraRunner(first, last, th);
            }

            //Reenable buttons
            startBtn.IsEnabled = true;
            runBtn.IsEnabled = true;

            //Get final run time
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            float seconds = elapsedMs / 1000;
            float seconds2 = elapsedMs % 1000;
            float seconds3 = seconds + seconds2 / 1000;

            //Redraw start and goal
            updatecol(first.NodeID, Brushes.Green);
            updatecol(last.NodeID, Brushes.Red);

            //Print run time
            MessageBox.Show("The algorithm has taken " + seconds3 + " seconds, path of size " + th.PathSize + " found.");
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
    }
}
