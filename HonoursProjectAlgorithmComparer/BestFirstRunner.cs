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
    class BestFirstRunner
    {
        //Table handler
        TableHandler th;

        //Main Window
        MainWindow wnd = (MainWindow)Application.Current.MainWindow;

        //Create BF class
        public BestFirstRunner(TableHandler tableHandler)
        {
            //Get the file handler from parameters
            this.th = tableHandler;
        }

        //Run BF search
        public async void algRun(Node firstNode, Node lastNode)
        {
            //Start timer and disable buttons
            var watch = System.Diagnostics.Stopwatch.StartNew();
            wnd.disableButtons();

            firstNode.Explored = true;
            firstNode.distance = firstNode.FindDistance(lastNode.CoordinateX, lastNode.CoordinateY);

            List<Node> nodeQueue = new List<Node>();

            nodeQueue.Add(firstNode);

            //While nodes in queue
            while (nodeQueue.Count() != 0)
            {
                //Stall to show visualisation
                await Task.Delay(10);

                wnd.updatecol(firstNode.NodeID, Brushes.Green);
                wnd.updatecol(lastNode.NodeID, Brushes.Red);

                nodeQueue = nodeQueue.OrderBy(o => o.distance).ToList();
                Node NodeChecking = nodeQueue[0];
                nodeQueue.RemoveAt(0);

                wnd.updatecol(NodeChecking.NodeID, Brushes.LightGreen);

                if (NodeChecking == firstNode && NodeChecking.ConnectedNodes.Count() == 0)
                {
                    return;
                }

                foreach (Node connectedNode in NodeChecking.ConnectedNodes)
                {
                    connectedNode.distance = connectedNode.FindDistance(lastNode.CoordinateX, lastNode.CoordinateY);
                    if (connectedNode.Explored != true)
                    {
                        if (connectedNode == lastNode)
                        {
                            connectedNode.Parent = NodeChecking;
                            //Stop timer and display path
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
                            return;
                        }
                        else
                        {
                            connectedNode.Explored = true;
                            nodeQueue.Add(connectedNode);
                            connectedNode.Parent = NodeChecking;
                            wnd.updatecol(connectedNode.NodeID, Brushes.GreenYellow);
                        }
                    }
                }
            }
            wnd.enableButtons();
            MessageBox.Show("No possible path");
        }
    }
}
