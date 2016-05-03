using CodeCracker.CSharp.Refactoring;
using Microsoft.CodeAnalysis.CodeFixes;
using System.Threading.Tasks;
using Xunit;


namespace CodeCracker.Test.CSharp.Refactoring
{
    public class AddMissingParameterTest : CodeFixVerifier
    {
        protected override CodeFixProvider GetCodeFixProvider() => new AddMissingParameterCodeFixProvider();

        [Fact]
        public async Task ShouldAddMissingParameter()
        {
            const string source = @"class TypeName
                            {
                                public void A()
                                {
                                    B(5);
                                }

                                public void B()
                                {
                                }
                            }";

            const string codeFix = @"class TypeName
                            {
                                public void A()
                                {
                                    B(5);
                                }

                                public void B(int a)
                                {
                                }
                            }";

            await VerifyCSharpFixAsync(source, codeFix);
        }
    }
}
