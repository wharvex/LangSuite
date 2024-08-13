using Shank.ExprVisitors;

namespace AST;

public class FloatNode : ExpressionNode
{
    public FloatNode(float value)
    {
        Value = value;
    }

    public float Value { get; set; }

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
