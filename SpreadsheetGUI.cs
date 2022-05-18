using SpreadsheetGrid_Core;
using SpreadsheetUtilities;
using SS;
using System;
using System.Windows.Forms;

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
/// Internal GUI code
/// </summary>

namespace GUI
{
    /// <summary>
    /// This class represents the internal code for the GUI
    /// </summary>
    public partial class SpreadsheetGUI : Form
    {
        private AbstractSpreadsheet spreadsheet;

        /// <summary>
        /// Constructor which creates the GUI form
        /// </summary>
        public SpreadsheetGUI()
        {
            this.gridWidget = new SpreadsheetGridWidget();
            InitializeComponent();
            spreadsheet = new Spreadsheet(s => true, s => s.ToUpper(), "six");
            gridWidget.SelectionChanged += GridWidget_SelectionChanged;
            gridWidget.SetSelection(0, 0, false);
            contentBox.Leave += ContentBox_Leave;
            nameBox.Text = "A1";
            this.FormClosing += SpreadsheetGUI_FormClosing;
        }

        /// <summary>
        /// This is the main event handler which modifies the spreadsheet.
        /// The GUI is designed so a user can select a cell and type in the contents via the 
        /// Contents text box. To insert the value into the spreadsheet, the user must click on any other cell
        /// or GUI element, since the content box is no longer the active element.
        /// </summary>
        /// <param name="sender"> event sender </param>
        /// <param name="e"> arguments </param>
        private void ContentBox_Leave(object? sender, EventArgs e)
        {
            int row, col;
            string contents = contentBox.Text;

            // Get the current selection
            gridWidget.GetSelection(out col, out row);

            // Use the helper methods to convert the col and row to a valid cell name
            string colName = FindColName(col);
            string rowName = (row + 1).ToString();
            string cellName = colName + rowName;
            nameBox.Text = cellName;

            // Set the contents of the cell in the spreadsheet to the text within content box
            List<string> cellsToRecalc = (List<string>)spreadsheet.SetContentsOfCell(cellName, contents);
            object value = spreadsheet.GetCellValue(cellName);
            string realValue;

            if (value is FormulaError) // Check to see what the value cell is, and format it accordingly
            {
                realValue = "Error";
            }
            else
            {
                realValue = value.ToString();
            }

            valueBox.Text = realValue;

            // Use a thread to recalculate dependent cells
            Thread thread = new Thread(() => ReCalculateCells(cellsToRecalc));
            thread.Start();
            while (thread.IsAlive) // Make the content box un-editable if it is still recalculating
            {
                contentBox.ReadOnly = true;
            }
            contentBox.ReadOnly = false; // Once finished allow the content box to be edited
            thread.Join();

            gridWidget.SetValue(col, row, realValue); // Set the cell to show its value in the grid
        }

        /// <summary>
        /// A helper method which recalculates cells if a dependency was changed. 
        /// This is done by a separate thread.
        /// </summary>
        /// <param name="cellsToRecalc"> cells to recalculate </param>
        private void ReCalculateCells(List<string> cellsToRecalc)
        {
            foreach (string cell in cellsToRecalc)
            {
                // Get the current cells value
                object cellValue = spreadsheet.GetCellValue(cell);
                string realCellValue;

                if (cellValue is FormulaError)
                {
                    realCellValue = "Error";
                }
                else
                {
                    realCellValue = cellValue.ToString();
                }

                // Change the grid for that cell to show its updated value
                string cellCol = cell.Substring(0, 1);
                int widgetCol = FindColValue(cellCol);
                int widgetRow = (Int32.Parse(cell.Substring(1, 1))) - 1;
                gridWidget.SetValue(widgetCol, widgetRow, realCellValue);
            }
        }

