using System;

namespace NameSorter
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("Starting");

            if (args.Length != 1)
            {
                Console.WriteLine("Please enter the filename to process");
                return -1;
            }
            else
            {
                Console.WriteLine(args[0]);
                String FileName = args[0];

                ExecutionState ExecutionState = new ExecutionState();

                NameSorterProcess Process = new NameSorterProcess(FileName, ExecutionState);

                if (Process.CanRun())
                {
                    Process.Run();
                    ExecutionState.dumpHistory();            

                    return 0;
                }
                else
                {
                    Console.WriteLine("Filename does not exists");

                    return -1;
                }
            }

        }
    }

    class NameSorterProcess
    {
        private readonly String FileName;
        private readonly NameDetailsLineBuilder NameDetailsLineBuilder;
        private readonly NameSortProcess NameSortProcess;

        private readonly NameSorterLinesProcess Process;
		private readonly NameSorterFileUtils FileUtils;
        private readonly IExecutionState ExecutionState;

        public NameSorterProcess(String fileName, IExecutionState executionState)
        {
            FileName = fileName;

            NameDetailsLineBuilder = new NameDetailsLineBuilder();
            NameSortProcess = new NameSortProcess();

            Process = new NameSorterLinesProcess(NameDetailsLineBuilder);				    		
		    FileUtils = new NameSorterFileUtils(Process);

            ExecutionState = executionState;
        }

        public Boolean CanRun()
        {
            if (FileUtils.checkFileNameExists(FileName))
            {
                return true;
            }
            return false;
        }


        public void Run()
        {
            if (!CanRun())
            {
                throw new ArgumentException("Cannot run with that file name");
            }

            NameSorterScreenUtils screenUtils = new NameSorterScreenUtils(NameDetailsLineBuilder);            
            
            NameSorterCommand command = new NameSorterCommand(FileUtils, screenUtils, ExecutionState);
            
            command.run(FileName, "sorted-names-list.txt");            
        }

    }

}
