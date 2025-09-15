
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CSharp.RuntimeBinder;
using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Security;
using System.Text.RegularExpressions;

namespace FunctionX;

/// <summary>
/// Excel의 Formulas 기능을 모방한 구현입니다.
/// Fx는 parameters에 의한 변수의 매핑을 지원합니다.
/// </summary>
public static partial class Fx
{
    private static readonly ConcurrentDictionary<string, Script<object>> _compiledScriptCache = new();
    private static readonly ConcurrentDictionary<string, ScriptOptions> _optionsCache = new();

    /// <summary>
    /// Clears the compilation cache. Use when memory optimization is needed.
    /// </summary>
    public static void ClearCache()
    {
        _compiledScriptCache.Clear();
        _optionsCache.Clear();
        GC.Collect(); // Force garbage collection after clearing cache
    }

    /// <summary>
    /// Gets the current cache statistics for monitoring performance
    /// </summary>
    public static (int CompiledScripts, int OptionsCache) GetCacheStatistics()
    {
        return (_compiledScriptCache.Count, _optionsCache.Count);
    }

    /// <summary>
    /// Evaluates a mathematical or logical expression asynchronously with Excel-like function support.
    /// </summary>
    /// <param name="expression">The expression to evaluate (e.g., "SUM(1,2,3)", "IF(@value > 10, 'High', 'Low')")</param>
    /// <param name="parameters">Optional dictionary of parameters that can be referenced in the expression using @paramName</param>
    /// <param name="customFuncType">Optional type containing custom functions to make available in the expression</param>
    /// <returns>The result of the expression evaluation</returns>
    /// <exception cref="FxUnsafeExpressionException">Thrown when expression contains potentially unsafe code</exception>
    /// <exception cref="FxCompilationErrorException">Thrown when expression cannot be compiled</exception>
    /// <exception cref="FxValueException">Thrown when expression contains invalid values</exception>
    /// <example>
    /// <code>
    /// // Basic math
    /// var result = await Fx.EvaluateAsync("1 + 2 * 3");
    ///
    /// // Using parameters
    /// var parameters = new Dictionary&lt;string, object?&gt; { {"value", 10} };
    /// var result = await Fx.EvaluateAsync("@value * 2", parameters);
    ///
    /// // Using Excel functions
    /// var data = new object[] {1, 2, 3, 4, 5};
    /// var parameters = new Dictionary&lt;string, object?&gt; { {"numbers", data} };
    /// var average = await Fx.EvaluateAsync("AVERAGE(@numbers)", parameters);
    /// </code>
    /// </example>
    public static async Task<object?> EvaluateAsync(
        string expression,
        IDictionary<string, object?>? parameters = null,
        Type? customFuncType = null)
    {
        try
        {
            CheckSafeExpression(expression);

            parameters ??= new Dictionary<string, object?>();
            var functions = new FxFunctions(parameters);

            var script = BuildCsScript(expression, parameters);
#if DEBUG
            Debug.WriteLine($"Expression: {script}");
#endif

            // Create cache key for options
            var optionsKey = customFuncType?.FullName ?? "default";
            var options = _optionsCache.GetOrAdd(optionsKey, _ =>
            {
                var opts = ScriptOptions.Default
                    .WithImports("System", "System.Linq")
                    .AddReferences(Assembly.Load("System.Core"));

                if (customFuncType != null)
                {
                    opts = opts
                        .AddReferences(customFuncType.Assembly)
                        .WithImports(customFuncType.Namespace);
                }

                return opts;
            });

            // Use cached compilation or create new one
            var compiledScript = _compiledScriptCache.GetOrAdd(script, scriptText =>
                CSharpScript.Create<object>(scriptText, options, globalsType: typeof(FxFunctions)));

            var result = await compiledScript.RunAsync(functions);
            return result.ReturnValue;
        }
        catch (CompilationErrorException ex)
        {
            throw new FxCompilationErrorException(ex.Message);
        }
        catch (ArgumentException ex)
        {
            throw new FxValueException(ex.Message);
        }
        catch (SecurityException ex)
        {
            throw new FxUnsafeExpressionException(ex.Message); // 보안 관련 예외
        }
        catch (InvalidOperationException ex)
        {
            throw new FxExpressionException("Invalid operation: " + ex.Message); // 잘못된 연산 예외
        }
        catch (RuntimeBinderException ex)
        {
            throw new FxExpressionException("Runtime binding error: " + ex.Message); // 바인딩 오류
        }
        catch (FxException)
        {
            throw; // FxException 유형의 예외는 재발생
        }
        catch (Exception ex)
        {
            throw new FxExpressionException("Unexpected error: " + ex.Message); // 기타 예외
        }
    }

