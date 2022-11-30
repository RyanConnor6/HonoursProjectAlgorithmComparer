using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
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
    class TableHandler
    {
        //Node Information
        private int nodeAmount;
        private int gridSize;
        private int[,] coordinatesArray;
        private int[,] connectionMatrix;
        private List<Node> nodesList = new List<Node>();
        MainWindow wnd = (MainWindow)Application.Current.MainWindow;

        //Constructor
        public TableHandler(int size)
        {
            //Get size and create node network
            gridSize = size;
            ConstructNetwork();
        }

        //Getters and setters for private values
        public int NodeAmount
        {
            get { return nodeAmount; }
            set { nodeAmount = value; }
        }

        public int[,] CoordinatesArray
        {
            get { return coordinatesArray; }
            set { coordinatesArray = value; }
        }

        public int[,] ConnectionMatrix
        {
            get { return connectionMatrix; }
            set { connectionMatrix = value; }
        }

        public List<Node> NodesList
        {
            get { return nodesList; }
            set { nodesList = value; }
        }

        //Create network
        void ConstructNetwork()
        {
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
                    if(column == 0)
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
                    counter++;
                }
            }
        }

        //Display final path by backtracking through parents
        public void RunDisplayFunctions(Node last)
        {
            //Get last node
            Node currentNode = last;

            String nodeat = "_" + currentNode.CoordinateX.ToString() + currentNode.CoordinateY.ToString();
            wnd.updatecol(currentNode.NodeID, Brushes.Khaki);
            currentNode = currentNode.Parent;

            do
            {
                nodeat = "_" + currentNode.CoordinateX.ToString() + currentNode.CoordinateY.ToString();
                wnd.updatecol(currentNode.NodeID, Brushes.Khaki);
                currentNode = currentNode.Parent;
            } while (currentNode.Parent != null);

            nodeat = "_" + currentNode.CoordinateX.ToString() + currentNode.CoordinateY.ToString();
            wnd.updatecol(currentNode.NodeID, Brushes.Khaki);
        }
    }
}
