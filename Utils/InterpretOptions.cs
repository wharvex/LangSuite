using CommandLine;

namespace Utils;

[Verb("Interpret", isDefault: false, HelpText = "invokes the shank interpreter")]
public class InterpretOptions
{ 
    [Value(index: 0, MetaName = "inputFile", HelpText = "The Shank source file", Required = true)]
    public IEnumerable<string> InputFiles { get; set; }

    [Option('u', "unit-test", HelpText = "Unit test options", Default = false)]
    public bool UnitTest { get; set; }

    [Option(
        'v',
        "vuop-test",
        HelpText = "Variable Usage Operation Test",
        Default = false
    )]
    public bool VuOpTest { get; set; }
}