    private static bool CheckSafeExpression(string expression)
    {
        // Enhanced security validation with regex patterns
        var forbiddenPatterns = new[]
        {
            @"\bimport\b",
            @"\busing\s+System\.IO\b",
            @"\bProcess\b",
            @"\bAssembly\b",
            @"\bFile\b",
            @"\bDirectory\b",
            @"\bThread\b",
            @"\bTask\.Run\b",
            @"\bEnvironment\b",
            @"\bReflection\b",
            @"\bDllImport\b",
            @"\bConsole\b",
            @"\bWindow\b",
            @"\bRegistry\b",
            @"\bnew\s+\w*Stream\b",
            @"\bnew\s+\w*Reader\b",
            @"\bnew\s+\w*Writer\b",
            @"\bActivator\b",
            @"\bAppDomain\b",
            @"\bGC\.Collect\b"
        };

        // Check for forbidden patterns with word boundaries
        foreach (var pattern in forbiddenPatterns)
        {
            if (Regex.IsMatch(expression, pattern, RegexOptions.IgnoreCase))
            {
                throw new FxUnsafeExpressionException($"Forbidden pattern detected: {pattern}");
            }
        }

        // Enhanced dynamic invoke pattern detection
        var dangerousPatterns = new[]
        {
            @"\bGetType\s*\(\s*\)",
            @"\bGetMethod\s*\(",
            @"\bGetProperty\s*\(",
            @"\bInvokeMember\s*\(",
            @"\bInvoke\s*\(",
            @"\.CreateInstance\s*\(",
            @"Type\.GetType\s*\(",
            @"typeof\s*\(\s*\w+\s*\)\s*\.\s*GetMethod",
            @"System\.Reflection",
            @"this\.GetType\s*\(\s*\)"
        };

        foreach (var pattern in dangerousPatterns)
        {
            if (Regex.IsMatch(expression, pattern, RegexOptions.IgnoreCase))
            {
                throw new FxUnsafeExpressionException($"Dangerous reflection pattern: {pattern}");
            }
        }

        // Check for potential code injection patterns
        var injectionPatterns = new[]
        {
            @"[;{}]",                    // Statement separators
            @"\bclass\s+\w+",            // Class definitions
            @"\bnamespace\s+\w+",        // Namespace definitions
            @"\bwhile\s*\(\s*true\s*\)", // Infinite loops
            @"\bfor\s*\(\s*;\s*;\s*\)",  // Infinite for loops
            @"#\s*(region|endregion|if|else|endif)", // Preprocessor directives
        };

        foreach (var pattern in injectionPatterns)
        {
            if (Regex.IsMatch(expression, pattern, RegexOptions.IgnoreCase))
            {
                throw new FxUnsafeExpressionException($"Code injection pattern detected: {pattern}");
            }
        }

        // Validate expression length to prevent DoS
        if (expression.Length > 10000)
        {
            throw new FxUnsafeExpressionException("Expression too long - potential DoS attack");
        }

        return true;
    }

    /// <summary>
    /// 실행가능한 C# 스크립트를 생성합니다.
    /// </summary>
    private static string BuildCsScript(string expression, IDictionary<string, object?> parameters)
    {
        var script = TransformToTryCatchBlocks(expression);
#if DEBUG
        Debug.WriteLine($"[Logs]");
        Debug.WriteLine($"input: {expression}");
        Debug.WriteLine($"output: {script}");
#endif
        script = BuildCsExpression(script, parameters);
        return script;
    }

    // IFERROR 함수를 try-catch 블록으로 변환합니다.
    // 재귀적으로 모든 중첩된 IFERROR 함수도 실행가능한 try-catch 함수로 변환합니다.
    // 실행가능한 C# 스크립트를 생성하여 반환합니다.
    // 예)
    // input: "IFERROR(10 / 5, "ERROR")"
    // output: "try { return 10 / 5; } catch { return "ERROR"; }"
    // input2: "IFERROR(INT(@a) / INT(@b), "ERROR")
    // ouput2: "try { return INT(@a) / INT(@b); } catch { return "ERROR"; }"
    // input3: "IFERROR(IFERROR(INT(@a) / INT(@b), "ERROR"), "ON ERROR")"
    // ouput3: "try { return (Func<object>)(() => { try { return INT(@a) / INT(@b); } catch { return "ERROR"; } })(); } catch { return "ON ERROR"; }"
    public static string TransformToTryCatchBlocks(string input)
    {
        return TransformIFERROR(input, false);
    }

