using Shared;

namespace AST;

public class AssignmentNode : StatementNode, INode
{
    public AssignmentNode(VariableUsagePlainNode target, ExpressionNode expression)
    {
        Target = target;
        Expression = expression;
        NewTarget = new VariableUsagePlainNode("emptyNewTarget", "default");
    }

    public AssignmentNode(
        VariableUsageNodeTemp target,
        ExpressionNode expression,
        bool isVuopReroute
    )
    {
        NewTarget = target;
        Expression = expression;
        Target = new VariableUsagePlainNode("emptyOldTarget", "default");
    }

    public VariableUsagePlainNode Target { get; set; }

    public VariableUsageNodeTemp NewTarget { get; set; }

    public ExpressionNode Expression { get; set; }

    public override string ToString()
    {
        return $"{Target} assigned as {Expression}";
    }

    public INode Walk(IVisitor v)
    {
        var ret = (AssignmentNode)v.Visit(this);

        NewTarget = (VariableUsageNodeTemp)ret.NewTarget.Walk(v);

        Expression = (ExpressionNode)ret.Expression.Walk(v);

        return v.Final(ret);
    }
}
