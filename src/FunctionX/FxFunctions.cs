using System.Globalization;

namespace FunctionX;

/// <summary>
/// Contains all Excel-like functions that can be used in FunctionX expressions.
/// This class provides the runtime context for expression evaluation.
/// </summary>
public partial class FxFunctions
{
    private readonly IDictionary<string, object?> parameters;

    /// <summary>
    /// Initializes a new instance of FxFunctions with the provided parameters.
    /// </summary>
    /// <param name="parameters">Dictionary of parameters that can be referenced in expressions</param>
    public FxFunctions(IDictionary<string, object?> parameters)
    {
        this.parameters = parameters;
    }

    #region Private helper methods

    /// <summary>
    /// Determines if a value is a numeric type that can be used in calculations.
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <returns>True if the value is a numeric type</returns>
    private static bool IsNumberType(object value)
    {
        return value is int || value is double || value is float || value is decimal;
    }

    /// <summary>
    /// Filters and converts an enumerable of objects to numeric values, excluding non-numeric items.
    /// </summary>
    /// <param name="values">The values to filter and convert</param>
    /// <returns>An enumerable of double values</returns>
    private static IEnumerable<double> FilterNumericValues(IEnumerable<object> values)
    {
        return values
            .Where(v => v != null && IsNumberType(v))
            .Select(v => Convert.ToDouble(v));
    }

    /// <summary>
    /// Recursively flattens nested arrays and enumerables into a single sequence.
    /// </summary>
    /// <param name="array">The array or enumerable to flatten</param>
    /// <returns>A flattened sequence of objects</returns>
    private static IEnumerable<object> Flatten(IEnumerable<object> array)
    {
        foreach (var item in array)
        {
            if (item is IEnumerable<object> enumerable && item is not string)
            {
                foreach (var subItem in Flatten(enumerable))
                {
                    yield return subItem;
                }
            }
            else
            {
                yield return item;
            }
        }
    }

    #endregion

    public object[] GetItems(string name)
    {
        var v = parameters[name];
        if (v == null)
        {
            return []; // 빈 배열 반환
        }
        else if (v.GetType().IsArray)
        {
            return (object[])v; // v가 배열인 경우 그대로 반환
        }
        else if (v is List<Dictionary<string, object?>> list)
        {
            return list.Cast<object>().ToArray();
        }
        else if (v is IEnumerable<object> enumerable)
        {
            return enumerable.ToArray();
        }
        else
        {
            return [v];
        }
    }

    public object? GetItem(string name)
    {
        if (!parameters.ContainsKey(name))
        {
            throw new FxReferenceException();
        }
        return parameters[name];
    }

    public double[] GetValues(string name)
    {
        var v = parameters[name];
        if (v == null)
            return [];

        if (v.GetType().IsArray)
            return ((object[])v).Select(v => Convert.ToDouble(v)).ToArray();

        return [];
    }

    public double GetValue(string name)
    {
        if (!parameters.ContainsKey(name))
        {
            throw new FxReferenceException(); // #REF! 예외 발생
        }

        var value = parameters[name] ?? throw new FxNAException();
        try
        {
            return Convert.ToDouble(value);
        }
        catch (System.Exception)
        {
            throw new FxValueException(); // #VALUE! 예외 발생
        }
    }

    public static double SUM(params object[] values)
    {
        try
        {
            var numericValues = FilterNumericValues(Flatten(values));
            return numericValues.Sum();
        }
        catch (System.Exception)
        {
            throw new FxValueException();
        }
    }

    public static double AVERAGE(params object[] values)
    {
        var flattenedValues = Flatten(values).Where(v => v != null).Select(v =>
        {
            try
            {
                return Convert.ToDouble(v);
            }
            catch (System.Exception)
            {
                return double.NaN;
            }
        });

        if (flattenedValues.Any(double.IsNaN) || !flattenedValues.Any())
        {
            return double.NaN; // 데이터 중 숫자로 변환할 수 없는 값이 있으면 NaN 반환
        }

        return flattenedValues.Average();
    }

