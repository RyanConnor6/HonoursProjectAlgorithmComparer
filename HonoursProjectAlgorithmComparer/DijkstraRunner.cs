using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

// RYAN CONNOR - 40437041
// HONOURS PROJECT
// SHORTEST PATH ALGORITHM COMPARER
// IMPLEMENTATION STARTED 21/11/22
// LAST UPDATE 30/11/22

namespace HonoursProjectAlgorithmComparer
{
    //Class to run Dijkstra algorithm
    class DijkstraRunner
    {
        //Table handler
        TableHandler th;

        //Main Window
        MainWindow wnd = (MainWindow)Application.Current.MainWindow;

        //Function to allow visualisation drawing without freezing
        public static void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                                  new Action(delegate { }));
        }

        //Called upon creation, runs the dijkstra algorithm
        public DijkstraRunner(Node firstNode, Node lastNode, TableHandler tableHandler)
        {
            //Get the file handler from parameters
            this.th = tableHandler;

            //Create list of all nodes
            List<Node> nodeQueue = new List<Node>();

            //Add each node to the queue
            foreach (Node c in th.NodesList)
            {
                c.fScore = int.MaxValue;
                c.Parent = null;
                nodeQueue.Add(c);
            }

            //Initialise cost of first node as 0
            firstNode.fScore = 0;

            //While nodes in queue
            while (nodeQueue.Count() != 0)
            {
                wnd.updatecol(firstNode.NodeID, Brushes.Green);
                wnd.updatecol(lastNode.NodeID, Brushes.Red);

                int counter = 0;
                while (counter < 2)
                {
                    DoEvents();
                    Thread.Sleep(4);
                    ++counter;
                }

                //Get next node to check
                nodeQueue = nodeQueue.OrderBy(o => o.fScore).ToList();
                Node NodeChecking = nodeQueue[0];
                nodeQueue.RemoveAt(0);

                if (NodeChecking.Parent != null)
                {
                    wnd.updatecol(NodeChecking.NodeID, Brushes.LightGreen);
                }

                if (NodeChecking == firstNode && NodeChecking.ConnectedNodes.Count() == 0)
                {
                    return;
                }

                //If goal reached, display path and end
                if (NodeChecking == lastNode)
                {
                    if (lastNode.Parent != null)
                    {
                        th.RunDisplayFunctions(lastNode);
                    }
                    //MessageBox.Show("Found!");
                    return;
                }

                //Update best costs
                foreach (Node connectedNode in NodeChecking.ConnectedNodes)
                {
                    double alt = NodeChecking.fScore + NodeChecking.FindDistance(connectedNode.CoordinateX, connectedNode.CoordinateY);
                    if (alt < connectedNode.fScore)
                    {
                        connectedNode.fScore = alt;
                        connectedNode.Parent = NodeChecking;
                        wnd.updatecol(connectedNode.NodeID, Brushes.GreenYellow);
                    }
                }
            }
        }
    }
}
