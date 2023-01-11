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
    }
}
