using Gamma.Parsing.Javascript.Syntax;

namespace Gamma.Parsing.Javascript.Syntax;


    public class AstVisitor
    {
        public virtual void Visit(AstNode node)
        {
            // Default implementation does nothing
            switch (node)
            {
                case ProgramNode programNode:
                    Visit(programNode);
                    break;
                case BlockStatementNode blockStatementNode:
                    Visit(blockStatementNode);
                    break;
                case VariableDeclarationNode variableDeclarationNode:
                    Visit(variableDeclarationNode);
                    break;
                case FunctionDeclarationNode functionDeclarationNode:
                    Visit(functionDeclarationNode);
                    break;
                case IdentifierNode identifierNode:
                    Visit(identifierNode);
                    break;
                case LiteralNode literalNode:
                    Visit(literalNode);
                    break;
                case BinaryExpressionNode binaryExpressionNode:
                    Visit(binaryExpressionNode);
                    break;
                case IfStatementNode ifStatementNode:
                    Visit(ifStatementNode);
                    break;
                case FunctionCallNode functionCallNode:
                    Visit(functionCallNode);
                    break;
                case ForStatementNode forStatementNode:
                    Visit(forStatementNode);
                    break;
                case UnaryExpressionNode unaryExpressionNode:
                    Visit(unaryExpressionNode);
                    break;
                case FunctionReturn functionReturn:
                    Visit(functionReturn);
                    break;
            }
        }

        public virtual void Visit(ProgramNode node)
        {
            foreach (var statement in node.Body)
            {
                Visit(statement);
            }
        }

        public virtual void Visit(BlockStatementNode node)
        {
            foreach (var statement in node.Body)
            {
                Visit(statement);
            }
        }

        public virtual void Visit(VariableDeclarationNode node)
        {
            foreach (var declaration in node.Declarations)
            {
                Visit(declaration);
            }
        }

        public virtual void Visit(FunctionDeclarationNode node)
        {
            foreach (var param in node.Parameters)
            {
                Visit(param);
            }
            Visit(node.Body);
        }

        public virtual void Visit(IdentifierNode node)
        {
            
        }

        public virtual void Visit(LiteralNode node)
        {
            
        }

        public virtual void Visit(BinaryExpressionNode node)
        {
            Visit(node.Left);
            Visit(node.Right);
        }

        public virtual void Visit(IfStatementNode node)
        {
            Visit(node.Test);
            Visit(node.Consequent);
            Visit(node.Alternate);
        }

        public virtual void Visit(FunctionCallNode node)
        {
            Visit(node.Identifier);
            foreach (var argument in node.Arguments)
            {
                Visit(argument);
            }
        }

        public virtual void Visit(ForStatementNode node)
        {
            Visit(node.Init);
            Visit(node.Test);
            Visit(node.Update);
            Visit(node.Body);
        }

        public virtual void Visit(UnaryExpressionNode node)
        {
            Visit(node.Operand);
        }

        public virtual void Visit(FunctionReturn node)
        {
            Visit(node.Expression);
        }
    }
