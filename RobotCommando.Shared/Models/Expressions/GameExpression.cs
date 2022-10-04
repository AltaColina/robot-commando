using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace RobotCommando.Models.Expressions;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public abstract class GameExpression<TContext, TDelegate> where TDelegate : Delegate
{
    private static readonly ScriptOptions CSOptions;
    private TDelegate? _expression;

    static GameExpression()
    {
        var asm = typeof(GameExpression<,>).Assembly;
        CSOptions = ScriptOptions.Default
            .AddReferences(asm)
            .AddImports("System.Linq")
            .AddImports(asm.ExportedTypes.Select(t => t.Namespace));
    }

    public string ExpressionText { get; init; } = null!;

    [JsonIgnore]
    public TDelegate Expression { get => _expression ??= Compile($"c => {ExpressionText}"); }

    private static TDelegate Compile(string expressionText) => CSharpScript.EvaluateAsync<TDelegate>(expressionText, CSOptions).ConfigureAwait(false).GetAwaiter().GetResult();

    private string GetDebuggerDisplay() => ExpressionText;

    public async Task CompileAsync()
    {
        try
        {
            _expression ??= await CSharpScript.EvaluateAsync<TDelegate>($"c => {ExpressionText}", CSOptions);
        }
        catch(Exception ex)
        {
            throw;
        }
    }
}
