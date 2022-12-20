using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

// RYAN CONNOR - 40437041
// HONOURS PROJECT
// SHORTEST PATH ALGORITHM COMPARER
// IMPLEMENTATION STARTED 21/11/22
// LAST UPDATE 30/11/22

namespace HonoursProjectAlgorithmComparer
{
    //Class to run BF algorithm
    class BreadthFirstRunner
    {
        //Table handler
        TableHandler th;

        //Main Window
        MainWindow wnd = (MainWindow)Application.Current.MainWindow;

        //Called upon creation, runs the BF algorithm
        public BreadthFirstRunner(TableHandler tableHandler)
        {
            //Get the file handler from parameters
            this.th = tableHandler;
        }

        public async void algRun(Node firstNode, Node lastNode)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            wnd.disableButtons();

            //Create list of all nodes
            List<Node> nodeQueue = new List<Node>();

            nodeQueue.Add(firstNode);

            firstNode.Parent = null;

            firstNode.Explored = true;

            //While nodes in queue
            while (nodeQueue.Count() != 0)
            {
                await Task.Delay(10);

                wnd.updatecol(firstNode.NodeID, Brushes.Green);
                wnd.updatecol(lastNode.NodeID, Brushes.Red);

                Node NodeChecking = nodeQueue[0];
                nodeQueue.RemoveAt(0);

                wnd.updatecol(NodeChecking.NodeID, Brushes.LightGreen);

                if (NodeChecking == firstNode && NodeChecking.ConnectedNodes.Count() == 0)
                {
                    return;
                }

                //If goal reached, display path and end
                if (NodeChecking == lastNode)
                {

                    watch.Stop();
                    wnd.enableButtons();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    float seconds = elapsedMs / 1000;
                    float seconds2 = elapsedMs % 1000;
                    float seconds3 = seconds + seconds2 / 1000;

                    if (lastNode.Parent != null)
                    {
                        th.RunDisplayFunctions(lastNode, seconds3);
                    }
                    //MessageBox.Show("Found!");
                    return;
                }

                //Update best costs
                foreach (Node connectedNode in NodeChecking.ConnectedNodes)
                {
                    if (connectedNode.Explored == false)
                    {
                        connectedNode.Explored = true;
                        connectedNode.Parent = NodeChecking;
                        nodeQueue.Add(connectedNode);
                        wnd.updatecol(connectedNode.NodeID, Brushes.GreenYellow);
                    }
                }
            }
        }
    }
}
