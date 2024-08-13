namespace Shared;

public interface INode
{ 
    public INode Walk(IVisitor v) => this;
}