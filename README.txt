```
Author:     Matthew Schnitter
Partner:    None
Date:       2/18/2022
Course:     CS 3500, University of Utah, School of Computing
GitHub ID:  MatthewSchnitter
Repo:       https://github.com/MatthewSchnitter/spreadsheet
Commit #:   fbbff5384d15445bdcc56da904fa791dcfd55070
Project:    Spreadsheet
Copyright:  CS 3500 and Matthew Schnitter - This work may not be copied for use in Academic Coursework.
```

# Comments to Evaluators:

This project contains the implementation of the AbstractSpreadsheets API.

# Assignment Specific Topics

Spreadsheet keeps track of cells and their dependencies on one another.
It contains its own nested class called Cell which holds the contents of type object. Also,
Spreadsheet can get the contents of the cell, set the contents of cells to a double, formula or string, 
and give a list of a cells dependents, as well as returning a list of all non-empty cells.
It can save and read from a file, as well as get the values of the cells.

# Consulted Peers:

My peers from lab discussed how DependencyGraph would factor in, but did not discuss
how we would handle the dependencies. We also discussed ways of using XMLReader like viewing 
the attributes.

# References:

    1. Recursive Functions - 
    https://docs.microsoft.com/en-us/cpp/c-language/recursive-functions?view=msvc-170
    2. XmlReaderClass -
    https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmlreader?view=net-6.0#methods
    3. .NET API - 
    https://docs.microsoft.com/en-us/dotnet/