        /// <summary>
        /// Event handler for when a different cell is selected in the GridWidget
        /// </summary>
        /// <param name="sender"> grid widget </param>
        private void GridWidget_SelectionChanged(SpreadsheetGridWidget sender)
        {
            int row, col;
            string value;
            sender.GetSelection(out col, out row);
            sender.GetValue(col, row, out value);

            // Use the helper methods to convert the col and row to a valid cell name
            string colName = FindColName(col);
            string rowName = (row + 1).ToString();
            string cellName = colName + rowName;
            nameBox.Text = cellName;

            // Get cell contents to set content box to display the current contents
            object contents = spreadsheet.GetCellContents(cellName);
            string realContents;
            if (contents is Formula)
            {
                realContents = "=" + contents.ToString(); // Prepend a '=' if the contents is a Formula
            }
            else
            {
                realContents = contents.ToString();
            }
            contentBox.Text = realContents;


            valueBox.Text = value;

        }

        /// <summary>
        /// Event handler for a new form under the File menu.
        /// </summary>
        /// <param name="sender"> new menu </param>
        /// <param name="e"> args </param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Tell the application context to run the form on the same
            // thread as the other forms.
            Spreadsheet_Window.getAppContext().RunForm(new SpreadsheetGUI()); // Taken from example code
        }

