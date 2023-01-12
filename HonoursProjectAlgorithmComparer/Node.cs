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
        //Private variables storing node info
        private int coordinateX;
        private int coordinateY;
        private double GScore;
        private double FScore;
        private double Distance;
        private int nodeID;
        private Node? parent;
        private List<Node> connectedNodes = new List<Node>();
        private bool explored = false;
        
        private int distType;

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

        public double distance
        {
            get { return Distance; }
            set { Distance = value; }
        }

        public int NodeID
        {
            get { return nodeID; }
            set { nodeID = value; }
        }

        public Node Parent
        {
            get { return parent!; }
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

        public int DistanceType
        {
            get { return distType; }
            set { distType = value; }
        }

        public void DistType(int type)
        {
            distType = type;
        }

        //Euclidean Distance Calculator
        public double FindDistance(int targetX, int targetY)
        {  
            //Switch depending on distance type
            switch (distType)
            {
                //Euclidean
                case 0:
                    double euclideanDistance = (Math.Sqrt((Math.Pow(CoordinateX - targetX, 2)) + (Math.Pow(CoordinateY - targetY, 2))));
                    return euclideanDistance;
                //Manhattan
                case 1:
                    return Math.Abs(CoordinateX - targetX) + Math.Abs(coordinateY - targetY);
                //Chebyshev
                case 2:
                    var dx = Math.Abs(targetX - coordinateX);
                    var dy = Math.Abs(targetY - coordinateY);
                    return (dx + dy) - Math.Min(dx, dy);
                default:
                    double euclideanDistance2 = (Math.Sqrt((Math.Pow(CoordinateX - targetX, 2)) + (Math.Pow(CoordinateY - targetY, 2))));
                    return euclideanDistance2;
            }
        }
    }
}
