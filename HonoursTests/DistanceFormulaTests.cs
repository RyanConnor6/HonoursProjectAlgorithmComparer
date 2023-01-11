using HonoursProjectAlgorithmComparer;

namespace HonoursTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
          
        }

        [Test]
        public void EuclideanDistanceTest()
        {
            Node node1 = new Node(1, 1, 1);
            node1.DistanceType = 0;
            Node node2 = new Node(2, 5, 5);

            double expected = 5.65685;

            double actual = node1.FindDistance(node2.CoordinateX, node2.CoordinateY);

            Assert.That(actual, Is.EqualTo(expected).Within(0.1), "Euclidean Distance Not Working Correctly");
        }

        [Test]
        public void EuclideanDistanceTest2()
        {
            Node node1 = new Node(1, 2, 7);
            node1.DistanceType = 0;
            Node node2 = new Node(2, 23, 2);

            double expected = 21.58703;

            double actual = node1.FindDistance(node2.CoordinateX, node2.CoordinateY);

            Assert.That(actual, Is.EqualTo(expected).Within(0.1), "Euclidean Distance Not Working Correctly");
        }

        [Test]
        public void ManhattanDistanceTest()
        {
            Node node1 = new Node(1, 1, 1);
            node1.DistanceType = 1;
            Node node2 = new Node(2, 5, 5);

            double expected = 8;

            double actual = node1.FindDistance(node2.CoordinateX, node2.CoordinateY);

            Assert.That(actual, Is.EqualTo(expected).Within(0.1), "Manhattan Distance Not Working Correctly");
        }

        [Test]
        public void ManhattanDistanceTest2()
        {
            Node node1 = new Node(1, 2, 7);
            node1.DistanceType = 1;
            Node node2 = new Node(2, 23, 2);

            double expected = 26;

            double actual = node1.FindDistance(node2.CoordinateX, node2.CoordinateY);

            Assert.That(actual, Is.EqualTo(expected).Within(0.1), "Manhattan Distance Not Working Correctly");
        }

        [Test]
        public void ChebyshevDistanceTest()
        {
            Node node1 = new Node(1, 1, 1);
            node1.DistanceType = 2;
            Node node2 = new Node(2, 5, 5);

            double expected = 4;

            double actual = node1.FindDistance(node2.CoordinateX, node2.CoordinateY);

            Assert.That(actual, Is.EqualTo(expected).Within(0.1), "Chebyshev Distance Not Working Correctly");
        }

        [Test]
        public void ChebyshevDistanceTest2()
        {
            Node node1 = new Node(1, 2, 7);
            node1.DistanceType = 2;
            Node node2 = new Node(2, 23, 2);

            double expected = 21;

            double actual = node1.FindDistance(node2.CoordinateX, node2.CoordinateY);

            Assert.That(actual, Is.EqualTo(expected).Within(0.1), "Chebyshev Distance Not Working Correctly");
        }
    }
}