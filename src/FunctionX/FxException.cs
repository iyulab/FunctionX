namespace FunctionX;

public abstract class FxException(string message) : Exception(message)
{
}

/// <summary>
/// Represents a division by zero error, mimicking Excel's #DIV/0! error
/// </summary>
public class FxDivideByZeroException : FxException
{
    public FxDivideByZeroException(string message = "Division by zero error") : base(message)
    {
    }

    public override string ToString() => "#DIV/0!";
}

/// <summary>
/// Represents a value error when invalid data types are used, mimicking Excel's #VALUE! error
/// </summary>
public class FxValueException : FxException
{
    public FxValueException(string message = "Invalid value or data type error") : base(message)
    {
    }

    public override string ToString() => "#VALUE!";
}

/// <summary>
/// Represents a reference error when referencing invalid data, mimicking Excel's #REF! error
/// </summary>
public class FxReferenceException : FxException
{
    public FxReferenceException(string message = "Invalid reference error") : base(message)
    {
    }

    public override string ToString() => "#REF!";
}

/// <summary>
/// Represents a name error when function or variable name is not recognized, mimicking Excel's #NAME? error
/// </summary>
public class FxNameException : FxException
{
    public FxNameException(string message = "Unrecognized function or variable name error") : base(message)
    {
    }

    public override string ToString() => "#NAME?";
}

/// <summary>
/// Represents a numeric error when calculation results in invalid number, mimicking Excel's #NUM! error
/// </summary>
public class FxNumException : FxException
{
    public FxNumException(string message = "Invalid numeric calculation error") : base(message)
    {
    }

    public override string ToString() => "#NUM!";
}

/// <summary>
/// Represents a not available error when value is not available, mimicking Excel's #N/A error
/// </summary>
public class FxNAException : FxException
{
    public FxNAException(string message = "Value not available error") : base(message)
    {
    }

    public override string ToString() => "#N/A";
}

/// <summary>
/// Represents a security error when expression contains potentially unsafe code
/// </summary>
public class FxUnsafeExpressionException : FxException
{
    public FxUnsafeExpressionException(string message = "Expression contains potentially unsafe code and cannot be executed") : base(message)
    {
    }
}

/// <summary>
/// Represents a general expression error during evaluation
/// </summary>
public class FxExpressionException : FxException
{
    public FxExpressionException(string message) : base($"Expression evaluation error: {message}")
    {
    }
}

/// <summary>
/// Represents a compilation error when expression cannot be compiled to C# code
/// </summary>
public class FxCompilationErrorException : FxException
{
    public FxCompilationErrorException(string message = "Expression compilation failed") : base($"Compilation error: {message}")
    {
    }
}