    private static string TransformIFERROR(string input, bool isNested)
    {
        var regex = new Regex(@"IFERROR\(((?:[^()]|(?<Open>\()|(?<-Open>\)))+)(?(Open)(?!)),\s*""(?<error>[^""]*)""\)");
        if (!regex.IsMatch(input))
        {
            return input;
        }

        return regex.Replace(input, match =>
        {
            var innerExpression = match.Groups[1].Value;
            var errorText = match.Groups["error"].Value;
            var transformedInnerExpression = TransformIFERROR(innerExpression, true);

            if (isNested)
            {
                return $"((Func<object>)(() => {{ try {{ return {transformedInnerExpression}; }} catch {{ return \"{errorText}\"; }} }}))()";
            }
            else
            {
                return $"try {{ return {transformedInnerExpression}; }} catch {{ return \"{errorText}\"; }}";
            }
        });
    }


    /// <summary>
    /// @변수를 적절한 Get함수로 치환합니다.
    /// GetItems()는 object[]를 반환합니다.
    /// GetItem()은 object?를 반환합니다.
    /// GetValues()는 double[]의 값을 반환합니다.
    /// GetValue()는 double의 값을 반환합니다.
    /// </summary>
    private static string BuildCsExpression(string expression, IDictionary<string, object?> parameters)
    {
        var stringLiterals = new List<string>();
        var modifiedExpression = Regex.Replace(expression, @"(""([^""\\]|\\.)*""|'([^'\\]|\\.)*')", match =>
        {
            var literal = match.Value;
            // 홑따옴표로 둘러싸인 문자열을 쌍따옴표로 변환합니다.
            if (literal.StartsWith("'") && literal.EndsWith("'"))
            {
                // 홑따옴표 내부의 홑따옴표 이스케이프 처리 (예: 'It\'s fine')
                var replaced = literal[1..^1].Replace("\\'", "'");
                // C# 스타일의 이스케이프로 변환합니다.
                replaced = replaced.Replace("'", "\\'");
                // 쌍따옴표로 감싸줍니다.
                literal = $"\"{replaced}\"";
            }
            stringLiterals.Add(literal);
            return $"$stringLiteral${stringLiterals.Count - 1}$";
        });

        modifiedExpression = TransformVariableReferences(modifiedExpression, parameters);

        modifiedExpression = Regex.Replace(modifiedExpression, @"\$stringLiteral\$(\d+)\$", match =>
        {
            int index = int.Parse(match.Groups[1].Value);
            return stringLiterals[index];
        });

        return modifiedExpression;
    }


    private static string TransformVariableReferences(string expression, IDictionary<string, object?> parameters)
    {
        var operationPattern = @"(@\w+)(\s*(==|!=|>|<|>=|<=|\+|\-|\*|\/|%|\^|&&|\|\||<<|>>|!)\s*@?\w+)+";

        expression = Regex.Replace(expression, operationPattern, match =>
        {
            return Regex.Replace(match.Value, @"\@\w+", m => ReplaceVariableWithGetValueOrValuesCall(m, parameters));
        });

        expression = Regex.Replace(expression, @"\@\w+", m => ReplaceVariableWithGetItemOrItemsCall(m, parameters));

        return expression;
    }

    private static string ReplaceVariableWithGetValueOrValuesCall(Match match, IDictionary<string, object?> parameters)
    {
        var variableName = match.Value[1..]; // '@' 제거
        if (parameters.TryGetValue(variableName, out object? value))
        {
            if (value == null)
            {
                return "null";
            }
            else if (value.GetType().IsArray)
            {
                return $"GetValues(\"{variableName}\")";
            }
            else
            {
                return $"GetValue(\"{variableName}\")";
            }
        }
        return match.Value;
    }

    private static string ReplaceVariableWithGetItemOrItemsCall(Match match, IDictionary<string, object?> parameters)
    {
        var variableName = match.Value[1..]; // '@' 제거
        if (parameters.TryGetValue(variableName, out object? value))
        {
            if (value == null)
            {
                return "null";
            }
            else if (value is IEnumerable && value is not string)
            {
                return $"GetItems(\"{variableName}\")";
            }
            else
            {
                return $"GetItem(\"{variableName}\")";
            }
        }
        return match.Value;
    }

}