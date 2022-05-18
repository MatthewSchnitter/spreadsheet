// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens

/// <summary> 
/// Author:    Matthew Schnitter 
/// Partner:   None 
/// Date:      2/1/2022 
/// Course:    CS 3500, University of Utah, School of Computing 
/// Copyright: CS 3500 and Matthew Schnitter - This work may not be copied for use in Academic Coursework. 
/// 
/// I, Matthew Schnitter, certify that I wrote this code from scratch and did not copy it in part or whole from  
/// another source.  All references used in the completion of the assignment are cited in my README file. 
/// 
/// File Contents 
/// This file contains the FormulaEvaluator reworked into a class called Formula 
/// </summary>


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        private List<string> tokens = new List<string>();
        private List<string> variables = new List<string>();

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
            // Turn the tokens into a list in order to use list methods
            IEnumerable<string> enumeratedTokens = Formula.GetTokens(formula);
            tokens = enumeratedTokens.ToList();
            // Helper method which checks sytax
            checkSyntaxAndGatherVariables();
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            IEnumerable<string> enumeratedTokens = Formula.GetTokens(formula);
            List<string> originalTokens = enumeratedTokens.ToList();

            // This loop normalizes and checks if varaibles are valid
            foreach (string token in originalTokens.ToList())
            {
                bool isVariable = checkForVariable(token);
                if (isVariable)
                {
                    // If token is a variable, normalize it and check for legality
                    string normalizedVari = normalize(token);
                    bool legalVari = checkForVariable(normalizedVari);

                    // If it is legal replace the variable with the normalized version
                    if (legalVari)
                    {
                        int index = originalTokens.IndexOf(token);
                        originalTokens.RemoveAt(index);
                        originalTokens.Insert(index, normalizedVari);
                    }
                    else
                    {
                        throw new FormulaFormatException("Normalized variable is illegal");
                    }

                    // Check if the variable is valid based on isValid
                    bool validVari = isValid(normalizedVari);
                    if (!validVari)
                    {
                        throw new FormulaFormatException("Normalized variable is invalid");
                    }

                }
            }

            // Check syntax
            tokens = originalTokens.ToList();
            checkSyntaxAndGatherVariables();
        }

        /// <summary>
        /// This is a helper method for both constructors which 
        /// throws a FormulaFormatException if the expression is syntactically invalid.
        /// It also gathers all unique variables in the formula for later use in
        /// the GetVariables() method.
        /// </summary>
        private void checkSyntaxAndGatherVariables()
        {
            // Gather all variables for later use in GetVariables
            foreach (string token in tokens)
            {
                bool isVari = checkForVariable(token);
                if (isVari)
                {
                    // Only add if it is a different variable
                    if (!variables.Contains(token))
                    {
                        variables.Add(token);
                    }
                }
            }

            if (tokens.Count == 0)
            {
                throw new FormulaFormatException("Formula is empty");
            }

            // Need to check if first and last tokens are valid
            string firstToken = tokens.First();
            string lastToken = tokens.Last();
            double num;
            bool IsNum = Double.TryParse(firstToken, out num);

            // Must be a number or right parenthese
            if (IsNum == false && firstToken != "(")
            {
                bool isVariable = checkForVariable(firstToken);
                if (isVariable == false)
                {
                    throw new FormulaFormatException("Invalid starting token");
                }
            }

            IsNum = Double.TryParse(lastToken, out num);

            // Must be a number or left parenthese
            if (IsNum == false && lastToken != ")")
            {
                bool isVariable = checkForVariable(lastToken);
                if (isVariable == false)
                {
                    throw new FormulaFormatException("Invalid ending token");
                }
            }

            // Variables to keep track of what tokens happen when
            int openParen = 0;
            int closeParen = 0;
            bool followsOpenParenOrOperator = false;
            bool followsNumVariCloseParen = false;

            foreach (string token in tokens)
            {
                // Check for a number or variable
                double loopNum;
                bool IsLoopNum = Double.TryParse(token, out loopNum);
                bool isVariable = checkForVariable(token);
                bool isOperator = false;

                if (token == "+" || token == "-" || token == "*" || token == "/")
                {
                    isOperator = true;
                }

                // If the token follows an open parenthese or operator, ensure the following token is valid
                if (followsOpenParenOrOperator == true)
                {
                    if (IsLoopNum == false && token != "(" && isVariable == false)
                    {
                        throw new FormulaFormatException("A number, variable, or open parenthese must follow an open parenthese or operator");
                    }
                    followsOpenParenOrOperator = false;
                }
                // If the token follows a number, variable, or close parenthese, ensure the following token is valid
                if (followsNumVariCloseParen == true)
                {
                    if (isOperator == false && token != ")")
                    {
                        throw new FormulaFormatException("An open parenthese or operator must follow a number, variable, or open parenthese");
                    }
                    followsNumVariCloseParen = false;
                }

                // If variable, number, or operator, next bool to true to ensure following token is valid
                if (isVariable == true)
                {
                    followsNumVariCloseParen = true;
                }
                if (IsLoopNum == true)
                {
                    followsNumVariCloseParen = true;
                }
                if (isOperator == true)
                {
                    followsOpenParenOrOperator = true;
                }


                if (token == "(")
                {
                    // Add total number of parentheses to keep track of balance
                    openParen++;
                    followsOpenParenOrOperator = true;
                }
                if (token == ")")
                {
                    closeParen++;
                    followsNumVariCloseParen = true;
                }
                if (closeParen > openParen)
                {
                    throw new FormulaFormatException("Close parentheses greater than open parentheses");
                }

                // If token is invalid, throw and expcetion
                if (token != "(" && token != ")" && token != "+" && token != "-" && token != "*" && token != "/" && IsLoopNum == false && isVariable == false)
                {
                    throw new FormulaFormatException("Invalid token");
                }

            }

            // If number of parentheses are unequal after the loop, throw and exception
            if (closeParen != openParen)
            {
                throw new FormulaFormatException("Parentheses are unbalanced");
            }
        }

        /// <summary>
        /// This method checks if a token is a variable by checking the beginning and end
        /// for a letter and number, repsectively. This is done by looping through all
        /// possible letters and numbers.
        /// </summary>
        /// <param name="token"> the token to check </param>
        /// <returns> A bool telling if the token is a variable </returns>
        private static bool checkForVariable(string token)
        {
            // Valid letters and numbers
            string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string nums = "0123456789";
            foreach (char letter in letters)
            {
                // First check if the beginning of the variable is valid
                if (token.StartsWith(letter) || token.StartsWith('_'))
                {
                    // Then, as long as it ends in a letter, numnber, or underscore, return true
                    foreach (char endLetter in letters)
                    {
                        if (token.EndsWith(endLetter) || token.EndsWith('_'))
                            return true;
                    }

                    foreach (char number in nums)
                    {
                        if (token.EndsWith(number) || token.EndsWith('_'))
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            Stack<string> operators = new Stack<string>();
            Stack<double> values = new Stack<double>();

            // This foreach loop is the main algortihm. Does different functions 
            // depending on the token.
            foreach (string token in tokens)
            {
                double currentValue;
                bool isDouble = double.TryParse(token, out currentValue);
                if (isDouble == true)
                {
                    // If token is a number, and operator count is more than 0, either multiply
                    // or divide, otherwise push it onto the value stack
                    if (operators.Count > 0 && operators.Peek() == "*")
                    {
                        double newValue = multiply(operators, values, currentValue);
                        values.Push(newValue);
                    }
                    else if (operators.Count > 0 && operators.Peek() == "/")
                    {
                        if (currentValue == 0)
                        {
                            return new FormulaError("Cannot divide by 0");
                        }
                        double newValue = divide(operators, values, currentValue);
                        values.Push(newValue);
                    }
                    else
                    {
                        values.Push(currentValue);
                    }
                }
                // If token is a + or -, and operators > 0, either add or subtract,
                // otherwise push value onto stack
                else if (token.Equals("+") || token.Equals("-"))
                {
                    if (operators.Count > 0 && operators.Peek() == "+")
                    {
                        double newValue = add(operators, values);
                        values.Push(newValue);
                        operators.Push(token);
                    }
                    else if (operators.Count > 0 && operators.Peek() == "-")
                    {
                        double newValue = subtract(operators, values);
                        values.Push(newValue);
                        operators.Push(token);
                    }
                    else
                    {
                        operators.Push(token);
                    }
                }
                // If token is *, /, or (, push it onto its stack
                else if (token.Equals("*") || token.Equals("/"))
                {
                    operators.Push(token);
                }
                else if (token.Equals("("))
                {
                    operators.Push(token);
                }

                else if (token.Equals(")"))
                {
                    // if token is ), check if the top of the operator stack is 
                    // + or -, add if so add or subtract
                    if (operators.Count > 0 && operators.Peek() == "+")
                    {
                        double newValue = add(operators, values);
                        values.Push(newValue);
                    }
                    else if (operators.Count > 0 && operators.Peek() == "-")
                    {
                        double newValue = subtract(operators, values);
                        values.Push(newValue);
                    }

                    //Pop the open parenthese
                    operators.Pop();

                    // If top of operator is * or /, either multiply or divide
                    if (operators.Count > 0 && operators.Peek() == "*")
                    {
                        operators.Pop();
                        double topValue1 = values.Pop();
                        double topValue2 = values.Pop();
                        double newValue = topValue1 * topValue2;
                        values.Push(newValue);
                    }
                    else if (operators.Count > 0 && operators.Peek() == "/")
                    {
                        operators.Pop();
                        double topValue1 = values.Pop();
                        double topValue2 = values.Pop();

                        // Return FormulaError if divide by 0
                        if (topValue1 == 0)
                        {
                            return new FormulaError("Cannot divide by 0");
                        }

                        double newValue = topValue2 / topValue1;
                        values.Push(newValue);
                    }

                }
                else
                {
                    // If token is not a number or operator, it must be a variable,
                    // so look it up
                    double variableValue;
                    try
                    {
                        variableValue = lookup(token);
                    }
                    catch (InvalidCastException e)
                    {
                        return new FormulaError("Invalid Formula");
                    }
                   
                    // Either multiply or divide depending on operator stack, otherwise
                    // push the variable's value
                    if (operators.Count > 0 && operators.Peek() == "*")
                    {
                        double newValue = multiply(operators, values, variableValue);
                        values.Push(newValue);
                    }
                    else if (operators.Count > 0 && operators.Peek() == "/")
                    {
                        if (variableValue == 0)
                        {
                            return new FormulaError("Cannot divide by 0");
                        }
                        double newValue = divide(operators, values, variableValue);
                        values.Push(newValue);
                    }
                    else
                    {
                        values.Push(variableValue);
                    }
                }
            }

            // Returns the the final value left on the stack
            // or adds/subtracts the final two numbers if two remain
            if (operators.Count == 0)
            {
                double finalValue = values.Pop();
                return finalValue;
            }
            else
            {
                if (operators.Count > 0 && operators.Peek() == "+")
                {
                    double finalValue = add(operators, values);
                    return finalValue;
                }
                else
                {
                    double finalValue = subtract(operators, values);
                    return finalValue;
                }
            }
        }

        /// <summary>
        /// Helper method which adds the top two values on the value stack
        /// </summary>
        /// <param name="operators"> the operator stack </param>
        /// <param name="values"> the value stack </param>
        /// <returns> the resulting value </returns>
        private static double add(Stack<string> operators, Stack<double> values)
        {
            // Pop operator and value stack twice then add
            operators.Pop();
            double topValue1 = values.Pop();
            double topValue2 = values.Pop();
            double newValue = topValue1 + topValue2;
            return newValue;
        }

        /// <summary>
        /// Helper method which subtracts the top two values on the value stack
        /// </summary>
        /// <param name="operators"> the operator stack </param>
        /// <param name="values"> the value stack </param>
        /// <returns> the resulting value </returns>
        private static double subtract(Stack<string> operators, Stack<double> values)
        {
            // Pop operator and value stack twice then subtract
            operators.Pop();
            double topValue1 = values.Pop();
            double topValue2 = values.Pop();
            double newValue = topValue2 - topValue1;
            return newValue;
        }

        /// <summary>
        /// Helper method which multiplies the top value on the value stack and the current token
        /// </summary>
        /// <param name="operators"> the operator stack </param>
        /// <param name="values"> the value stack </param>
        /// <returns> the resulting value </returns>
        private static double multiply(Stack<string> operators, Stack<double> values, double multiplier)
        {
            // Pop operator and value stack once then multiply by multiplier
            operators.Pop();
            double topValue = values.Pop();
            double newValue = topValue * multiplier;
            return newValue;
        }

        /// <summary>
        /// Helper method which divides the top value on the value stack and the current token
        /// </summary>
        /// <param name="operators"> the operator stack </param>
        /// <param name="values"> the value stack </param>
        /// <returns> the resulting value </returns>
        private static double divide(Stack<string> operators, Stack<double> values, double divisor)
        {
            // Pop operator and value stack once then divide by divisor
            operators.Pop();
            double topValue = values.Pop();
            double newValue = topValue / divisor;
            return newValue;
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return variables;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            // Concatenate the tokens
            string formula = "";
            foreach (string token in tokens)
            {
                formula += token;
            }
            return formula;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || obj is not Formula)
            {
                return false;
            }

            // Cast obj to Formula to get tokens
            Formula otherFormula = (Formula)obj;

            if (tokens.Count != otherFormula.tokens.Count)
            {
                return false;
            }

            List<string> firstList = tokens;
            List<string> otherList = otherFormula.tokens;

            int otherIndex = 0;
            foreach (string token in firstList)
            {
                // Loop through both sets of tokens
                if (token != otherList[otherIndex])
                {
                    // If tokens do not equal check for numbers
                    double thisNum;
                    double otherNum;
                    bool IsThisNum = Double.TryParse(token, out thisNum);
                    bool IsOtherNum = Double.TryParse(otherList[otherIndex], out otherNum);

                    // If they are both numbers, convert back to string to check
                    if (IsThisNum && IsOtherNum)
                    {
                        string newThisNum = thisNum.ToString();
                        string newOtherNum = otherNum.ToString();
                        if (newThisNum != newOtherNum)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                otherIndex++;
            }
            // Return true if no differences found
            return true;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (f1 is null && f2 is null)
            {
                return true;
            }

            if (f1 is null && f2 is not null || f1 is not null && f2 is null)
            {
                return false;
            }

            // Call Equals to check equality
            if (f1.Equals(f2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            if (f1 is null && f2 is null)
            {
                return false;
            }

            if (f1 is null && f2 is not null || f1 is not null && f2 is null)
            {
                return true;
            }

            // Call Equals to check equality
            if (!f1.Equals(f2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            // Get the hashcode of each token and add them together to get formula HashCode
            int hash = 0;
            foreach (string token in tokens)
            {
                hash += token.GetHashCode();
            }
            return hash;
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}