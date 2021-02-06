using System;
using System.Collections.Generic;
using System.IO;

namespace NameSorter
{
    public class NameDetails
    {
        public string LastName { get; }
        public string FirstName { get; }

        public List<String> OtherNames { get; }

        public NameDetails(string lastName, string firstName)
        {
            LastName = lastName;
            FirstName = firstName;
            OtherNames = null;
        }

        public NameDetails(string lastName, string firstName, List<String> otherNames)
        {
            LastName = lastName;
            FirstName = firstName;
            OtherNames = otherNames;
        }
    }

    
    
    public class NameDetailsLineBuilder 
    {

        public List<String> debugLines { get; }
        

        public Boolean canBuildFromLine(String line)
        {
            if (String.IsNullOrEmpty(line))
		    {
			    return false;
		    }
		            
    		line = line.Trim();		
		
		    String[] parts = line.Split(" ");
		
		    if (parts.Length < 2)
		    {
    			return false;
		    }

            return true;
        }

        public NameDetails buildFromLine(String line)
        {
            if (!canBuildFromLine(line))
            {
                throw new InvalidOperationException("Cannot handle line");
            }
		            
    		line = line.Trim();				
		    String[] parts = line.Split(" ");

            debugLines.Add("parts = " + parts.Length);
		
		
		    String firstName = parts[0];
		    String lastName = parts[parts.Length -1];
		
		    // List<String> others = new List<String>();
            List<String> others = null;

		    if (parts.Length > 2)
		    {
			    others = new List<String>();
						
			    for (int i = 1; i < parts.Length -1; i++)
			    {
				    String part = parts[i];
                    debugLines.Add("part = " + part);
				
				    if (!String.IsNullOrEmpty(part))
				    {				
                        debugLines.Add("adding = " + part);
    					others.Add(part);
				    }
			    }
                debugLines.Add("others.size=" + others.Count);
		    }

            
		
		    NameDetails details;
		
		    if (others != null)
		    {
                debugLines.Add("3 args");
			    details = new NameDetails(lastName, firstName, others);
		    }
		    else
		    {
                debugLines.Add("2 args");
			    details = new NameDetails(lastName, firstName);
		    }
		
		    return details;
        }

        public String generateLine(NameDetails name)
        {
            String line = name.FirstName + " ";
			
            if (name.OtherNames != null)
		    {
			    foreach (String other in name.OtherNames)
			    {
				    line = line + other + " "; 
			    }				
		    }
								
		    line = line + name.LastName;
		
		    return line;
        }

        public NameDetailsLineBuilder()
        {            
            debugLines = new List<String>();
        }
    }

    public class NameDetailsComparator : IComparer<NameDetails>
    {
        public int Compare(NameDetails x, NameDetails y)
    {
        if (object.ReferenceEquals(x, y))
        {
            return 0;
        }
        
        if (x == null)
        {
            return -1;
        }
 
        if (y == null)
        {
            return 1;
        }
 
        int ret = String.Compare(x.LastName, y.LastName);

        if (ret == 0)
        {
            ret = String.Compare(x.FirstName, y.FirstName);
        }

        return ret;
        // return ret != 0 ? ret : x.age.CompareTo(y.age);
    }
    }

    public class NameSortProcess
    {
        public List<NameDetails> Sort(List<NameDetails> items)
        {
            List<NameDetails> sortedList = new List<NameDetails>(items);
            
            // IEnumerable<NameDetails> query = items.
            // OrderBy(pet => pet.Age);

            NameDetailsComparator comparator = new NameDetailsComparator();

            sortedList.Sort(comparator);

            return sortedList;
        }
    }

    public interface INameSorterLinesProcess
    {
        public List<NameDetails> ParseFromText(List<String> lines);
	    public List<String> GenerateLines(List<NameDetails> details);
    }

    public class NameSorterLinesProcess : INameSorterLinesProcess
    {
        private readonly  NameDetailsLineBuilder LineBuilder;

        public  NameSorterLinesProcess(NameDetailsLineBuilder lineBuilder)
        {
            LineBuilder = lineBuilder;
        }

        public List<NameDetails> ParseFromText(List<String> lines)
        {
            List<NameDetails> names = new List<NameDetails>();
		
    		foreach (String line in lines)
            {
                if (!String.IsNullOrEmpty(line))
                {
                    NameDetails details = LineBuilder.buildFromLine(line);
                    
                    names.Add(details);				
                }                
		    }
		
		
		    return names;
        }

