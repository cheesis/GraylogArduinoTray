using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

/*
    Prerequisits
    ============
    arduino software because it contains the usb driver
    wix for the installer http://wixtoolset.org/
    json.net (run "Install-Package Newtonsoft.Json" from nu-get console)

    if you don't want to use arduino IDE get "Arduino IDE for Visual Studio" from https://visualstudiogallery.msdn.microsoft.com/069a905d-387d-4415-bc37-665a5ac9caba


    for the testing server
    =========================
    get node.js tools for visual studio https://www.visualstudio.com/en-us/features/node-js-vs.aspx
    add "Advanced REST client" app to chrome - otional but good to have to test the test server ...hahaha...get it?...whatever


    design choice
    ===============
    I chose to query the entire day every time, elastic search will only return 10 results but give a count of the total
    the reason is that I want to make sure nothing is missed, if going form the last query dimestamp timing issues could lead to missign events or double reporting them
    another upside of this is that I don't have to provide the time zone of my time stamps which makes the queries static, so they can go into settings


    TODO
    =====
    C#
    - create installer
        - copy usb driver...for that we have to find it first

*/

namespace GraylogArduinoTray
{
    public class GrayLogTray : Form
    {
        private ContextMenu trayMenu;
        private NotifyIcon trayIcon;
        private Arduino arduino;
        private ElasticSearch prodErrors;

        public GrayLogTray()
        {
            Logger.log("Starting up.");

            arduino = new Arduino(9600);  // hard coded baudrate, has to match what's set in the arduino hardware (see Arduino project)

            // Create a simple tray menu with only one item.
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Settings", OnSettings);
            trayMenu.MenuItems.Add("Exit", OnExit);

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "GraylogArduino";
            trayIcon.Icon = Properties.Resources.sadIcon;

            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;

            // initialize the queries that we are going to run against elastic search
            prodErrors = new ElasticSearch(Properties.Settings.Default.ProdErrorURI, Properties.Settings.Default.ProdErrorJSON);

            // set baseline for errors today
            prodErrors.postSearch();

            var timer = new System.Timers.Timer(Properties.Settings.Default.QueryInterval_ms);
            timer.Elapsed += DoOnTimer;
            timer.Enabled = true;
        }

        private void DoOnTimer(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();

            int prodErrrosBefore = prodErrors.numberOfHits;

            prodErrors.postSearch();

            if (prodErrors.numberOfHits > prodErrrosBefore)
            {
                // OMG new errors
                int errors = prodErrors.numberOfHits - prodErrrosBefore;
                bool success = arduino.sendError(errors);
                trayBalloon(prodErrors.numberOfHits - prodErrrosBefore);
            }
        }

        private void trayBalloon(int entries)
        {
            if (Properties.Settings.Default.ShowBalloon)
            {
                trayIcon.BalloonTipText = entries + " found.";
                trayIcon.BalloonTipClicked += new EventHandler(TrayIcon_Click);
                trayIcon.BalloonTipTitle = "Errors found";
                trayIcon.ShowBalloonTip(3);
            }
        }

        private void TrayIcon_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Settings.Default.OnTrayBalloonClickURI);
        }

        private void OnSettings(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }


        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }


        private void OnExit(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }
    }
}
