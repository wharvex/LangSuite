using Shared;

namespace AST;

public abstract class ASTNode : INode
{
    public string InheritsDirectlyFrom { get; init; }
    public int Line { get; init; }
    public string FileName { get; init; }

    protected ASTNode()
    {
        InheritsDirectlyFrom = GetType().BaseType?.Name ?? "None";
        Line = 1;
        FileName = "";
    }

    public abstract void Accept(Visitor v);

    public virtual ASTNode Walk(WalkCompliantVisitor v) => this;

    public abstract ASTNode? Walk(SAVisitor v);
}
