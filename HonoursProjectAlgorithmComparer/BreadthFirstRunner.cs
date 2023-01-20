using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        ConnectionManager cm;

        //Explored nodes
        int exploredNodes = 0;

        //Main Window
        MainWindow wnd = (MainWindow)Application.Current.MainWindow;

        //Create BF class
        public BreadthFirstRunner(ConnectionManager ConnectionManager)
        {
            //Get the file handler from parameters
            this.cm = ConnectionManager;
        }

        //Run BF search
        public async Task algRun(Node firstNode, Node lastNode, CancellationToken token, int runSpeed)
        {
            //Start timer and disable buttons
            var watch = System.Diagnostics.Stopwatch.StartNew();

            //Create list of all nodes
            List<Node> nodeQueue = new List<Node>();

            nodeQueue.Add(firstNode);

            firstNode.Parent = null!;

            firstNode.Explored = true;

            //While nodes in queue
            while (nodeQueue.Count() != 0)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                //Stall to show visualisation
                await Task.Delay(runSpeed);
                wnd.showStatsOnRun(watch, exploredNodes);

                if (token.IsCancellationRequested)
                {
                    return;
                }

                Node NodeChecking = nodeQueue[0];
                nodeQueue.RemoveAt(0);
                exploredNodes++;

                wnd.updatecol(NodeChecking.NodeID, Brushes.LightGreen);

                wnd.updatecol(firstNode.NodeID, Brushes.Green);
                wnd.updatecol(lastNode.NodeID, Brushes.Red);

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

                    //Run all display functions in connection manager and end
                    if (lastNode.Parent != null)
                    {
                        cm.RunDisplayFunctions(lastNode, seconds3, token, runSpeed);
                    }
                    return;
                }

                //Update best
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
