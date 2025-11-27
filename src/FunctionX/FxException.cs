namespace FunctionX;

/// <summary>
/// Base class for all FunctionX exceptions that mirror Excel error types.
/// </summary>
/// <param name="message">The error message describing the exception.</param>
public abstract class FxException(string message) : Exception(message)
{
}

/// <summary>
/// Represents a division by zero error, mimicking Excel's #DIV/0! error.
/// </summary>
public class FxDivideByZeroException : FxException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FxDivideByZeroException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public FxDivideByZeroException(string message = "Division by zero error") : base(message)
    {
    }

    /// <inheritdoc/>
    public override string ToString() => "#DIV/0!";
}

/// <summary>
/// Represents a value error when invalid data types are used, mimicking Excel's #VALUE! error.
/// </summary>
public class FxValueException : FxException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FxValueException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public FxValueException(string message = "Invalid value or data type error") : base(message)
    {
    }

    /// <inheritdoc/>
    public override string ToString() => "#VALUE!";
}

/// <summary>
/// Represents a reference error when referencing invalid data, mimicking Excel's #REF! error.
/// </summary>
public class FxReferenceException : FxException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FxReferenceException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public FxReferenceException(string message = "Invalid reference error") : base(message)
    {
    }

    /// <inheritdoc/>
    public override string ToString() => "#REF!";
}

/// <summary>
/// Represents a name error when function or variable name is not recognized, mimicking Excel's #NAME? error.
/// </summary>
public class FxNameException : FxException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FxNameException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public FxNameException(string message = "Unrecognized function or variable name error") : base(message)
    {
    }

    /// <inheritdoc/>
    public override string ToString() => "#NAME?";
}

/// <summary>
/// Represents a numeric error when calculation results in invalid number, mimicking Excel's #NUM! error.
/// </summary>
public class FxNumException : FxException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FxNumException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public FxNumException(string message = "Invalid numeric calculation error") : base(message)
    {
    }

    /// <inheritdoc/>
    public override string ToString() => "#NUM!";
}

/// <summary>
/// Represents a not available error when value is not available, mimicking Excel's #N/A error.
/// </summary>
public class FxNAException : FxException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FxNAException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public FxNAException(string message = "Value not available error") : base(message)
    {
    }

    /// <inheritdoc/>
    public override string ToString() => "#N/A";
}

/// <summary>
/// Represents a security error when expression contains potentially unsafe code.
/// </summary>
public class FxUnsafeExpressionException : FxException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FxUnsafeExpressionException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public FxUnsafeExpressionException(string message = "Expression contains potentially unsafe code and cannot be executed") : base(message)
    {
    }
}

/// <summary>
/// Represents a general expression error during evaluation.
/// </summary>
public class FxExpressionException : FxException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FxExpressionException"/> class.
    /// </summary>
    /// <param name="message">The error message describing the expression error.</param>
    public FxExpressionException(string message) : base($"Expression evaluation error: {message}")
    {
    }
}

/// <summary>
/// Represents a compilation error when expression cannot be compiled to C# code.
/// </summary>
public class FxCompilationErrorException : FxException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FxCompilationErrorException"/> class.
    /// </summary>
    /// <param name="message">The error message describing the compilation error.</param>
    public FxCompilationErrorException(string message = "Expression compilation failed") : base($"Compilation error: {message}")
    {
    }
}