    public static double MAX(params object[] values)
    {
        var flattenedValues = Flatten(values).Where(v => v != null).Select(v =>
        {
            try
            {
                return Convert.ToDouble(v);
            }
            catch (System.Exception)
            {
                return double.NaN;
            }
        });

        if (flattenedValues.Any(double.IsNaN) || !flattenedValues.Any())
        {
            return double.NaN;
        }

        return flattenedValues.Max();
    }

    public static double MIN(params object[] values)
    {
        if (values.Length == 0) return double.NaN;

        // 배열에 null이 포함되어 있더라도 유효한 숫자 값만을 고려하여 최소값을 계산해야 합니다.
        return values.Where(v => v != null).Select(v => Convert.ToDouble(v)).Min();
    }

    public static double COUNT(params object[] values)
    {
        var flattenedValues = Flatten(values).Where(v => v != null && IsNumberType(v));
        return flattenedValues.Count();
    }

    public static double COUNTA(params object[] values)
    {
        return Flatten(values).Count(v => v != null);
    }

    public static bool AND(params object[] values)
    {
        try
        {
            return Flatten(values).All(v => v != null && Convert.ToBoolean(v));
        }
        catch (System.Exception)
        {
            throw new FxValueException();
        }
    }

    public static bool OR(params object[] values)
    {
        return Flatten(values).Any(v => v != null && Convert.ToBoolean(v));
    }

    public static bool NOT(object value)
    {
        return value == null || !Convert.ToBoolean(value);
    }

    public static bool XOR(params object[] values)
    {
        return Flatten(values).Count(v => v != null && Convert.ToBoolean(v)) % 2 == 1;
    }

    public static object IF(object condition, object trueValue, object falseValue)
    {
        return Convert.ToBoolean(condition) ? trueValue : falseValue;
    }

    public static object? IFS(params object[] values)
    {
        if (values.Length % 2 != 0)
        {
            throw new FxValueException();
        }

        for (int i = 0; i < values.Length; i += 2)
        {
            if (Convert.ToBoolean(values[i]))
            {
                return values[i + 1];
            }
        }

        return null;
    }

    public static object? SWITCH(object value, params object[] casesAndValues)
    {
        if (casesAndValues.Length < 2)
        {
            throw new ArgumentException("There should be at least one case and one value.");
        }

        // casesAndValues의 길이가 홀수인 경우 마지막 값은 기본값으로 처리합니다.
        bool hasDefaultValue = casesAndValues.Length % 2 != 0;
        int loopCount = hasDefaultValue ? casesAndValues.Length - 1 : casesAndValues.Length;

        for (int i = 0; i < loopCount; i += 2)
        {
            if (Equals(value, casesAndValues[i]))
            {
                return casesAndValues[i + 1];
            }
        }

        // 모든 케이스를 확인한 후 일치하는 케이스가 없다면, 기본값 반환 (기본값이 제공된 경우)
        return hasDefaultValue ? casesAndValues[^1] : null;
    }


    public static string CONCAT(params object[] values)
    {
        var flattenedValues = Flatten(values);
        return string.Join("", flattenedValues);
    }

    public static string LEFT(object input, object count)
    {
        if (input is not string text)
        {
            throw new FxValueException(); // #VALUE!
        }
        try
        {
            return text[..Math.Min(Convert.ToInt32(count), text.Length)];
        }
        catch (System.Exception)
        {
            throw new FxValueException(); // #VALUE!
        }
    }

    public static string RIGHT(object input, object count)
    {
        if (input is string text)
        {
            return text[Math.Max(0, text.Length - Convert.ToInt32(count))..];
        }
        else
        {
            return string.Empty;
        }
    }

    public static string MID(object input, object start, object count)
    {
        if (input is string text)
        {
            int startIndex = Convert.ToInt32(start) - 1;
            if (startIndex >= text.Length) return string.Empty;

            startIndex = Math.Max(0, startIndex);
            int length = Math.Min(Convert.ToInt32(count), text.Length - startIndex);
            return text.Substring(startIndex, length);
        }
        else
        {
            return string.Empty;
        }
    }

