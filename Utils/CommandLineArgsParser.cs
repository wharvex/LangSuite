namespace Utils;

public class CommandLineArgsParser
{
    public static void InvokeShank(string[] args)
    {
        ProgramNode program = new ProgramNode();
        CommandLine
            .Parser.Default.ParseArguments<
                CompileOptions,
                Settings,
                InterpretOptions,
                CompilePracticeOptions
            >(args)
            .WithParsed<Settings>(options => SealizeSettings(options))
            .WithParsed<CompileOptions>(options => RunCompiler(options, program))
            .WithParsed<InterpretOptions>(options => RunInterpreter(options, program))
            .WithParsed<CompilePracticeOptions>(options => RunCompilePractice(options, program))
            .WithNotParsed(
                errors =>
                    Console.WriteLine(
                        $"error bad input, consult the documentation or run"
                            + $"shank --help if you want a list of Commands"
                    )
            );
    }

    private static void RunCompiler(CompileOptions options, ProgramNode program)
    {
        if (options.DefaultSettings)
        {
            if (
                File.Exists(
                    Path.Combine(OutputHelper.DocPath, "~ShankData", "DefaultSettings.json")
                )
            )
            {
                Settings? s = JsonConvert.DeserializeObject<Settings>(
                    File.ReadAllText(
                        Path.Combine(OutputHelper.DocPath, "~ShankData", "DefaultSettings.json")
                    )
                );
                if (s.Setlinker != null)
                {
                    options.LinkerOption = s.Setlinker;
                }

                if (s.SetCPU != null)
                {
                    options.TargetCPU = s.SetCPU;
                }

                if (s.SetOpLevel != null)
                {
                    options.OptimizationLevels = s.SetOpLevel;
                }
            }
        }

        LLVMCodeGen a = new LLVMCodeGen();
        options.InputFile.ToList().ForEach(n => Console.WriteLine(n));

        var fakeInterpretOptions = new InterpretOptions()
        {
            VuOpTest = options.VuOpTest,
            UnitTest = false,
            InputFiles = []
        };
        options
            .InputFile.ToList()
            .ForEach(
                n =>
                    GetFiles(n) //multiple files
                        .ForEach(ip => ScanAndParse(ip, program, fakeInterpretOptions))
            );

        // TODO: ANY NEW THINGS YOU WANT TO ADD TO SEMANTIC ANALYSIS PLEASE ADD TO NEWRUNSEMANTICANALYSIS SO THAT WE CAN SWITCH OVER
        RunSemanticAnalysis(fakeInterpretOptions, program);
        // NewRunSemanticAnalysis(options, program);

        if (options.UnitTest)
        {
            Interpreter.ActiveInterpretOptions = fakeInterpretOptions;
            Interpreter.Modules = program.Modules;
            Interpreter.StartModule = program.GetStartModuleSafe();
            InterpretProgramWithTests();
        }

        // GetFiles(options.InputFile).ForEach(ip => ScanAndParse(ip, program));
        // if (options.UnitTest)
        //     It2();
        var monomorphization = new MonomorphizationVisitor(options.UnitTest);
        program.Accept(monomorphization);
        var monomorphizedProgram = monomorphization.ProgramNode;

        a.CodeGen(options, monomorphizedProgram);
    }

    private static void RunCompiler2(Compile2Options options, ProgramNode program)
    {
        var fakeInterpretOptions = new InterpretOptions()
        {
            VuOpTest = options.VuOpTest,
            UnitTest = options.UnitTest,
            InputFiles = []
        };
        GetFiles2(options.InFolder)
            .ForEach(
                n =>
                    GetFiles(n) //multiple files
                        .ForEach(ip => ScanAndParse(ip, program, fakeInterpretOptions))
            );

        // TODO: ANY NEW THINGS YOU WANT TO ADD TO SEMANTIC ANALYSIS PLEASE ADD TO NEWRUNSEMANTICANALYSIS SO THAT WE CAN SWITCH OVER
        RunSemanticAnalysis(fakeInterpretOptions, program);
        // NewRunSemanticAnalysis(options, program);

        if (options.UnitTest)
        {
            Interpreter.ActiveInterpretOptions = fakeInterpretOptions;
            Interpreter.Modules = program.Modules;
            Interpreter.StartModule = program.GetStartModuleSafe();
            InterpretProgramWithTests();
        }

        // GetFiles(options.InputFile).ForEach(ip => ScanAndParse(ip, program));
        // if (options.UnitTest)
        //     It2();
        var monomorphization = new MonomorphizationVisitor(options.UnitTest);
        program.Accept(monomorphization);
        var monomorphizedProgram = monomorphization.ProgramNode;

        LLVMCodeGen gen = new LLVMCodeGen();
        gen.CodeGen2(options, monomorphizedProgram);
    }

