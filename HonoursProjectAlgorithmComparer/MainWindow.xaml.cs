using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
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
        //Main Window Initialise
        public MainWindow()
        {
            InitializeComponent();
        }

        //Variables for grid
        List<StackPanel> myList = new();
        Grid myGrid = new Grid();
        Node first;
        Node last;
        TableHandler th;
        int size = 0;

        string mode = "null";

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
            myGrid.Children.Clear();
            myGrid.RowDefinitions.Clear();
            myGrid.ColumnDefinitions.Clear();
            this.canContainer.Children.Clear();
            myList.Clear();

            myGrid.Width = 750;
            myGrid.Height = 750;
            myGrid.HorizontalAlignment = HorizontalAlignment.Center;
            myGrid.VerticalAlignment = VerticalAlignment.Center;
            myGrid.ShowGridLines = false;

            // Define the Columns
            for (int i = 0; i < size; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                myGrid.ColumnDefinitions.Add(cd);
                RowDefinition rd = new RowDefinition();
                myGrid.RowDefinitions.Add(rd);
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    StackPanel stp = new StackPanel();
                    stp.Background = Brushes.MintCream;
                    Grid.SetColumn(stp, j);
                    Grid.SetRow(stp, i);
                    myGrid.Children.Add(stp);

                    myList.Add(stp);
                }
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Button MyControl1 = new Button();
                    MyControl1.Name = "c" + (j+1) + "c" + (i+1);
                    //MyControl1.Content = MyControl1.Name;
                    MyControl1.Click += Button_Click;
                    MyControl1.FontSize = 10;
                    MyControl1.Background = Brushes.Transparent;
                    //RotateTransform rotateTransform = new RotateTransform(180);
                    //MyControl1.RenderTransform = rotateTransform;

                    Grid.SetColumn(MyControl1, j);
                    Grid.SetRow(MyControl1, i);
                    myGrid.Children.Add(MyControl1);
                }
            }

            this.canContainer.Children.Add(myGrid);
        }

        //Update colour on grid
        public void updatecol(int ID, Brush color)
        {
            myList[ID - 1].Background = color;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;

            string[] subs = b.Name.Split('c');

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
            else if(mode.Equals("placeend"))
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
            else if (mode.Equals("placewall"))
            {
                int x = Int32.Parse(subs[1]);
                int y = Int32.Parse(subs[2]);

                foreach (Node n in th.NodesList)
                {
                    if (n.CoordinateX == x && n.CoordinateY == y)
                    {
                        updatecol(n.NodeID, Brushes.Black);
                    }
                }
            }
            else if (mode.Equals("removewall"))
            {
                int x = Int32.Parse(subs[1]);
                int y = Int32.Parse(subs[2]);

                foreach (Node n in th.NodesList)
                {
                    if (n.CoordinateX == x && n.CoordinateY == y)
                    {
                        updatecol(n.NodeID, Brushes.MintCream);
                    }
                }
            }
        }

        private void runBtn_Click(object sender, RoutedEventArgs e)
        {
            mode = "running";

            List<StackPanel> walls = new();

            var iterator = 0;
            foreach (StackPanel stp in myList)
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
                iterator++;
            }

            foreach (StackPanel stp in myList)
            {
                stp.Background = Brushes.MintCream;
            }

            foreach (StackPanel stp in walls)
            {
                stp.Background = Brushes.Black;
            }

            updatecol(first.NodeID, Brushes.Green);
            updatecol(last.NodeID, Brushes.Red);

            var watch = System.Diagnostics.Stopwatch.StartNew();

            psBtn.IsEnabled = false;
            pgBtn.IsEnabled = false;
            pwBtn.IsEnabled = false;
            rwBtn.IsEnabled = false;
            startBtn.IsEnabled = false;
            runBtn.IsEnabled = false;

            DijkstraRunner runDijkstra = new DijkstraRunner(first, last, th);

            psBtn.IsEnabled = true;
            pgBtn.IsEnabled = true;
            pwBtn.IsEnabled = true;
            rwBtn.IsEnabled = true;
            startBtn.IsEnabled = true;
            runBtn.IsEnabled = true;

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            float seconds = elapsedMs / 1000;
            float seconds2 = elapsedMs % 1000;
            float seconds3 = seconds + seconds2 / 1000;

            updatecol(first.NodeID, Brushes.Green);
            updatecol(last.NodeID, Brushes.Red);

            MessageBox.Show("The algorithm has taken " + seconds3 + " seconds");
        }

        private void psBtn_Click(object sender, RoutedEventArgs e)
        {
            resetSearchVisualisation();
            mode = "placestart";
        }

        private void pgBtn_Click(object sender, RoutedEventArgs e)
        {
            resetSearchVisualisation();
            mode = "placeend";
        }

        private void pwBtn_Click(object sender, RoutedEventArgs e)
        {
            resetSearchVisualisation();
            mode = "placewall";
        }

        private void rwBtn_Click(object sender, RoutedEventArgs e)
        {
            resetSearchVisualisation();
            mode = "removewall";
        }

        private void resetSearchVisualisation()
        {
            if (mode.Equals("running"))
            {
                foreach (StackPanel stp in myList)
                {
                    if (stp.Background == Brushes.GreenYellow || stp.Background == Brushes.LightGreen || stp.Background == Brushes.Khaki)
                    {
                        stp.Background = Brushes.MintCream;
                    }
                }
            }
        }
    }
}
