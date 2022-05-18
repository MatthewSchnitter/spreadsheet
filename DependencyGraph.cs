// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SpreadsheetUtilities
{    /// <summary> 
     /// Author:    Matthew Schnitter 
     /// Partner:   None
     /// Date:      1/28/2022
     /// Course:    CS 3500, University of Utah, School of Computing 
     /// Copyright: CS 3500 and Matthew Schnitter - This work may not be copied for use in Academic Coursework. 
     /// 
     /// I, Matthew Schnitter, certify that I wrote this code from scratch and did not copy it in part or whole from  
     /// another source.  All references used in the completion of the assignment are cited in my README file. 
     /// 
     /// File Contents 
     /// 
     ///  Contains the implementation for DependencyGraph
     ///    
     /// </summary>

     /// <summary>
     /// (s1,t1) is an ordered pair of strings
     /// t1 depends on s1; s1 must be evaluated before t1
     /// 
     /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two 
     /// ordered pairs
     /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 
     ///equals t2.
     /// Recall that sets never contain duplicates.  If an attempt is made to add an 
     ///element to a
     /// set, and the element is already in the set, the set remains unchanged.
     /// 
     /// Given a DependencyGraph DG:
     /// 
     ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is
     ///called dependents(s).
     ///        (The set of things that depend on s)    
     ///        
     ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is
     ///called dependees(s).
     ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        private Dictionary<string, List<string>> dependents;
        private Dictionary<string, List<string>> dependees;
        private int graphSize;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependents = new Dictionary<string, List<string>>();
            dependees = new Dictionary<string, List<string>>();
            graphSize = 0;
        }

        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return graphSize; }
        }

        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you
        /// would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                if (dependees.ContainsKey(s))
                {
                    List<string> dependeeValues = new List<string>();
                    bool containsDependeeElement = dependees.TryGetValue(s, out dependeeValues);

                    // We know dependeeValues has a value because we have already chekced
                    // if dependees contains the key for that value.
                    return dependeeValues.Count();
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if (dependents.ContainsKey(s))
            {
                List<string> dependentValues = new List<string>();
                bool containsDependentElement = dependents.TryGetValue(s, out dependentValues);

                return containsDependentElement;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            if (this[s] > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (dependents.ContainsKey(s))
            {
                List<string> dependentValues = new List<string>();
                bool containsDependentElement = dependents.TryGetValue(s, out dependentValues);
                return dependentValues;
            }
            else
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (dependees.ContainsKey(s))
            {
                List<string> dependeeValues = new List<string>();
                bool containsDependeeElement = dependees.TryGetValue(s, out dependeeValues);
                return dependeeValues;
            }
            else
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// <para>This should be thought of as: t depends on s </para>   
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S </param>
        /// <param name="t"> t cannot be evaluated until s is</param>     
        public void AddDependency(string s, string t)
        {
            List<string> dependentValues = new List<string>();
            List<string> dependeeValues = new List<string>();
            bool containsDependentElement = false;
            bool containsDependeeElement = false;

            // We need to first check whether s and t already exists in dependees/dependents
            // If s or t exist if their dictionaries, we get those values
            if (dependents.ContainsKey(s) && dependees.ContainsKey(t))
            {
                containsDependentElement = dependents.TryGetValue(s, out dependentValues);
                containsDependeeElement = dependees.TryGetValue(t, out dependeeValues);
            }
            else if (dependents.ContainsKey(s) && !dependees.ContainsKey(t))
            {
                containsDependentElement = dependents.TryGetValue(s, out dependentValues);
            }
            else if (!dependents.ContainsKey(s) && dependees.ContainsKey(t))
            {
                containsDependeeElement = dependees.TryGetValue(t, out dependeeValues);
            }

            // Depending on if dependents and dependees already contain s and t,
            // the process for adding changes.
            if (containsDependentElement == true && containsDependeeElement == true)
            {
                if (dependentValues.Contains(t))
                {
                    return;
                }

                // If dependents contains s and/or dependees contain t,
                // the key must be removed in order to add it back in with the 
                // correct dependency added to its list
                dependents.Remove(s);
                dependentValues.Add(t);
                dependents.Add(s, dependentValues);

                dependees.Remove(t);
                dependeeValues.Add(s);
                dependees.Add(t, dependeeValues);

                graphSize++;
            }
            else if (containsDependentElement == true && containsDependeeElement == false)
            {
                if (dependentValues.Contains(t))
                {
                    return;
                }

                dependents.Remove(s);
                dependentValues.Add(t);
                dependents.Add(s, dependentValues);

                dependeeValues.Add(s);
                dependees.Add(t, dependeeValues);

                graphSize++;
            }
            else if (containsDependentElement == false && containsDependeeElement == true)
            {
                if (dependentValues.Contains(t))
                {
                    return;
                }

                dependentValues.Add(t);
                dependents.Add(s, dependentValues);

                dependees.Remove(t);
                dependeeValues.Add(s);
                dependees.Add(t, dependeeValues);

                graphSize++;
            }
            else
            {
                dependentValues.Add(t);
                dependents.Add(s, dependentValues);

                dependeeValues.Add(s);
                dependees.Add(t, dependeeValues);


                graphSize++;
            }
        }

        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if (dependents.Count == 0)
            {
                return;
            }

            List<string> dependentValues = new List<string>();
            List<string> dependeeValues = new List<string>();

            if (dependents.ContainsKey(s))
            {
                bool containsDependentElement = dependents.TryGetValue(s, out dependentValues);
                bool containsDependeeElement = dependees.TryGetValue(t, out dependeeValues);

                if (containsDependentElement)
                {
                    if (!dependentValues.Contains(t))
                    {
                        return;
                    }

                    // If there is only one value, need to simply remove the key
                    // so it does not point to an empty list
                    if (dependentValues.Count == 1 && dependeeValues.Count == 1)
                    {
                        dependents.Remove(s);
                        dependees.Remove(t);

                        graphSize--;
                    }
                    else if (dependentValues.Count == 1 && dependeeValues.Count != 1)
                    {
                        dependents.Remove(s);

                        dependees.Remove(t);
                        dependeeValues.Remove(s);
                        dependees.Add(t, dependeeValues);

                        graphSize--;
                    }
                    else if(dependentValues.Count != 1 && dependeeValues.Count == 1)
                    {
                        dependents.Remove(s);
                        dependentValues.Remove(t);
                        dependents.Add(s, dependentValues);

                        dependees.Remove(t);

                        graphSize--;
                    }
                    else
                    {
                        dependents.Remove(s);
                        dependentValues.Remove(t);
                        dependents.Add(s, dependentValues);

                        dependees.Remove(t);
                        dependeeValues.Remove(s);
                        dependees.Add(t, dependeeValues);

                        graphSize--;
                    }


                }
            }
            else
            {
                return;
            }

        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (!dependents.ContainsKey(s))
            {
                return;
            }

            List<string> oldDependents = new List<string>();
            bool containsOldDependents = dependents.TryGetValue(s, out oldDependents);

            // Use Add/RemoveDependency methods in order to cover
            // both dictionaries
            if (oldDependents != null)
            {
                foreach (string t in oldDependents.ToList())
                {
                    this.RemoveDependency(s, t);
                }
            }

            foreach (string t in newDependents.ToList())
            {
                this.AddDependency(s, t);
            }

        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            Dictionary<string, List<string>> oldDependents = new Dictionary<string, List<string>>();
            oldDependents = dependents;

            foreach (KeyValuePair<string, List<string>> entry in oldDependents) // See reference 1 in README
            {
                if (entry.Value.ToList().Contains(s))
                {
                    this.RemoveDependency(entry.Key, s);
                }
            }

            foreach (string t in newDependees.ToList())
            {
                this.AddDependency(t, s);
            }

        }

    }
}
