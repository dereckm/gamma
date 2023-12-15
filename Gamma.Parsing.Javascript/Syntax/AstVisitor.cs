using Gamma.Parsing.Javascript.Syntax;

namespace Gamma.Parsing.Javascript.Syntax;


    public class AstVisitor
    {
        protected virtual bool ShouldVisit(AstNode node) 
        {
             return true; 
        }

        public virtual void Visit(AstNode node)
        {
            if (!ShouldVisit(node)) 
            {
                return;
            }

            // Default implementation does nothing
            switch (node)
            {
                case Program programNode:
                    VisitProgram(programNode);
                    break;
                case BlockStatement blockStatementNode:
                    VisitBlockStatement(blockStatementNode);
                    break;
                case VariableDeclaration variableDeclarationNode:
                    VistiVariableDeclaration(variableDeclarationNode);
                    break;
                case NamedFunctionDeclaration functionDeclarationNode:
                    VisitNamedFunctionDeclaration(functionDeclarationNode);
                    break;
                case Identifier identifierNode:
                    VisitIdentifier(identifierNode);
                    break;
                case Literal literalNode:
                    VisitLiteral(literalNode);
                    break;
                case BinaryExpression binaryExpressionNode:
                    VisitBinaryExpression(binaryExpressionNode);
                    break;
                case IfStatement ifStatementNode:
                    VisitIfStatement(ifStatementNode);
                    break;
                case FunctionCall functionCallNode:
                    VisitFunctionCall(functionCallNode);
                    break;
                case ForStatement forStatementNode:
                    VisitForStatement(forStatementNode);
                    break;
                case UnaryExpression unaryExpressionNode:
                    VisitUnaryExpression(unaryExpressionNode);
                    break;
                case FunctionReturn functionReturn:
                    VisitFunctionReturn(functionReturn);
                    break;
                case ArrayNode array:
                    VisitArray(array);
                    break;
                case IndexerCall indexerCallNode:
                    VisitIndexerCall(indexerCallNode);
                    break;
                case MemberExpression memberExpression:
                    VisitMemberExpression(memberExpression);
                    break;
                case AnonymousFunctionDeclaration anonymousFunction:
                    VisitAnonymousFunctionDeclaration(anonymousFunction);
                    break;
                case ForOfStatement forOfStatement:
                    VisitForOfStatement(forOfStatement);
                    break;
                case BreakStatement breakStatement:
                    VisitBreakStatement(breakStatement);
                    break;
            }
        }

        public virtual void VisitProgram(Program node)
        {
            foreach (var statement in node.Body)
            {
                Visit(statement);
            }
        }

        public virtual void VisitBlockStatement(BlockStatement node)
        {
            foreach (var statement in node.Body)
            {
                Visit(statement);
            }
        }

        public virtual void VistiVariableDeclaration(VariableDeclaration node)
        {
            foreach (var declaration in node.Declarations)
            {
                Visit(declaration);
            }
        }

        public virtual void VisitNamedFunctionDeclaration(NamedFunctionDeclaration node)
        {
            foreach (var param in node.Parameters)
            {
                Visit(param);
            }
            Visit(node.Body);
        }

        public virtual void VisitIdentifier(Identifier node)
        {
            
        }

        public virtual void VisitLiteral(Literal node)
        {
            
        }

        public virtual void VisitBinaryExpression(BinaryExpression node)
        {
            Visit(node.Left);
            Visit(node.Right);
        }

        public virtual void VisitIfStatement(IfStatement node)
        {
            Visit(node.Test);
            Visit(node.Consequent);
            Visit(node.Alternate);
        }

        public virtual void VisitFunctionCall(FunctionCall node)
        {
            VisitIdentifier(node.Identifier);
            foreach (var argument in node.Arguments)
            {
                Visit(argument);
            }
        }

        public virtual void VisitForStatement(ForStatement node)
        {
            Visit(node.Init);
            Visit(node.Test);
            Visit(node.Update);
            Visit(node.Body);
        }

        public virtual void VisitUnaryExpression(UnaryExpression node)
        {
            Visit(node.Operand);
        }

        public virtual void VisitFunctionReturn(FunctionReturn node)
        {
            Visit(node.Expression);
        }

        public virtual void VisitArray(ArrayNode node)
        {
            foreach(var item in node.Items)
            {
                Visit(item);
            }
        }

        public virtual void VisitIndexerCall(IndexerCall node)
        {
            Visit(node.Argument);
        }

        public virtual void VisitMemberExpression(MemberExpression node)
        {
            Visit(node.Object);
            Visit(node.Property);
        }

        public virtual void VisitAnonymousFunctionDeclaration(AnonymousFunctionDeclaration node)
        {
            foreach(var parameter in node.Parameters)
            {
                Visit(parameter);
            }
            Visit(node.Body);
        }

        public virtual void VisitForOfStatement(ForOfStatement node)
        {
            VistiVariableDeclaration(node.Left);
            Visit(node.Right);
            Visit(node.Body);
        }

        public virtual void VisitBreakStatement(BreakStatement node)
        {
            
        }
    }