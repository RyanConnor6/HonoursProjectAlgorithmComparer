using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HonoursTests
{
    public class GridTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void PanelCountTest()
        {
            MainWindow window = null!;

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

            int expected = 400;
            int actual = window.panelList.Count();

            Assert.That(actual, Is.EqualTo(expected).Within(0), "Incorrect amount of panels generated");
        }
    }
}
