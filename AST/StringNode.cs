using Shank.ExprVisitors;

namespace AST;

public class StringNode : ExpressionNode
{
    public StringNode(string value)
    {
        Value = value;
    }

    public string Value { get; set; }

    public override string ToString()
    {
        return $"{Value}";
    }

    public override void Accept(Visitor v) => v.Visit(this);

    public override ASTNode? Walk(SAVisitor v)
    {
        var temp = v.Visit(this);
        if (temp != null)
            return temp;

        return v.PostWalk(this);
    }
}