    public static string TRIM(object input)
    {
        return input is string text ? text.Trim() : string.Empty;
    }

    public static string UPPER(object input)
    {
        return input is string text ? text.ToUpper() : string.Empty;
    }

    public static string LOWER(object input)
    {
        return input is string text ? text.ToLower() : string.Empty;
    }

    public static string PROPER(object input)
    {
        if (input is not string text)
        {
            throw new FxValueException(); // #VALUE!
        }
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
    }

    public static string REPLACE(object input, object oldValue, object newValue)
    {
        if (input is not string text)
        {
            throw new FxValueException(); // #VALUE!
        }

        if (oldValue == null || newValue == null)
        {
            throw new FxValueException(); // #VALUE!
        }

        try
        {
            return text.Replace(oldValue.ToString()!, newValue.ToString()!);
        }
        catch (System.Exception)
        {
            throw new FxValueException(); // #VALUE!
        }
    }

    public static int LEN(object input)
    {
        if (input is not string text)
        {
            throw new FxValueException(); // #VALUE!
        }
        return text.Length;
    }

    public static object? INDEX(object[] range, object rowIndexObj, object columnIndexObj)
    {
        int rowIndex = Convert.ToInt32(rowIndexObj);

        // rowIndex는 1부터 시작하는 것을 가정합니다.
        if (rowIndex < 1 || rowIndex > range.Length)
        {
            throw new FxReferenceException();
        }

        var row = range[rowIndex - 1];
        if (row is object[] rowArray)
        {
            var columnIndex = Convert.ToInt32(columnIndexObj);
            // 배열의 범위를 체크
            if (columnIndex < 1 || columnIndex > rowArray.Length)
            {
                return null;
            }
            return rowArray[columnIndex - 1]; // 배열 인덱스는 0부터 시작
        }
        else if (row is List<Dictionary<string, object?>> listDictionary)
        {
            if (rowIndex > listDictionary.Count)
            {
                return null;
            }
            var dictionary = listDictionary[rowIndex - 1];

            // columnIndexObj가 string 타입일 경우의 처리
            if (columnIndexObj is string columnName)
            {
                // 해당 열 이름으로 값을 찾음
                if (dictionary.TryGetValue(columnName, out object? value))
                {
                    return value;
                }
            }
        }
        else if (row is Dictionary<string, object?> dictionary)
        {
            // columnIndexObj가 string 타입일 경우의 처리
            if (columnIndexObj is string columnName && dictionary.TryGetValue(columnName, out object? value))
            {
                return value;
            }
            else
            {
                var columnIndex = Convert.ToInt32(columnIndexObj);
                if (columnIndex < 1 || columnIndex > dictionary.Count)
                {
                    return null;
                }
                return dictionary.ElementAt(columnIndex - 1).Value;
            }
        }

        // 처리할 수 없는 타입이거나 범위를 벗어나는 경우
        return null;
    }

    public static object? VLOOKUP(object key, object range, object pColumnIndex, object? pExactMatch)
    {
        var columnIndex = Convert.ToInt32(pColumnIndex) - 1;
        var exactMatch = pExactMatch == null || Convert.ToBoolean(pExactMatch);
        var rows = range as object[] ?? throw new ArgumentException("The range must be an array of dictionaries.");

        bool isKeyNumber = double.TryParse(key.ToString(), out double keyNumber);

        // 근사치 검색을 위한 변수 준비
        Dictionary<string, object?>? closestMatchRow = null;
        double closestMatchValue = double.MinValue;

        foreach (var item in rows)
        {
            var row = item as Dictionary<string, object?> ?? throw new ArgumentException("Each row must be a dictionary.");
            var firstColumnValue = row.Values.First()!;
            bool isRowValueNumber = double.TryParse(firstColumnValue.ToString(), out double rowValue);

            if (exactMatch)
            {
                if (key.Equals(firstColumnValue))
                {
                    return row.ElementAtOrDefault(columnIndex).Value;
                }
            }
            else if (!exactMatch && isRowValueNumber && isKeyNumber && rowValue <= keyNumber && rowValue > closestMatchValue)
            {
                closestMatchRow = row;
                closestMatchValue = rowValue;
            }
        }