    private static void RunInterpreter(InterpretOptions options, ProgramNode program)
    {
        options
            .InputFiles.ToList()
            .ForEach(
                n =>
                    GetFiles(n) //multiple files
                        .ForEach(ip => ScanAndParse(ip, program, options))
            );
        // TODO: ANY NEW THINGS YOU WANT TO ADD TO SEMANTIC ANALYSIS PLEASE ADD TO NEWRUNSEMANTICANALYSIS SO THAT WE CAN SWITCH OVER
        RunSemanticAnalysis(options, program);
        // NewRunSemanticAnalysis(options, program);

        Interpreter.ActiveInterpretOptions = options;
        Interpreter.Modules = program.Modules;
        Interpreter.StartModule = program.GetStartModuleSafe();
        if (!options.UnitTest)
            InterpretProgram(program);
        else
            InterpretProgramWithTests();
    }

    // extract semantic analysis into one function so that both compiler and interpreter do the same thing
    // this is copied from interpreter as it seems to have had the most up to date semantic analysis at the time of doing this
    // TODO: ANY NEW THINGS YOU WANT TO ADD TO SEMANTIC ANALYSIS PLEASE ADD TO NEWRUNSEMANTICANALYSIS SO THAT WE CAN SWITCH OVER
    public static void RunSemanticAnalysis(InterpretOptions options, ProgramNode program)
    {
        program.SetStartModule();
        SemanticAnalysis.ActiveInterpretOptions = options;
        BuiltInFunctions.Register(program.GetStartModuleSafe().Functions);

        SAVisitor.ActiveInterpretOptions = options;
        program.Walk(new ImportVisitor());
        SemanticAnalysis.AreExportsDone = true;
        SemanticAnalysis.AreImportsDone = true;

        // This resolves unknown types.
        //program.Walk(new RecordVisitor());


        // program.Walk(new TestVisitor());

        // Create WalkCompliantVisitors.
        var nuVis = new NestedUnknownTypesResolvingVisitor(SemanticAnalysis.ResolveType);
        program.Walk(new UnknownTypesVisitor());
        SemanticAnalysis.AreSimpleUnknownTypesDone = true;
        var vgVis = new VariablesGettingVisitor();
        var etVis = new ExpressionTypingVisitor(SemanticAnalysis.GetTypeOfExpression)
        {
            ActiveInterpretOptions = options
        };

        // Apply WalkCompliantVisitors.
        program.Walk(nuVis);
        SemanticAnalysis.AreNestedUnknownTypesDone = true;
        program.Walk(vgVis);
        program.Walk(etVis);

        //Run old SA.
        OutputHelper.DebugPrintJson(program, "pre_old_sa");
        SemanticAnalysis.CheckModules(program);
        OutputHelper.DebugPrintJson(program, "post_old_sa");

        NewSemanticAnalysis.Run(program);
    }
    private static int InterpretProgram(ProgramNode program)
    {
        Interpreter.InterpretFunction(
            program.GetStartModuleSafe().GetStartFunctionSafe(),
            [],
            program.GetStartModuleSafe()
        );
        return 1;
    }

    private static void ScanAndParse(
        string inPath,
        ProgramNode program,
        InterpretOptions? options = null
    )
    {
        List<Token> tokens = [];
        var lexer = new Lexer();

        // Read the file and turn it into tokens.
        var lines = File.ReadAllLines(inPath);
        tokens.AddRange(lexer.Lex(lines));

        var parser = new Shank.Parser(tokens, options);

        // Parse the tokens and turn them into an AST.
        while (tokens.Count > 0)
        {
            var module = parser.Module();
            program.AddToModules(module);
        }
    }

    private static void InterpretProgramWithTests()
    {
        LinkedList<TestResult> UnitTestResults = new LinkedList<TestResult>();
        Interpreter
            .GetModulesAsList()
            .ForEach(module =>
            {
                module
                    .GetFunctionsAsList()
                    .ForEach(f => f.ApplyActionToTests(Interpreter.InterpretFunction, module));

                //wierdly doesnt work


                Console.WriteLine(
                    "[[ Tests from "
                        + module.Name
                        + " ]]\n"
                        + string.Join("\n", Program.UnitTestResults)
                );
            });
    }

    private static List<string> GetFiles(string dir)
    {
        if (Directory.Exists(dir))
        {
            return [..Directory.GetFiles(dir, "*.shank", SearchOption.AllDirectories)];
        }
        else if (File.Exists(dir))
        {
            return [dir];
        }

        throw new FileNotFoundException($"file or dir {dir} doesnt exist");
    }

    private static List<string> GetFiles2(string dir)
    {
        if (Directory.Exists(dir))
            return [..Directory.GetFiles(dir, "*.shank", SearchOption.AllDirectories)];

        throw new DirectoryNotFoundException("Directory at path `" + dir + "' not found.");
    }
}