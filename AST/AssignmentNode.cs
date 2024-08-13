namespace AST;

public class AssignmentNode : StatementNode
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

    public override object[] returnStatementTokens()
    {
        object[] arr = { "", Target.Name, Expression.ToString() };

        return arr;
    }

    public override void Accept(Visitor v) => v.Visit(this);

    public override string ToString()
    {
        return $"{Target} assigned as {Expression}";
    }

    public override ASTNode Walk(WalkCompliantVisitor v)
    {
        var ret = v.Visit(this, out var shortCircuit);
        if (shortCircuit)
        {
            return ret;
        }

        if (v is ExpressionTypingVisitor etv && etv.GetVuopTestFlag())
        {
            NewTarget = (VariableUsageNodeTemp)NewTarget.Walk(etv);
        }
        else
        {
            Target = (VariableUsagePlainNode)Target.Walk(v);
        }

        Expression = (ExpressionNode)Expression.Walk(v);

        return v.Final(this);
    }

    public override ASTNode? Walk(SAVisitor v)
    {
        var temp = v.Visit(this);
        if (temp != null)
            return temp;

        Target = (VariableUsagePlainNode)(Target.Walk(v) ?? Target);

        Expression = (ExpressionNode)(Expression.Walk(v) ?? Expression);

        return v.PostWalk(this);
    }
}
