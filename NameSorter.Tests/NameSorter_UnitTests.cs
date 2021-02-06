using System;
using System.Collections.Generic;

using NameSorter;
using Xunit;
using Xunit.Abstractions;


namespace NameSorter.UnitTests
{


    public class ExecutionStateMock : IExecutionState
    {
        public void recordInterestingEvent(String line)
        {            
        }
	    public void dumpHistory()
        {            
        }
    }

    class NameSorterFileUtilsMock : INameSorterFileUtils
    {

        private NameSorterLinesProcess Process; 
	    private String TestFileName;
	    private List<String> Lines;
	    public List<String> LastLinesWritten { get; set; }
        public NameSorterFileUtilsMock(NameSorterLinesProcess process, String testFileName, List<String> lines)
        {
            Process = process;
		    TestFileName = testFileName;
		    Lines = lines;
		
		    LastLinesWritten = null;
        }

        public Boolean checkFileNameExists(String fileName)
        {
            return true;
        }

	    public List<NameDetails> loadNamesFromFile(String fileName)
        {
            List<NameDetails> names = Process.ParseFromText(Lines);
		
		    return names;
        }
	    public void saveNamesToFile(String fileName, List<NameDetails> names)
        {
            LastLinesWritten = Process.GenerateLines(names);
        }
    }

    public class PrimeService_IsPrimeShould
    {
        private readonly NameDetailsLineBuilder nameDetailsLineBuilder;
        private readonly NameSortProcess nameSortProcess;
         
        private readonly ITestOutputHelper consoleOutput;

        public PrimeService_IsPrimeShould(ITestOutputHelper output)
        {
            nameDetailsLineBuilder = new NameDetailsLineBuilder();
            nameSortProcess = new NameSortProcess();

            consoleOutput = output;
        }

        

        [Fact]
        public void ValidName()
        {
            NameDetails person1 = new NameDetails("last", "first");

            Assert.Equal("first", person1.FirstName);
            Assert.Equal("last", person1.LastName);

        }

        [Fact]
        public void InValidPersonFomLine()
        {
		    String line = "John ";
            
            Boolean canBuild = nameDetailsLineBuilder.canBuildFromLine(line);
            Assert.False(canBuild);
        }

        [Fact]
        public void ValidPerdsonFomLine()
        {
		    String line = "John Snow";
            
            Boolean canBuild = nameDetailsLineBuilder.canBuildFromLine(line);
            Assert.True(canBuild);

		    NameDetails details = nameDetailsLineBuilder.buildFromLine(line);
		
		    Assert.Equal("John", details.FirstName);
		    Assert.Equal("Snow", details.LastName);
        }

        [Fact]
	    public void ValidPersonFromLineWithMiddlename()
	    {
		    String line = "John Peter Roger Snow";
		
    		NameDetails details = nameDetailsLineBuilder.buildFromLine(line);

            Assert.Equal("John", details.FirstName);
            Assert.Equal("Snow", details.LastName);

            List<String> others = details.OtherNames;

            /* NameDetailsLineBuilder ndb = (NameDetailsLineBuilder) nameDetailsLineBuilder;
            foreach (String debug in ndb.debugLines)
            {
                consoleOutput.WriteLine(debug);
            } */

            Assert.Equal(2, others.Count);
		
            Assert.Equal("Peter", others[0]);
            Assert.Equal("Roger", others[1]);

    	}

        [Fact]
        public void SortNamesWhenNoNamesProvided() 
        {		
            List<NameDetails> namesToSort = new List<NameDetails>();
                                                
            List<NameDetails> sortedList = nameSortProcess.Sort(namesToSort);		

            Assert.Empty(sortedList);
        }

        [Fact]
        public void SortSingleName() 
	    {		
		    NameDetails johnSmith = new NameDetails("Smith", "John");
		
		    List<NameDetails> namesToSort = new List<NameDetails>();
            namesToSort.Add(johnSmith);
		            
		    List<NameDetails> sortedList = nameSortProcess.Sort(namesToSort);		
		
		    Assert.NotEmpty(sortedList);
    	}

        [Fact]
        public void SortMuldipleNames() 
	    {		
		    NameDetails adamSmith = new NameDetails("Smith", "Adam");
		    NameDetails alexSmith = new NameDetails("Smith", "Alex");
		    NameDetails johnSmith = new NameDetails("Smith", "John");
		    NameDetails johnBrown = new NameDetails("Brown", "John");

		
		    List<NameDetails> namesToSort = new List<NameDetails>();
            namesToSort.Add(adamSmith);
            namesToSort.Add(alexSmith);
            namesToSort.Add(johnSmith);
            namesToSort.Add(johnBrown);
		
            
		    List<NameDetails> sortedList = nameSortProcess.Sort(namesToSort);		
		
		    Assert.NotEmpty(sortedList);
            Assert.Equal(4, sortedList.Count);

            NameDetails first = sortedList[0];
            Assert.Equal("John", first.FirstName);
            Assert.Equal("Brown", first.LastName);


            NameDetails second = sortedList[1];
            Assert.Equal("Adam", second.FirstName);
            Assert.Equal("Smith", second.LastName);

            NameDetails third = sortedList[2];
            Assert.Equal("Alex", third.FirstName);
            Assert.Equal("Smith", third.LastName);

            NameDetails fourth = sortedList[3];
            Assert.Equal("John", fourth.FirstName);
            Assert.Equal("Smith", fourth.LastName);
    	}


        [Fact]
	    public void RunEndToEndProcess1() 
	    {		
		
    		String expectedLine1 = "Mad as a hatter Max";
		    String expectedLine2 = "Modonna Smith"; 
		    String expectedLine3 = "John Snow";

            List<String> fileContent = new List<String>();
            fileContent.Add(expectedLine3);
            fileContent.Add(expectedLine1);
            fileContent.Add(expectedLine2);

            NameSorterLinesProcess process = new NameSorterLinesProcess(nameDetailsLineBuilder);				    		
		    NameSorterFileUtilsMock fileUtils = new NameSorterFileUtilsMock(process, "test1.txt", fileContent);		
            NameSorterScreenUtils screenUtils = new NameSorterScreenUtils(nameDetailsLineBuilder);
            ExecutionStateMock stateMock = new ExecutionStateMock();
		
		    NameSorterCommand command = new NameSorterCommand(fileUtils, screenUtils, stateMock);
		
		    command.run("test1.txt", "test1.done.txt");
		
		    List<String> linesWritten = fileUtils.LastLinesWritten;
		
		    Assert.Equal(3, linesWritten.Count);
		
		    String line1 = linesWritten[0];
		    Assert.Equal(expectedLine1, line1);

            String line2 = linesWritten[1];
		    Assert.Equal(expectedLine2, line2);

            String line3 = linesWritten[2];
		    Assert.Equal(expectedLine3, line3);
		
	    }

        
    }
}
