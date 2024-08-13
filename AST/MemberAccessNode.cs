using Shank.ExprVisitors;

namespace AST;

public class MemberAccessNode(string name) : ASTNode
{
    public string Name { get; init; } = name;

    public override void Accept(Visitor v)
    {
        throw new NotImplementedException();
    }

    public override ASTNode? Walk(SAVisitor v)
    {
        throw new NotImplementedException();
    }
}
