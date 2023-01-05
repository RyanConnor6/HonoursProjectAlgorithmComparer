using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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

        //Class creation
        public DijkstraRunner(TableHandler tableHandler)
        {
            //Get the file handler from parameters
            this.th = tableHandler;
        }

        //Run Dijkstra
        public async void algRun(Node firstNode, Node lastNode, CancellationToken token, int runSpeed)
        {
            //Start timer and disable buttons
            var watch = System.Diagnostics.Stopwatch.StartNew();
            //wnd.disableButtons();

            //Create list of all nodes
            List<Node> nodeQueue = new List<Node>();

            //Add each node to the queue
            foreach (Node c in th.NodesList)
            {
                c.distance = int.MaxValue;
                c.Parent = null;
                nodeQueue.Add(c);
            }

            //Initialise cost of first node as 0
            firstNode.distance = 0;

            //While nodes in queue
            while (nodeQueue.Count() != 0)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                //Stall to show visualisation
                await Task.Delay(runSpeed);

                if (token.IsCancellationRequested)
                {
                    return;
                }

                //Make sure important nodes arent coloured over
                wnd.updatecol(firstNode.NodeID, Brushes.Green);
                wnd.updatecol(lastNode.NodeID, Brushes.Red);

                //Get next node to check
                nodeQueue = nodeQueue.OrderBy(o => o.distance).ToList();
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
                    //Stop timer and display path
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    float seconds = elapsedMs / 1000;
                    float seconds2 = elapsedMs % 1000;
                    float seconds3 = seconds + seconds2 / 1000;

                    if (lastNode.Parent != null)
                    {
                        th.RunDisplayFunctions(lastNode, seconds3, token, runSpeed);
                    }
                    return;
                }

                //Update best costs
                foreach (Node connectedNode in NodeChecking.ConnectedNodes)
                {
                    double alt = NodeChecking.distance + NodeChecking.FindDistance(connectedNode.CoordinateX, connectedNode.CoordinateY);
                    if (alt < connectedNode.distance)
                    {
                        connectedNode.distance = alt;
                        connectedNode.Parent = NodeChecking;
                        wnd.updatecol(connectedNode.NodeID, Brushes.GreenYellow);
                    }
                }
            }
            MessageBox.Show("No possible path");
        }
    }
}
