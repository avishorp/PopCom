using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Management;
using System.Drawing;
using PopCom.Properties;
using Microsoft.Win32;

namespace PopCom
{
    class Popper : IDisposable
    {
        //Popper is a singleton
        private static readonly Popper popper = new Popper();

        //notify icon: prepare the icons we may use in the notification
        private NotifyIcon notify;
        private ContextMenu contextMenu = new ContextMenu();
        private MenuItem enablePlugInMenuItem;
        private MenuItem enablePlugOutMenuItem;

        // Registry keys
        private const string REGISTRY_KEY_SETTINGS = "SOFTWARE\\PopCom";
        private const string REGISTRY_VALUE_ENABLE_PLUGIN = "NotifPlugIn";
        private const string REGISTRY_VALUE_ENABLE_PLUGOUT = "NotifPlugOut";

        // Events enabled
        bool enablePlugIn = true;
        bool enablePlugOut = true;

        public static Popper GetInstance()
        {
            return popper;
        }

        private Popper() //singleton so private constructor!
        {
            // Create menu
            enablePlugInMenuItem = new MenuItem("Plug-in Notifications", togglePlugInNotif);
            enablePlugOutMenuItem = new MenuItem("Plug-out Notifications", togglePlugOutNotif);
            contextMenu.MenuItems.Add(enablePlugInMenuItem);
            contextMenu.MenuItems.Add(enablePlugOutMenuItem);
            enablePlugInMenuItem.Checked = true;
            //add a exit submenu item
            contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add(new MenuItem("Exit", new EventHandler(Menu_OnExit)));

            // notifyicon
            notify = new NotifyIcon();
            notify.Icon = PopCom.popcom;
            notify.Text = "PopCom Plug-in notifier";
            notify.ContextMenu = contextMenu;
            notify.Visible = true;

            enablePlugInNotif = true;
            enablePlugOutNotif = false;

            ReadSettings();

            // Create a WMI query for device insertion
            WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'");

            ManagementEventWatcher insertWatcher = new ManagementEventWatcher(insertQuery);
            insertWatcher.EventArrived += new EventArrivedEventHandler((object sender, EventArrivedEventArgs args) => DeviceEvent(sender, args, true));
            insertWatcher.Start();

            // Create a WMI query for device removal
            WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'");

            ManagementEventWatcher removeWatcher = new ManagementEventWatcher(removeQuery);
            removeWatcher.EventArrived += new EventArrivedEventHandler((object sender, EventArrivedEventArgs args) => DeviceEvent(sender, args, false));
            removeWatcher.Start();

        }

        private void ReadSettings()
        {
            enablePlugInNotif = ConvertToBoolSafe(ReadRegistryKeyCascading(REGISTRY_KEY_SETTINGS, REGISTRY_VALUE_ENABLE_PLUGIN, true));
            enablePlugOutNotif = ConvertToBoolSafe(ReadRegistryKeyCascading(REGISTRY_KEY_SETTINGS, REGISTRY_VALUE_ENABLE_PLUGOUT, false));
        }

        private void WriteSettings()
        {
            try
            {
                Registry.SetValue(Registry.CurrentUser.Name + "\\" + REGISTRY_KEY_SETTINGS, REGISTRY_VALUE_ENABLE_PLUGIN, enablePlugInNotif);
                Registry.SetValue(Registry.CurrentUser.Name + "\\" + REGISTRY_KEY_SETTINGS, REGISTRY_VALUE_ENABLE_PLUGOUT, enablePlugOutNotif);
            }
            catch(Exception e)
            {
                MessageBox.Show("Failed writing the settings to registry", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private object ReadRegistryKeyCascading(string key, string value, object defaultValue)
        {
            // First, try to read the key from the local (user) scope
            object v = Registry.GetValue(Registry.CurrentUser.Name + "\\"+ key, value, null);

            // If the value exists, return it
            if (v != null)
                return v;

            // Try from the global (machine) scope. If it does not
            // exist there either, return the default value
            return Registry.GetValue(Registry.LocalMachine.Name + "\\" + key, value, defaultValue);
        }

        private bool ConvertToBoolSafe(object o)
        {
            try
            {
                return Convert.ToBoolean(o);
            }
            catch(FormatException e)
            {
                return false;
            }
        }

        private void DeviceEvent(object sender, EventArrivedEventArgs e, bool insertion)
        {
            if ((insertion && enablePlugIn) || (!insertion && enablePlugOut))
            {
                ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];

                var guid = instance["ClassGuid"];
                if (guid != null && guid.ToString() == "{4d36e978-e325-11ce-bfc1-08002be10318}")
                {
                    notify.ShowBalloonTip(20000,
                        insertion ? "Port Plugged in" : "Port plugged out",
                        instance["Caption"].ToString(), ToolTipIcon.Info);
                }
            }
        }

        ~Popper()
        {
            Dispose();
        }

        public void Dispose()
        {
        }

        void Menu_OnExit(Object sender, EventArgs e)
        {
            //be sure to call Application.Exit
            Dispose();
            Application.Exit();
        }

        public bool enablePlugInNotif
        {
            get
            {
                return enablePlugIn;
            }
            set
            {
                enablePlugIn = value;
                enablePlugInMenuItem.Checked = value;
            }
        }

        public bool enablePlugOutNotif
        {
            get
            {
                return enablePlugOut;
            }
            set
            {
                enablePlugOut = value;
                enablePlugOutMenuItem.Checked = value;
            }
        }

        private void togglePlugInNotif(object sender, EventArgs args)
        {
            enablePlugInNotif = !enablePlugInNotif;
            WriteSettings();
        }

        private void togglePlugOutNotif(object sender, EventArgs args)
        {
            enablePlugOutNotif = !enablePlugOutNotif;
            WriteSettings();
        }

    }
}
