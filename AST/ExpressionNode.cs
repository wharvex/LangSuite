using Shared;

namespace AST;

public abstract class ExpressionNode : INode
{
    private IType? _type;
    public IType Type
    {
        get => _type ?? IType.Default;
        set => _type = value;
    }

    public INode Walk(IVisitor v) => this;
}