        if (!exactMatch && closestMatchRow != null)
        {
            return closestMatchRow.ElementAtOrDefault(columnIndex).Value;
        }

        return null;
    }

    public static object[] UNIQUE(params object[] values)
    {
        try
        {
            return Flatten(values).Distinct().ToArray();
        }
        catch (System.Exception)
        {
            throw new FxExpressionException("Unique expression error.");
        }
    }

    public static int INT(object value)
    {
        return Convert.ToInt32(value);
    }

    /// <summary>
    /// Counts the number of cells in a range that meet a specified condition.
    /// Similar to Excel's COUNTIF function.
    /// </summary>
    /// <param name="range">Array of values to evaluate</param>
    /// <param name="criteria">Condition to test each value against</param>
    /// <returns>Count of values meeting the criteria</returns>
    public static double COUNTIF(object[] range, object criteria)
    {
        try
        {
            var flattenedRange = Flatten(range);
            var criteriaString = criteria.ToString() ?? "";

            // Parse criteria - supports >, <, >=, <=, =, <>, and exact matches
            if (criteriaString.StartsWith(">="))
            {
                var threshold = Convert.ToDouble(criteriaString[2..]);
                return flattenedRange.Count(v => v != null && IsNumberType(v) && Convert.ToDouble(v) >= threshold);
            }
            else if (criteriaString.StartsWith("<="))
            {
                var threshold = Convert.ToDouble(criteriaString[2..]);
                return flattenedRange.Count(v => v != null && IsNumberType(v) && Convert.ToDouble(v) <= threshold);
            }
            else if (criteriaString.StartsWith("<>"))
            {
                var compareValue = criteriaString[2..];
                return flattenedRange.Count(v => v?.ToString() != compareValue);
            }
            else if (criteriaString.StartsWith(">"))
            {
                var threshold = Convert.ToDouble(criteriaString[1..]);
                return flattenedRange.Count(v => v != null && IsNumberType(v) && Convert.ToDouble(v) > threshold);
            }
            else if (criteriaString.StartsWith("<"))
            {
                var threshold = Convert.ToDouble(criteriaString[1..]);
                return flattenedRange.Count(v => v != null && IsNumberType(v) && Convert.ToDouble(v) < threshold);
            }
            else if (criteriaString.StartsWith("="))
            {
                var compareValue = criteriaString[1..];
                return flattenedRange.Count(v => v?.ToString() == compareValue);
            }
            else
            {
                // Exact match (string or numeric)
                if (double.TryParse(criteriaString, out double numericCriteria))
                {
                    return flattenedRange.Count(v => v != null && IsNumberType(v) && Math.Abs(Convert.ToDouble(v) - numericCriteria) < 0.0000001);
                }
                else
                {
                    return flattenedRange.Count(v => v?.ToString() == criteriaString);
                }
            }
        }
        catch (System.Exception)
        {
            throw new FxValueException("Invalid criteria in COUNTIF function");
        }
    }

