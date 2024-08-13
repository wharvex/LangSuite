namespace Shared;

public interface IVisitor
{
    INode Visit(INode node) => node;

    INode Final(INode node) => node;
}