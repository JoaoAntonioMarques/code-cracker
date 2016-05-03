using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace CodeCracker.CSharp.Refactoring
{

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddMissingParameterCodeFixProvider)), Shared]
    public class AddMissingParameterCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create("CS1501");

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var parent = root.FindToken(diagnosticSpan.Start).Parent;

            var method = root.DescendantNodes().OfType<MethodDeclarationSyntax>().Single(m => m.Identifier.Value.ToString() == "B");

            //method.AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("a")).WithType(SyntaxFactory.ParseTypeName("int")));

            //var variableUnused = parent.AncestorsAndSelf().OfType<CatchDeclarationSyntax>().First();
            context.RegisterCodeFix(CodeAction.Create($"Add missing parameter'", c => RemoveVariableAsync(context.Document, method, c), nameof(AddMissingParameterCodeFixProvider)), diagnostic);
        }


        private async static Task<Document> RemoveVariableAsync(Document document, MethodDeclarationSyntax method, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            
            var newMethod = method.AddParameterListParameters(SyntaxFactory.Parameter(SyntaxFactory.Identifier("a")).WithType(SyntaxFactory.ParseTypeName("int")));

            var newRoot = root.ReplaceNode(method, newMethod);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