    /// <summary>
    /// Sums the values in a range that meet a specified condition.
    /// Similar to Excel's SUMIF function.
    /// </summary>
    /// <param name="range">Array of values to evaluate</param>
    /// <param name="criteria">Condition to test each value against</param>
    /// <param name="sumRange">Optional array of values to sum (if null, uses range)</param>
    /// <returns>Sum of values meeting the criteria</returns>
    public static double SUMIF(object[] range, object criteria, object[]? sumRange = null)
    {
        try
        {
            var flattenedRange = Flatten(range).ToArray();
            var flattenedSumRange = sumRange != null ? Flatten(sumRange).ToArray() : flattenedRange;
            var criteriaString = criteria.ToString() ?? "";

            double sum = 0;

            for (int i = 0; i < flattenedRange.Length; i++)
            {
                var rangeValue = flattenedRange[i];
                var sumValue = i < flattenedSumRange.Length ? flattenedSumRange[i] : null;

                bool meetsCriteria = false;

                // Parse criteria - supports >, <, >=, <=, =, <>, and exact matches
                if (criteriaString.StartsWith(">="))
                {
                    var threshold = Convert.ToDouble(criteriaString[2..]);
                    meetsCriteria = rangeValue != null && IsNumberType(rangeValue) && Convert.ToDouble(rangeValue) >= threshold;
                }
                else if (criteriaString.StartsWith("<="))
                {
                    var threshold = Convert.ToDouble(criteriaString[2..]);
                    meetsCriteria = rangeValue != null && IsNumberType(rangeValue) && Convert.ToDouble(rangeValue) <= threshold;
                }
                else if (criteriaString.StartsWith("<>"))
                {
                    var compareValue = criteriaString[2..];
                    meetsCriteria = rangeValue?.ToString() != compareValue;
                }
                else if (criteriaString.StartsWith(">"))
                {
                    var threshold = Convert.ToDouble(criteriaString[1..]);
                    meetsCriteria = rangeValue != null && IsNumberType(rangeValue) && Convert.ToDouble(rangeValue) > threshold;
                }
                else if (criteriaString.StartsWith("<"))
                {
                    var threshold = Convert.ToDouble(criteriaString[1..]);
                    meetsCriteria = rangeValue != null && IsNumberType(rangeValue) && Convert.ToDouble(rangeValue) < threshold;
                }
                else if (criteriaString.StartsWith("="))
                {
                    var compareValue = criteriaString[1..];
                    meetsCriteria = rangeValue?.ToString() == compareValue;
                }
                else
                {
                    // Exact match (string or numeric)
                    if (double.TryParse(criteriaString, out double numericCriteria))
                    {
                        meetsCriteria = rangeValue != null && IsNumberType(rangeValue) && Math.Abs(Convert.ToDouble(rangeValue) - numericCriteria) < 0.0000001;
                    }
                    else
                    {
                        meetsCriteria = rangeValue?.ToString() == criteriaString;
                    }
                }

                if (meetsCriteria && sumValue != null && IsNumberType(sumValue))
                {
                    sum += Convert.ToDouble(sumValue);
                }
            }

            return sum;
        }
        catch (System.Exception)
        {
            throw new FxValueException("Invalid criteria or range in SUMIF function");
        }
    }

    /// <summary>
    /// Rounds a number to a specified number of decimal places.
    /// </summary>
    /// <param name="number">The number to round</param>
    /// <param name="digits">Number of decimal places (can be negative)</param>
    /// <returns>Rounded number</returns>
    public static double ROUND(object number, object digits)
    {
        try
        {
            var num = Convert.ToDouble(number);
            var decimals = Convert.ToInt32(digits);

            // Handle negative decimal places (round to tens, hundreds, etc.)
            if (decimals < 0)
            {
                var multiplier = Math.Pow(10, -decimals);
                return Math.Round(num / multiplier) * multiplier;
            }

            return Math.Round(num, decimals, MidpointRounding.AwayFromZero);
        }
        catch (System.Exception)
        {
            throw new FxValueException("Invalid number or digits in ROUND function");
        }
    }

    /// <summary>
    /// Returns the absolute value of a number.
    /// </summary>
    /// <param name="number">The number</param>
    /// <returns>Absolute value</returns>
    public static double ABS(object number)
    {
        try
        {
            return Math.Abs(Convert.ToDouble(number));
        }
        catch (System.Exception)
        {
            throw new FxValueException("Invalid number in ABS function");
        }
    }

    /// <summary>
    /// Checks if a value is blank (null or empty string).
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <returns>True if blank, false otherwise</returns>
    public static bool ISBLANK(object? value)
    {
        return value == null ||
               (value is string str && string.IsNullOrWhiteSpace(str)) ||
               (value is DBNull);
    }

    /// <summary>
    /// Checks if a value is a number.
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <returns>True if numeric, false otherwise</returns>
    public static bool ISNUMBER(object? value)
    {
        if (value == null || value is DBNull) return false;

        return IsNumberType(value) ||
               (value is string str && double.TryParse(str, out _));
    }
}