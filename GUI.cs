/// <summary> 
/// Author:    Matthew Schnitter 
/// Partner:   None 
/// Date:      3/4/2022 
/// Course:    CS 3500, University of Utah, School of Computing 
/// Copyright: CS 3500 and Matthew Schnitter - This work may not be copied for use in Academic Coursework. 
/// 
/// I, Matthew Schnitter, certify that I wrote this code from scratch and did not copy it in part or whole from  
/// another source.  All references used in the completion of the assignment are cited in my README file. 
/// 
/// File Contents 
/// Code which creates the form. Code for creating a new form was provided from the ForStudents example.
/// </summary>

namespace GUI
{
    class Spreadsheet_Window : ApplicationContext
    {
        /// <summary>
        ///  Number of open forms
        /// </summary>
        private int formCount = 0;

        /// <summary>
        ///  Singleton ApplicationContext
        /// </summary>
        private static Spreadsheet_Window appContext;

        /// <summary>
        /// Returns the one DemoApplicationContext.
        /// </summary>
        public static Spreadsheet_Window getAppContext()
        {
            if (appContext == null)
            {
                appContext = new Spreadsheet_Window();
            }
            return appContext;
        }

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private Spreadsheet_Window()
        {
        }

        /// <summary>
        /// Build another GUI Window
        /// </summary>
        public void RunForm(Form form)
        {
            // One more form is running
            formCount++;

            // Assign an EVENT handler to take an action when the GUI is closed 
            form.FormClosed += (o, e) => { if (--formCount <= 0) ExitThread(); };

            // Run the form
            form.Show();
        }

    }

    /// <summary>
    /// Class to create the GUI
    /// </summary>
    internal static class GUI
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start an application context and run one form inside it
            Spreadsheet_Window appContext = Spreadsheet_Window.getAppContext();
            appContext.RunForm(new SpreadsheetGUI());
            Application.Run(appContext);
        }
    }
}