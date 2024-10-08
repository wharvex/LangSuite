using System.Text;
using Shared;

namespace AST;

public abstract class StatementNode : INode
{
    protected static string StatementListToString(List<StatementNode> statements)
    {
        var b = new StringBuilder();
        statements.ForEach(c => b.Append("\t" + c));
        return b.ToString();
    }

    public virtual object[] returnStatementTokens()
    {
        object[] arr = { };
        return arr;
    }

    public override void Accept(Visitor v) => throw new NotImplementedException();

    public override ASTNode? Walk(SAVisitor v)
    {
        throw new NotImplementedException();
    }
}
