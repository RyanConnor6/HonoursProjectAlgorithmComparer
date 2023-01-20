using System;
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
    //Class to run a* algorithm
    class AStarRunner
    {
        //ConnectionManager
        ConnectionManager cm;

        //Explored nodes
        int exploredNodes = 0;

        //Main window
        MainWindow wnd = (MainWindow)Application.Current.MainWindow;

        //Create class
        public AStarRunner(ConnectionManager ConnectionManager)
        {
            //table Handler
            this.cm = ConnectionManager;
        }

        //Run A*
        public async void algRun(Node firstNode, Node lastNode, CancellationToken token, int runSpeed)
        {
            //Start watch and disable buttons
            var watch = System.Diagnostics.Stopwatch.StartNew();

            //Create open set and add first Node
            List<Node> openSet = new List<Node>();
            openSet.Add(firstNode);

            //Create closed set
            List<Node> closedSet = new List<Node>();

            //Determine fScore of first Node
            firstNode.fScore = firstNode.FindDistance(lastNode.CoordinateX, lastNode.CoordinateY);

            //While there are still values in the open set
            while (openSet.Count() != 0)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                //Stall task to show visualisation
                await Task.Delay(runSpeed);
                wnd.showStatsOnRun(watch, exploredNodes);

                if (token.IsCancellationRequested)
                {
                    return;
                }

                //Update colours
                foreach (Node c in openSet)
                {
                    wnd.updatecol(c.NodeID, Brushes.GreenYellow);
                }
                foreach (Node c in closedSet)
                {
                    wnd.updatecol(c.NodeID, Brushes.LightGreen);
                }
                wnd.updatecol(firstNode.NodeID, Brushes.Green);
                wnd.updatecol(lastNode.NodeID, Brushes.Red);


                //Get the next Node with the lowest fScore 
                Node NodeChecking = openSet[0];
                String Nodeat = "_" + NodeChecking.CoordinateX.ToString() + NodeChecking.CoordinateY.ToString();
                exploredNodes++;

                //If the Node is now the last Node
                if (NodeChecking == lastNode)
                {
                    //Stop timer and capture time
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    float seconds = elapsedMs / 1000;
                    float seconds2 = elapsedMs % 1000;
                    float seconds3 = seconds + seconds2 / 1000;

                    //Run all display functions in connection manager and end
                    cm.RunDisplayFunctions(lastNode, seconds3, token, runSpeed);
                    return;
                }

                //Remove currently checking Node from the open set
                openSet.RemoveAt(0);

                //Add Node to the closed set
                closedSet.Add(NodeChecking);

                //Order by lowest fScore
                closedSet = closedSet.OrderBy(o => o.fScore).ToList();
                openSet = openSet.OrderBy(o => o.fScore).ToList();

                //Get all the connections of the current Node
                List<Node> connections = NodeChecking.ConnectedNodes;

                //Run for each connectedNode that the current Node has
                foreach (Node connectedNode in connections)
                {
                    //If the openSet does not already contain the connected Node, then add it
                    if (openSet.Contains(connectedNode) == false)
                    {
                        //Make sure it's not already in closed set so cant be explored again
                        //This prevents unnecessary backtracking down the already explored route
                        if (closedSet.Contains(connectedNode) == false)
                        {
                            //get the gScore of the connected Node
                            //This is the current Nodes score plus the distance between the two Nodes
                            connectedNode.gScore = NodeChecking.gScore + NodeChecking.FindDistance(connectedNode.CoordinateX, connectedNode.CoordinateY);

                            //Make the parent the current Node and find the fScore of the Node
                            connectedNode.Parent = NodeChecking;
                            connectedNode.fScore = connectedNode.gScore + connectedNode.FindDistance(lastNode.CoordinateX, lastNode.CoordinateY);

                            //Add the connectedNode to the open list and re-order
                            openSet.Add(connectedNode);
                            closedSet = closedSet.OrderBy(o => o.fScore).ToList();
                            openSet = openSet.OrderBy(o => o.fScore).ToList();
                        }
                    }
                    //If the open set contains the Node already, or now has the Node
                    if (openSet.Contains(connectedNode) == true)
                    {
                        //Check if the new gScore is better than the connected Node's current gScore
                        double tentative_gScore = NodeChecking.gScore + NodeChecking.FindDistance(connectedNode.CoordinateX, connectedNode.CoordinateY);
                        if (tentative_gScore < connectedNode.gScore)
                        {
                            //The new best path for it is the lower gScore
                            //Change it's gScore and Parent accordingly
                            connectedNode.Parent = NodeChecking;
                            connectedNode.gScore = tentative_gScore;

                            //Update fScore based on new gScore value and re-order
                            connectedNode.fScore = connectedNode.gScore + connectedNode.FindDistance(lastNode.CoordinateX, lastNode.CoordinateY);
                            closedSet = closedSet.OrderBy(o => o.fScore).ToList();
                            openSet = openSet.OrderBy(o => o.fScore).ToList();
                        }
                    }
                }
            }
            //If there are no more Nodes in the open set and closed set does not contain the last Node
            if (closedSet.Contains(lastNode) == false)
            {
                return;
            }
        }
    }
}