        /// <summary>
        /// Event handler for closing a form under File menu
        /// </summary>
        /// <param name="sender"> close menu </param>
        /// <param name="e"> args </param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // If there are unsaved changes, display a warning before closing
            if (spreadsheet.Changed == true)
            {
                const string message = "There are unsaved changes in the spreadsheet. Are you sure you want to close?";
                const string caption = "Warning";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    return;
                }
                else
                {
                    Close();
                }
            }

            Close();
        }

        /// <summary>
        /// Event handler for closing the form with the X button
        /// </summary>
        /// <param name="sender"> X button </param>
        /// <param name="e"> args </param>
        private void SpreadsheetGUI_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // If there are unsaved changes, display a warning
            if (spreadsheet.Changed == true)
            {
                const string message = "There are unsaved changes in the spreadsheet. Are you sure you want to close?";
                const string caption = "Warning";
                var result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    e.Cancel = true; // Cancel the closing of the form if user selects No.
                }
            }
        }

        /// <summary>
        /// Event handler for saving a spreadsheet under File menu.
        /// </summary>
        /// <param name="sender"> save menu </param>
        /// <param name="e"> args </param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open a save file dialog to select where to save and name a file
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = ".sprd File|*.sprd|All Files|*.*"; // User can choose between all files or .sprd extension
            saveDialog.Title = "Save Spreadsheet";
            saveDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveDialog.FileName != "")
            {
                spreadsheet.Save(saveDialog.FileName); // Save the file
            }
        }

        /// <summary>
        /// Event handler for opening a saved spreadsheet from File menu.
        /// </summary>
        /// <param name="sender"> open menu </param>
        /// <param name="e"> args </param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;

            // Open a file dialog for the user
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // See references
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = ".sprd File|*.sprd|All Files|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    gridWidget.Clear(); // Clear current spreadsheet as it will be overwritten

                    // Create a new spreadsheet with the file
                    spreadsheet = new Spreadsheet(filePath, s => true, s => s.ToUpper(), "six");
                    List<string> cells = (List<string>)spreadsheet.GetNamesOfAllNonemptyCells();

                    // Update the grid widget with current cells values
                    foreach (string cell in cells)
                    {
                        object value = spreadsheet.GetCellValue(cell);
                        string realValue;

                        if (value is FormulaError)
                        {
                            realValue = "Error";
                        }
                        else
                        {
                            realValue = value.ToString();
                        }

                        string cellCol = cell.Substring(0, 1);
                        int widgetCol = FindColValue(cellCol);
                        int widgetRow = (Int32.Parse(cell.Substring(1, 1))) - 1;
                        gridWidget.SetValue(widgetCol, widgetRow, realValue);
                    }
                    gridWidget.SetSelection(0, 0, true);
                }
            }
        }

        /// <summary>
        /// Event handler for clicking on Sum Column button
        /// </summary>
        /// <param name="sender"> sum column button </param>
        /// <param name="e"> args </param>
        private void sumColumn_Click(object sender, EventArgs e)
        {
            // Get the column the user wants to sum
            string columnToSum = colToSumTextBox.Text.ToUpper();
            double sum = 0;
            List<string> cells = (List<string>)spreadsheet.GetNamesOfAllNonemptyCells();

            // Loop over all cells
            foreach (string cell in cells)
            {
                // If a cell name is within the provided column, add its value to the sum
                // If the value is non-numerical, skip over the cell.
                if (cell.Substring(0, 1) == columnToSum)
                {
                    object value = spreadsheet.GetCellValue(cell);
                    if (value is FormulaError || value is string)
                    {
                        continue;
                    }
                    else
                    {
                        sum += (double)value;
                    }
                }
            }
            // Show the columns sum
            sumOfColTextBox.Text = sum.ToString();
        }

        /// <summary>
        /// Event handler for clicking the Help button 
        /// </summary>
        /// <param name="sender"> help button </param>
        /// <param name="e"> args </param>
        private void helpButton_Click(object sender, EventArgs e)
        {
            // Message to display
            const string message = "Spreadsheet Help\n\n* The File tab allows the user to:\n    - Create a new spreadsheet window\n    - Close the current spreadsheet\n    - Save the spreadsheet to a file\n    - Open a previously saved spreadsheet\n" +
                "* To edit the contents of a cell, select the desired cell by clicking on it, then click on the Cell Contents text box and type in the contents. In order to see the cell's value appear within the spreadsheet, click on the cell again or click on any other cell.\n" +
                "* To insert Formulas, make sure to prepend an equals sign ('=') before the formula. Also, cell names can be either upper or lower case.\n" +
                "* Additional Feature: By providing a column name (either upper or lower case) in the Add Column box, the user can add all numerical contents within that column by clicking the Sum Column button. The sum will appear in the nearby text box. If there are non-numerical values in the column, they will be ignored" +
                ". If there are no numerical elements in a column, or a column name is not provided, the sum will default to 0.";
            const string caption = "Help";
            // Show the help menu
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        /// <summary>
        /// Finds the name given the column index
        /// </summary>
        /// <param name="col"> col index </param>
        /// <returns> cell name </returns>
        private string FindColName(int col)
        {
            // Return the correct name depending on the column number
            switch (col)
            {
                case 0: return "A";
                case 1: return "B";
                case 2: return "C";
                case 3: return "D";
                case 4: return "E";
                case 5: return "F";
                case 6: return "G";
                case 7: return "H";
                case 8: return "I";
                case 9: return "J";
                case 10: return "K";
                case 11: return "L";
                case 12: return "M";
                case 13: return "N";
                case 14: return "O";
                case 15: return "P";
                case 16: return "Q";
                case 17: return "R";
                case 18: return "S";
                case 19: return "T";
                case 20: return "U";
                case 21: return "V";
                case 22: return "W";
                case 23: return "X";
                case 24: return "Y";
                case 25: return "Z";
            }
            return "";
        }

        /// <summary>
        /// Finds the name given the column index
        /// </summary>
        /// <param name="col"> col index </param>
        /// <returns> cell name </returns>
        private int FindColValue(string col)
        {
            // Return the correct number depending on the column name
            switch (col)
            {
                case "A": return 0;
                case "B": return 1;
                case "C": return 2;
                case "D": return 3;
                case "E": return 4;
                case "F": return 5;
                case "G": return 6;
                case "H": return 7;
                case "I": return 8;
                case "J": return 9;
                case "K": return 10;
                case "L": return 11;
                case "M": return 12;
                case "N": return 13;
                case "O": return 14;
                case "P": return 15;
                case "Q": return 16;
                case "R": return 17;
                case "S": return 18;
                case "T": return 19;
                case "U": return 20;
                case "V": return 21;
                case "W": return 22;
                case "X": return 23;
                case "Y": return 24;
                case "Z": return 25;
            }
            return -1;
        }


    }
}