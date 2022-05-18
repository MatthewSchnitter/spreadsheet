using SpreadsheetUtilities;
using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

/// <summary> 
/// Author:    Matthew Schnitter 
/// Partner:   None 
/// Date:      2/18/2022 
/// Course:    CS 3500, University of Utah, School of Computing 
/// Copyright: CS 3500 and Matthew Schnitter - This work may not be copied for use in Academic Coursework. 
/// 
/// I, Matthew Schnitter, certify that I wrote this code from scratch and did not copy it in part or whole from  
/// another source.  All references used in the completion of the assignment are cited in my README file. 
/// 
/// File Contents 
/// This file contains the inherited methods from AbstractSpreadsheet implemented according
/// to AbstractSpreadhseet's API.
/// </summary>

namespace SS
{
    /// <inheritdoc/>
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<string, Cell> cells;
        private DependencyGraph dependencies;
        private bool isChanged;

        /// <inheritdoc/>
        public override bool Changed { get => isChanged; protected set => isChanged = value; }

        /// <summary>
        /// This private class handles Cells which are inserted into the spreadsheet.
        /// It can create a new cell, get, and set the contents of the cell.
        /// Contents is of the type object to account how the Cell's contents 
        /// being a double, string, or Formula.
        /// </summary>
        private class Cell
        {
            private object contents;

            /// <summary>
            /// Contructor which sets the cell's contents ot its input.
            /// </summary>
            /// <param name="newContents"> the cells new contents </param>
            public Cell(object newContents)
            {
                contents = newContents;
            }

            /// <summary>
            /// Defualt constructor sets contents to an empty string.
            /// </summary>
            public Cell()
            {
                contents = "";
            }

            /// <summary>
            /// Gets contents
            /// </summary>
            public object Contents { get { return contents; } }

            /// <summary>
            /// Sets the contents of the cell to the given input.
            /// </summary>
            /// <param name="contents"> the new contents of the cell </param>
            public void SetContents(object contents)
            {
                this.contents = contents;
            }
        }

