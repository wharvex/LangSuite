namespace Utils;

[Verb("Compile", isDefault: false, HelpText = "invokes the shank LLVM compiler ")]
public class CompileOptions
{
    [Option(
        "use-default-settings",
        Default = false,
        HelpText = "uses the default settings in AppData/~ShankData"
    )]
    public bool DefaultSettings { get; set; }

    [Value(index: 0, MetaName = "inputFile", HelpText = "The Shank source file", Required = true)]
    public IEnumerable<string> InputFile { get; set; }

    [Option('o', "output", HelpText = "returns an output file", Default = "a")]
    public string OutFile { get; set; }

    [Option('c', "compile", HelpText = "compile to object file")]
    public bool CompileToObj { get; set; }

    public OptPasses OptLevel { get; set; }

    [Option(
        'O',
        "optimize",
        Default = "0",
        Required = false,
        HelpText = "Set optimization level.(0 being least 3 being most)"
    )]
    public string? OptimizationLevels
    {
        set
        {
            OptLevel = value switch
            {
                "0" => OptPasses.Level0,
                "1" => OptPasses.Level1,
                "2" => OptPasses.Level2,
                "3" => OptPasses.Level3,
                _ => OptPasses.Level3
            };
        }
    }

    [Option(
        "emit-llvm",
        HelpText = "writes LLVM IR to a file you provide with -o dir it saves to is /Shank-IR/ doesnt"
    )]
    public bool emitIR { get; set; }

    [Option(
        'a',
        "assembly",
        HelpText = "option to generate an assembly file. appears in /Shank-Assembly/ directory -target is how you add a specific one"
    )]
    public bool Assembly { get; set; }

    [Option("print-llvm", HelpText = "prints LLVM IR to the console")]
    public bool printIR { get; set; }

    [Option(
        'S',
        "compile-off",
        HelpText = "no exe or object file will be produced here but you may generate .s, .ll files",
        Default = false
    )]
    public bool CompileOff { get; set; }

    [Option(
        "linker",
        HelpText = "add whatever linker you feel if non specified it defaults to clang",
        Default = "clang"
    )]
    public string LinkerOption { get; set; }

    [Option('l', HelpText = "for linked files")]
    public IEnumerable<string> LinkedFiles { get; set; }

    [Option('u', "unit-test", HelpText = "Unit test options", Default = false)]
    public bool UnitTest { get; set; }

    [Option('L', "LinkPath", Default = "/", HelpText = "for a link path")]
    public string LinkedPath { get; set; }

    [Option(
        "target",
        Default = "generic",
        HelpText = "target cpu (run clang -print-supported-cpus to see list"
    )]
    public string TargetCPU { get; set; }

    [Option('v', "vuop-test", HelpText = "Variable Usage Operation Test", Default = false)]
    public bool VuOpTest { get; set; }
}