	    public List<String> GenerateLines(List<NameDetails> details)
        {
            List<String> lines = new List<String>();
		
		    foreach (NameDetails name in details)
            {
                String line = LineBuilder.generateLine(name);
                
                lines.Add(line);
            }
            
            
            return lines;
        }        
    }

    public interface INameSorterFileUtils
    {
        public Boolean checkFileNameExists(String fileName);
	    public List<NameDetails> loadNamesFromFile(String fileName);
	    public void saveNamesToFile(String fileName, List<NameDetails> names);
    }

    public class NameSorterFileUtils : INameSorterFileUtils
    {
        private readonly NameSorterLinesProcess LinesProcess;
        public NameSorterFileUtils(NameSorterLinesProcess lineProcess)
        {
            LinesProcess = lineProcess;
        }
        public Boolean checkFileNameExists(String fileName)
        {
            Boolean exists = File.Exists(fileName);

            return exists;
        }
	    public List<NameDetails> loadNamesFromFile(String fileName)
        {
            string[] LinesAsArray = File.ReadAllLines(fileName); 

            List<String> Lines = new List<String>();
            Lines.AddRange(LinesAsArray);

            List<NameDetails> names = LinesProcess.ParseFromText(Lines);

            return names;
        }
	    public void saveNamesToFile(String fileName, List<NameDetails> names)
        {

            List<String> lines = LinesProcess.GenerateLines(names);

            using (StreamWriter outputFile = new StreamWriter(fileName))
            {
                foreach (string line in lines)
                {
                    outputFile.WriteLine(line);
                }
            }
        }
    }


    public interface INameSorterScreenUtils
    {
        public void writeToScreen(List<NameDetails> names);
    }

    public  class NameSorterScreenUtils : INameSorterScreenUtils
    {

        private readonly  NameDetailsLineBuilder LineBuilder;

        public  NameSorterScreenUtils(NameDetailsLineBuilder lineBuidler)
        {
            LineBuilder = lineBuidler;
        }

        public void writeToScreen(List<NameDetails> names)
        {
            foreach (NameDetails details in names)
            {
                String line = LineBuilder.generateLine(details);

                Console.WriteLine(line);
            }
        }
    }

    public interface IExecutionState
    {
        public void recordInterestingEvent(String line);
	    public void dumpHistory();
    }

    public class ExecutionState : IExecutionState
    {

        private readonly List<String> events;

        public ExecutionState()
        {
            events = new List<String>();
        }

        public void recordInterestingEvent(String line)
        {

        }
	    public void dumpHistory()
        {

        }
    }

    public class NameSorterCommand
    {
        private readonly INameSorterFileUtils FileUtils;
        private readonly INameSorterScreenUtils ScreenUtils;

        private readonly IExecutionState ExecutionState;

        public NameSorterCommand(INameSorterFileUtils fileUtils, INameSorterScreenUtils screenUtils, IExecutionState state)
        {
            FileUtils = fileUtils;
            ScreenUtils = screenUtils;
            ExecutionState = state;
        }

        public void run(String inputFileName, String targetFileName)
        {
            ExecutionState.recordInterestingEvent("run started");
		    ExecutionState.recordInterestingEvent("inputFileName:" + inputFileName);
		    ExecutionState.recordInterestingEvent("targetFileName:" + targetFileName);
		
		    if (FileUtils.checkFileNameExists(inputFileName) == false)
		    {
			    throw new Exception("Filename does not exists " + inputFileName);
		    }
		
		    ExecutionState.recordInterestingEvent("starting loading from file");
		
		    List<NameDetails> names = FileUtils.loadNamesFromFile(inputFileName);
		
		    ExecutionState.recordInterestingEvent("done loading from file");
		
            NameSortProcess SortProcess = new NameSortProcess();

		    List<NameDetails> sortedNames = SortProcess.Sort(names);
		
		    ExecutionState.recordInterestingEvent("writing to file");
		    ExecutionState.recordInterestingEvent("targetFileName:" + targetFileName);
		    ExecutionState.recordInterestingEvent("names:" + sortedNames.Count);
		
		    FileUtils.saveNamesToFile(targetFileName, sortedNames);
		
		    ScreenUtils.writeToScreen(sortedNames);
		
		    ExecutionState.recordInterestingEvent("run done");
        }

    }

}