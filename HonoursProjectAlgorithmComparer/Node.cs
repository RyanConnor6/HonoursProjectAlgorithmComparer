using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// RYAN CONNOR - 40437041
// HONOURS PROJECT
// SHORTEST PATH ALGORITHM COMPARER
// IMPLEMENTATION STARTED 21/11/22
// LAST UPDATE 30/11/22

namespace HonoursProjectAlgorithmComparer
{
    public class Node
    {
        //Private variables storing cave info
        private int coordinateX;
        private int coordinateY;
        private double GScore;
        private double FScore;
        private int nodeID;
        private Node parent;
        private List<Node> connectedNodes = new List<Node>();
        private bool explored = false;

        //Node constructor
        public Node(int ID, int x, int y)
        {
            coordinateX = x;
            coordinateY = y;
            nodeID = ID;
        }

        //Getters and setters
        public int CoordinateX
        {
            get { return coordinateX; }
            set { coordinateX = value; }
        }

        public int CoordinateY
        {
            get { return coordinateY; }
            set { coordinateY = value; }
        }

        public double gScore
        {
            get { return GScore; }
            set { GScore = value; }
        }

        public double fScore
        {
            get { return FScore; }
            set { FScore = value; }
        }

        public int NodeID
        {
            get { return nodeID; }
            set { nodeID = value; }
        }

        public Node Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public List<Node> ConnectedNodes
        {
            get { return connectedNodes; }
            set { connectedNodes = value; }
        }

        public bool Explored
        {
            get { return explored; }
            set { explored = value; }
        }

        //Euclidean Distance Calculator
        public double FindDistance(int targetX, int targetY)
        {
            double euclideanDistance = (Math.Sqrt((Math.Pow(CoordinateX - targetX, 2)) + (Math.Pow(CoordinateY - targetY, 2))));
            return euclideanDistance;
        }
    }
}
