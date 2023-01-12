namespace HonoursTests
{
    public class NodeTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void NodeCountTest()
        {
            MainWindow window = null;

            // The dispatcher thread
            var t = new Thread(() =>
            {
                window = new MainWindow();

                // Initiates the dispatcher thread shutdown when the window closes
                window.Closed += (s, e) => window.Dispatcher.InvokeShutdown();

                window.Show();

                // Makes the thread support message pumping
                System.Windows.Threading.Dispatcher.Run();
            });

            // Configure the thread
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            ConnectionManager cm = new ConnectionManager(20, window);
            cm.ConstructNetwork(false, 'E');

            int expected = 400;
            int actual = cm.NodesList.Count();

            Assert.That(actual, Is.EqualTo(expected).Within(0), "Incorrect amount of nodes generated");
        }

        [Test]
        public void NodeNeighbourTestCornerNoDiagonal()
        {
            MainWindow window = null;

            // The dispatcher thread
            var t = new Thread(() =>
            {
                window = new MainWindow();

                // Initiates the dispatcher thread shutdown when the window closes
                window.Closed += (s, e) => window.Dispatcher.InvokeShutdown();

                window.Show();

                // Makes the thread support message pumping
                System.Windows.Threading.Dispatcher.Run();
            });

            // Configure the thread
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            ConnectionManager cm = new ConnectionManager(20, window);
            cm.ConstructNetwork(false, 'E');

            int expected = 2;
            int actual = cm.NodesList[0].ConnectedNodes.Count();

            Assert.That(actual, Is.EqualTo(expected).Within(0), "Incorrect amount of neighbouring nodes");
        }

        [Test]
        public void NodeNeighbourTestCornerDiagonal()
        {
            MainWindow window = null;

            // The dispatcher thread
            var t = new Thread(() =>
            {
                window = new MainWindow();

                // Initiates the dispatcher thread shutdown when the window closes
                window.Closed += (s, e) => window.Dispatcher.InvokeShutdown();

                window.Show();

                // Makes the thread support message pumping
                System.Windows.Threading.Dispatcher.Run();
            });

            // Configure the thread
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            ConnectionManager cm = new ConnectionManager(20, window);
            cm.ConstructNetwork(true, 'E');

            int expected = 3;
            int actual = cm.NodesList[0].ConnectedNodes.Count();

            Assert.That(actual, Is.EqualTo(expected).Within(0), "Incorrect amount of neighbouring nodes");
        }

        [Test]
        public void NodeNeighbourTestInGridNoDiagonal()
        {
            MainWindow window = null;

            // The dispatcher thread
            var t = new Thread(() =>
            {
                window = new MainWindow();

                // Initiates the dispatcher thread shutdown when the window closes
                window.Closed += (s, e) => window.Dispatcher.InvokeShutdown();

                window.Show();

                // Makes the thread support message pumping
                System.Windows.Threading.Dispatcher.Run();
            });

            // Configure the thread
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            ConnectionManager cm = new ConnectionManager(20, window);
            cm.ConstructNetwork(false, 'E');

            int expected = 4;
            int actual = cm.NodesList[25].ConnectedNodes.Count();

            Assert.That(actual, Is.EqualTo(expected).Within(0), "Incorrect amount of neighbouring nodes");
        }

        [Test]
        public void NodeNeighbourTestInGridDiagonal1()
        {
            MainWindow window = null;

            // The dispatcher thread
            var t = new Thread(() =>
            {
                window = new MainWindow();

                // Initiates the dispatcher thread shutdown when the window closes
                window.Closed += (s, e) => window.Dispatcher.InvokeShutdown();

                window.Show();

                // Makes the thread support message pumping
                System.Windows.Threading.Dispatcher.Run();
            });

            // Configure the thread
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            ConnectionManager cm = new ConnectionManager(20, window);
            cm.ConstructNetwork(true, 'E');

            int expected = 8;
            int actual = cm.NodesList[25].ConnectedNodes.Count();

            Assert.That(actual, Is.EqualTo(expected).Within(0), "Incorrect amount of neighbouring nodes");
        }
    }
}
