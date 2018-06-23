using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace PopCom
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /**************************** Unhandeld exception catch ******************************************************
             * Create an handler that will be called by the framework when a unhandeld exception is detected
             *************************************************************************************************************/
            AppDomain myCurrentDomain = AppDomain.CurrentDomain;
            myCurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(myCurrentDomain_UnhandledException);

            /******************** NO Form ********************************************************
            /* Here you create the class that will serve as the main
             * instead of loading the form and then hiding it. 
             * The class should call Application.Exit() when exit time comes.
             * The form will only be created and kept in memory when its actually used.
             * Note that Application.Run does not start any form
             * Application.SetCompatibleTextRenderingDefault() should not be called when there is no form
             * Checker is a Singleton
             *************************************************************************************/
            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            Popper checker = Popper.GetInstance();
            Application.Run();
            checker = null; //must be done to stop referencing Checker so GC can collect it.
        }

        static void myCurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception unhandeld = (Exception)e.ExceptionObject;
                string error = "Exception:\r\n";
                error += "Message: " + unhandeld.Message + "\r\n";
                error += "Source:  " + unhandeld.Source + "\r\n";
                error += "StackTrace:  " + unhandeld.StackTrace + "\r\n";
                if (unhandeld.InnerException != null)
                {
                    error += "Innerexception Message: \r\n";
                    error += unhandeld.InnerException.Message + "\r\n";
                    error += "Source:  " + unhandeld.InnerException.Source + "\r\n";
                    error += "StackTrace:  " + unhandeld.InnerException.StackTrace + "\r\n";
                }

                EventLog.WriteEntry("Application", error, EventLogEntryType.Information);
            }
            catch (Exception more)
            {
                //even more misery
                MessageBox.Show("Additional problem: " + more.Message);
            }
            finally
            {
                MessageBox.Show(Application.ProductName + " has encountered an error, and will now close. Please check the Event Viewer for more details", 
                    "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
