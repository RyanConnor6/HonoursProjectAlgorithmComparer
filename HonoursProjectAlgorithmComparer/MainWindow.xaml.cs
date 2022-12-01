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
        List<Node> nodesList = new();

        //Start Button clicked
        private void startBtn_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            //int size = rnd.Next(5, 20);
            int size = 25;
            creategrid(size);

            TableHandler th = new TableHandler(size);

            int nodeAmount = th.NodeAmount;
            nodesList = th.NodesList;

            int rand = rnd.Next(0, nodeAmount);
            int rand2 = rnd.Next(0, nodeAmount);

            if (rand == rand2)
            {
                while (rand == rand2)
                {
                    rand2 = rnd.Next(0, nodeAmount);
                }
            }

            if (rand2 == rand + 1 || rand2 == rand - 1 || rand2 == rand + size || rand2 == rand - size)
            {
                while (rand2 == rand + 1 || rand2 == rand - 1 || rand2 == rand + size || rand2 == rand - size)
                {
                    rand2 = rnd.Next(0, nodeAmount);
                }
            }

            Node first = nodesList[rand];
            Node last = nodesList[rand2];

            //MessageBox.Show(rand.ToString() + " " + rand2.ToString());

            updatecol(first.NodeID, Brushes.Green);
            updatecol(last.NodeID, Brushes.Red);
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
            myGrid.ShowGridLines = true;

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
            this.canContainer.Children.Add(myGrid);
        }

        //Update colour on grid
        public void updatecol(int ID, Brush color)
        {
            myList[ID - 1].Background = color;
        }

        private void sizeBtn_Click(object sender, RoutedEventArgs e)
        {
            int x = Int32.Parse(coordBox1.Text);
            int y = Int32.Parse(coordBox2.Text);

            foreach(Node n in nodesList)
            {
                if (n.CoordinateX == x && n.CoordinateY == y)
                {
                    updatecol(n.NodeID, Brushes.Blue);
                    foreach (Node neighbour in n.ConnectedNodes)
                    {
                        updatecol(neighbour.NodeID, Brushes.LightBlue);
                    }
                }
            }
        }
    }
}