        /// <summary>
        /// The constructor for Spreadsheet sets the Dictionary mapping for cells to a new Dictionary
        /// with strings being the key and Cell name, and an instance of the Cell class being the value.
        /// Also creates an empty dependency graph to keep track of Dependencies. Sets version to default
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            cells = new Dictionary<string, Cell>();
            dependencies = new DependencyGraph();
        }

        /// <summary>
        /// This constructor takes in an isValid and Normalize delegate, as well as a version name
        /// </summary>
        /// <param name="isValid"> validity delegate </param>
        /// <param name="normalize"> normalize delgate </param>
        /// <param name="version"> version name </param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Cell>();
            dependencies = new DependencyGraph();
        }

        /// <summary>
        /// This constructor takes in an isValid and Normalize delegate, as well as a version name
        /// Also creates a new spreadsheet based off of  the given file
        /// </summary>
        /// <param name="PathToFile"> the file to read from </param>
        /// <param name="isValid"> validity delegate </param>
        /// <param name="normalize"> normalize delgate </param>
        /// <param name="version"> version name </param>
        public Spreadsheet(string PathToFile, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Cell>();
            dependencies = new DependencyGraph();
            LoadFromFile(PathToFile); //Load the new given into this spreadsheet
        }

        /// <inheritdoc/>
        public override object GetCellContents(string name)
        {
            if (name == null || !ExtensionMethods.checkForVariable(name) || !IsValid(name))
            {
                throw new InvalidNameException();
            }

            // Get the cell which is mapped to the input parameter
            Cell cell;
            bool hasCell = cells.TryGetValue(name, out cell);

            // If the name of the cell, has a cell mapped to it in the dictionary,
            // and is not null, check the contents of the cell
            if (hasCell && cell != null)
            {
                // Since contents is of the type object, we need to indivudally check
                // all possible types of contents, and return the correct type.
                if (cell.Contents is String)
                {
                    return (String)cell.Contents;
                }
                else if (cell.Contents is Double)
                {
                    return (Double)cell.Contents;
                }
                else
                {
                    return (Formula)cell.Contents;
                }
            }
            // If the cell's name does not have a mapping, the cell is considered
            // empty, and an empty string is returned.
            else
            {
                return "";
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            // Loop over the keys of the cell dictionary to get all cells
            // which contain contents. Return the list of all names
            List<string> names = new List<string>();
            foreach (string name in cells.Keys)
            {
                names.Add(name);
            }

            return names;
        }

        /// <inheritdoc/>
        protected override IList<string> SetCellContents(string name, double number)
        {
            // If the cell already contains contents,
            // we need to remove dependencies is a formula is being replaced, then
            // replace the old contents with the new contents 
            if (cells.ContainsKey(name))
            {
                RemoveDependenciesIfFormulaIsReplaced(name);
                cells.GetValueOrDefault(name).SetContents(number);
            }
            // Otherwise create a new cell and add it under the input name
            else
            {
                Cell newCell = new Cell(number);
                cells.Add(name, newCell);
            }

            // Get the cells that are dependent on the given cell and return that list
            List<string> cellDependencies = new List<string>();
            cellDependencies.Add(name);
            return GetDependencies(name, cellDependencies);
        }

        /// <inheritdoc/>
        protected override IList<string> SetCellContents(string name, string text)
        {
            // If the cell already contains contents,
            // we need to remove dependencies is a formula is being replaced, then
            // replace the old contents with the new contents
            if (cells.ContainsKey(name))
            {
                RemoveDependenciesIfFormulaIsReplaced(name);
                cells.GetValueOrDefault(name).SetContents(text);
            }
            // Otherwise create a new cell and add it under the input name
            else
            {
                Cell newCell = new Cell(text);
                cells.Add(name, newCell);
            }

            // Get the cells that are dependent on the given cell and return that list
            List<string> cellDependencies = new List<string>();
            cellDependencies.Add(name);
            return GetDependencies(name, cellDependencies);
        }

        /// <inheritdoc/>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            if (formula is null)
            {
                throw new ArgumentNullException("Formula is null");
            }

            // Get the variables contained in the formula
            List<string> variables = (List<string>)formula.GetVariables();

            // If the cell already contains contents,
            // we need to remove dependencies is a formula is being replaced, then
            // replace the old contents with the new contents
            if (cells.ContainsKey(name))
            {
                RemoveDependenciesIfFormulaIsReplaced(name);
                cells.GetValueOrDefault(name).SetContents(formula);

                // Need to add a dependency for each variable in the formula for the cell
                foreach (string variable in variables)
                {
                    dependencies.AddDependency(variable, name);
                }
                // Checks for cicrular dependency
                LinkedList<string> cellsToRecalculate = (LinkedList<string>)GetCellsToRecalculate(name);
            }
            // Otherwise create a new cell and add it under the input name
            else
            {
                Cell newCell = new Cell(formula);
                cells.Add(name, newCell);
                // Need to add a dependency for each variable in the formula for the cell
                foreach (string variable in variables)
                {
                    dependencies.AddDependency(variable, name);
                }
                // Checks for cicrular dependency
                LinkedList<string> cellsToRecalculate = (LinkedList<string>)GetCellsToRecalculate(name);
            }

            // Get the cells that are dependent on the given cell and return that list
            List<string> cellDependencies = new List<string>();
            cellDependencies.Add(name);
            return GetDependencies(name, cellDependencies);
        }

        /// <inheritdoc/>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            // Use GetDependents method from DependencyGraph
            return dependencies.GetDependents(name);
        }

        /// <summary>
        /// This method recuresively gets all depedencies  of the cell 
        /// for use in SetCellContents.
        /// </summary>
        /// <param name="name"> the name of the cell </param>
        /// <param name="currentDependencies"> the list of dependencies </param>
        /// <returns></returns>
        private List<string> GetDependencies(string name, List<string> currentDependencies)
        {
            // Get the direct dependents of the cell
            List<string> dependents = (List<string>)GetDirectDependents(name);
            foreach (string dependent in dependents)
            {
                // Get the dependents of every variable in that cell
                currentDependencies = GetDependencies(dependent, currentDependencies);
                // Add the current dependent to the list if it is not present
                if (!currentDependencies.Contains(dependent))
                {
                    currentDependencies.Add(dependent);
                }
            }
            return currentDependencies;
        }

        /// <summary>
        /// See method title
        /// </summary>
        /// <param name="name"> the cell to replace dependencies </param>
        private void RemoveDependenciesIfFormulaIsReplaced(string name)
        {
            if (GetCellContents(name) is Formula)
            {
                // If the current contents are a formula, get the variables of
                // formula and remove the dependencies of the variables from the cell
                Formula formula = (Formula)GetCellContents(name);
                List<string> variables = (List<string>)formula.GetVariables();
                foreach (string variable in variables)
                {
                    dependencies.RemoveDependency(variable, name);
                }
            }
        }

        /// <inheritdoc/>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            //Check valididty
            if (name == null || !ExtensionMethods.checkForVariable(name) || !IsValid(name))
            {
                throw new InvalidNameException();
            }

            name = Normalize(name);

            double value;
            bool isDouble = double.TryParse(content, out value);
            if (isDouble == true)
            {
                // If the contents is a double call the SetCellContents which takes in a double
                List<string> cellDependencies = (List<string>)SetCellContents(name, value);
                Changed = true;
                return cellDependencies;
            }
            else if (content.StartsWith("="))
            {
                // If content starts with an "=" it is a formula
                string newContent = content.Substring(1); // Append the formula so the "=" is gone
                Formula formula = new Formula(newContent, Normalize, IsValid); // Create a normalized, valid formula
                List<string> cellDependencies = (List<string>)SetCellContents(name, formula);
                Changed = true;
                return cellDependencies;
            }
            else
            {   // Otherwise, add the contents if it were a string
                List<string> cellDependencies = (List<string>)SetCellContents(name, content);
                Changed = true;
                return cellDependencies;
            }
        }

        /// <inheritdoc/>
        public override string GetSavedVersion(string filename)
        {
            // Create an XmlReader inside this block, and automatically Dispose() it at the end.
            using (XmlReader reader = XmlReader.Create(filename))
            {

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                // This is an example of reading an attribute on an element
                                string version = reader["version"];
                                return version;
                        }
                    }
                }

                throw new SpreadsheetReadWriteException("File does not contain a version");
            }
        }

        /// <summary>
        /// A helper method for the constructor where a filepath to build a new spreadsheet
        /// from is passed in. Reads the file and creates the spreadsheet as it reads.
        /// </summary>
        /// <param name="filepath"> the file to read from </param>
        /// <exception cref="SpreadsheetReadWriteException"> thrown if an error occurs</exception>
        private void LoadFromFile(string filepath)
        {
            // Create an XmlReader inside this block, and automatically Dispose() it at the end.
            XmlReader reader;
            try
            {
                reader = XmlReader.Create(filepath);
            }
            catch (FileNotFoundException)
            {
                throw new SpreadsheetReadWriteException("File not found");
            }


            using (reader)
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                // This is an example of reading an attribute on an element
                                string version = reader["version"];
                                if (version != Version)
                                {
                                    throw new SpreadsheetReadWriteException("Version does not match");
                                }
                                break; // no more direct info to read on spreadsheet

                            case "cell":

                                // Get to the node that contains name
                                while (reader.Read())
                                {
                                    if (reader.Name == "name")
                                    {
                                        break;
                                    }
                                }

                                // Get the name of the cell
                                reader.Read();
                                string name = reader.Value;

                                // Get to the node that contains contents
                                while (reader.Read())
                                {
                                    if (reader.Name == "contents")
                                    {
                                        break;
                                    }
                                }
                             
                                //Get the cell contents
                                reader.Read();
                                string contents = reader.Value;

                                // Add to the spreadsheet
                                SetContentsOfCell(name, contents);

                                break;

                            // Continues until end of file
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void Save(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "";

            // Create an XmlWriter inside this block, and automatically Dispose() it at the end.
            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");

                // This adds an attribute to the spreadsheet element
                writer.WriteAttributeString("version", Version);

                // write the cells themselves
                foreach (KeyValuePair<string, Cell> currentCell in cells)
                {
                    writer.WriteStartElement("cell");
                    // We use a shortcut to write an element with a single string
                    writer.WriteElementString("name", currentCell.Key);

                    // Depending on the contents of the cell convert it to a string then add
                    if (currentCell.Value.Contents is string)
                    {
                        string contents = (string)currentCell.Value.Contents;
                        writer.WriteElementString("contents", contents);
                    }
                    else if (currentCell.Value.Contents is Double)
                    {
                        string contents = currentCell.Value.Contents.ToString();
                        writer.WriteElementString("contents", contents);
                    }
                    else
                    {
                        string contents = currentCell.Value.Contents.ToString();
                        contents = "=" + contents; // Prepend a "=" if it is a formula
                        writer.WriteElementString("contents", contents);
                    }

                    writer.WriteEndElement(); // Ends the cell block
                }

                writer.WriteEndElement(); // Ends the spreadsheet block
                writer.WriteEndDocument();
            }
            // Set changed to false since the spreadsheet is saved
            Changed = false;
        }

        /// <inheritdoc/>
        public override object GetCellValue(string name)
        {
            if (name == null || !ExtensionMethods.checkForVariable(name) || !IsValid(name))
            {
                throw new InvalidNameException();
            }

            // Get the cells conntents
            Cell cell = cells.GetValueOrDefault(name);
            object contents = cell.Contents;

            if (contents is Formula)
            {
                // If contents is a formula, evaluate the formula to get its value
                Formula formula = (Formula)contents;
                object value = formula.Evaluate(Lookup);
                return value; // Can return a FormulaFormatException if Evaluate fails
            }
            else
            {
                return contents;
            }
        }

        /// <summary>
        /// This is the lookup delegate which is passed into Evaluate.
        /// If the contents of the cell to lookup is not a double or another formula,
        /// returns an throws an InvalidCastException which is caught by Evaluate. This tells
        /// Evaluate to return a FormulaError. Otherwise evaluate the Formula of the cell and 
        /// so on until the value of the cell is returned.
        /// </summary>
        /// <param name="name"> the cell to lookup </param>
        /// <returns> the value of the cell, or a FormulaError </returns>
        /// <exception cref="InvalidCastException"> thrown if the cell contains a string or FormulaError </exception>
        private double Lookup(string name)
        {
            Cell cell = cells.GetValueOrDefault(name);
            object contents = cell.Contents;

            if (contents is Formula)
            { 
                // If this cell contains a formula, evaluate it
                Formula formula = (Formula)contents;
                double value = (double)formula.Evaluate(Lookup);
                return value;
            }
            // Throw expcetion is contents are string or FormulaError
            else if (contents is string)
            {
                throw new InvalidCastException();
            }
            else if (contents is FormulaError)
            {
                throw new InvalidCastException();
            }
            return (double)contents;
        }
    }
}
