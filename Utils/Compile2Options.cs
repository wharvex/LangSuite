namespace Utils;

[Verb("Compile2", isDefault: false, HelpText = " ... ")]
public class Compile2Options
{ 
    [Option('i', HelpText = "Path (rel/abs) to the input folder", Default = ".")]
    public string InFolder { get; set; } = ".";

    [Option('o', HelpText = "Path (rel/abs) to the output file", Default = "a")]
    public string OutFile { get; set; } = "a";

    [Option('t', HelpText = "Output file type (accepted values: ir, obj, asm, exe)", Default = "ir")]
    public string OutType { get; set; } = "ir";

    [Option('u', HelpText = "Unit test flag", Default = false)]
    public bool UnitTest { get; set; }

    [Option('v', "vuop-test", HelpText = "Vuop flag", Default = false)]
    public bool VuOpTest { get; set; }
}