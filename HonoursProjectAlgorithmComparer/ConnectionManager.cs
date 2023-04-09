using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

// RYAN CONNOR - 40437041
// HONOURS PROJECT
// SHORTEST PATH ALGORITHM COMPARER
// IMPLEMENTATION STARTED 21/11/22
// LAST UPDATE 30/11/22

namespace HonoursProjectAlgorithmComparer
{
    //Class to construct node networks on grid
    //Class to print out final constructed path
    public class ConnectionManager
    {
        //Node Information
        private int nodeAmount;
        private int gridSize;
        private int pathSize;
        private List<Node> nodesList = new List<Node>();
        public MainWindow wnd;

        //Constructor
        public ConnectionManager(int size, MainWindow Window)
        {
            //Get size and create node network
            gridSize = size;
            wnd = Window;
            ConstructNetwork(true, 0);
        }

        //Getters and setters for private values
        public int NodeAmount
        {
            get { return nodeAmount; }
            set { nodeAmount = value; }
        }

        public int PathSize
        {
            get { return pathSize; }
            set { pathSize = value; }
        }

        public List<Node> NodesList
        {
            get { return nodesList; }
            set { nodesList = value; }
        }

        //Create network
        public void ConstructNetwork(bool diagonalAllowed, int distType)
        {
            //Reset connections
            for (int i = 0; i < NodesList.Count(); i++)
            {
                NodesList.Clear();
            }

            //Amount of nodes is gridsize times gridsize
            NodeAmount = gridSize * gridSize;

            //First node is 1
            int nodeID = 1;

            //Create a node for every square
            for (int i = 0; i < nodeAmount / gridSize; i++)
            {
                for (int j = 0; j < nodeAmount / gridSize; j++)
                {
                    int x = j+1;
                    int y = i+1;
                    Node node = new Node(nodeID, x, y);

                    node.DistType(distType);
                    
                    NodesList.Add(node);
                    nodeID++;
                }
            }

            //Add all connecting nodes to each neighbour list
            int counter = 0;
            for (int row = 0; row < gridSize; row++)
            {
                for (int column = 0; column < gridSize; column++)
                {
                    if (column == 0)
                    {
                        NodesList[counter].ConnectedNodes.Add(NodesList[counter + 1]);
                    }
                    else if (column == gridSize - 1)
                    {
                        NodesList[counter].ConnectedNodes.Add(NodesList[counter - 1]);
                    }
                    else
                    {
                        NodesList[counter].ConnectedNodes.Add(NodesList[counter + 1]);
                        NodesList[counter].ConnectedNodes.Add(NodesList[counter - 1]);
                    }
                    if (row == 0)
                    {
                        NodesList[counter].ConnectedNodes.Add(NodesList[counter + gridSize]);
                    }
                    else if (row == gridSize - 1)
                    {
                        NodesList[counter].ConnectedNodes.Add(NodesList[counter - gridSize]);
                    }
                    else
                    {
                        NodesList[counter].ConnectedNodes.Add(NodesList[counter + gridSize]);
                        NodesList[counter].ConnectedNodes.Add(NodesList[counter - gridSize]);
                    }

                    //Diagonal connections
                    if (diagonalAllowed == true)
                    {
                        if ((counter + gridSize) + 1 < NodesList.Count())
                        {
                            if (column != gridSize - 1)
                            {
                                NodesList[counter].ConnectedNodes.Add(NodesList[counter + gridSize + 1]);
                            }
                        }
                        if ((counter + gridSize) - 1 < NodesList.Count())
                        {
                            if (column != 0)
                            {
                                NodesList[counter].ConnectedNodes.Add(NodesList[counter + gridSize - 1]);
                            }
                        }
                        if ((counter - gridSize) - 1 >= 0)
                        {
                            if (column != 0)
                            {
                                NodesList[counter].ConnectedNodes.Add(NodesList[counter - gridSize - 1]);
                            }
                        }
                        if ((counter - gridSize) + 1 >= 0)
                        {
                            if (column != gridSize - 1)
                            {
                                NodesList[counter].ConnectedNodes.Add(NodesList[counter - gridSize + 1]);
                            }
                        }
                    }

                    counter++;
                }
            }
        }

        //Display final path by backtracking through parents
        public async void RunDisplayFunctions(Node last, float time, CancellationToken token, int runSpeed)
        {
            pathSize = 0;

            //Get last node
            Node currentNode = last;

            wnd.updatecol(currentNode.NodeID, Brushes.Red);
            currentNode = currentNode.Parent;

            //While there are still parented nodes, draw path
            if(currentNode.Parent != null)
            {
                do
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    //Delay to show path being created, update path label in main window
                    await Task.Delay(runSpeed);
                    wnd.UpdatePathSize(pathSize);

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    //Update path colour
                    pathSize++;
                    wnd.updatecol(currentNode.NodeID, Brushes.Khaki);
                    currentNode = currentNode.Parent;
                } while (currentNode.Parent != null);
            }

            wnd.updatecol(currentNode.NodeID, Brushes.Green);

            //Show results in mainwindow
            wnd.showResults(time, pathSize); 
        }
    }
}