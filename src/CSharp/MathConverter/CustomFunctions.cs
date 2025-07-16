﻿using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

#if WPF
using System.Windows;
#elif AVALONIA
using Avalonia;
using Avalonia.Data;
#endif

namespace HexInnovation
{
    sealed class NowFunction : ZeroArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo) => DateTime.Now;
    }
    sealed class UnsetValueFunction : ZeroArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo)
        {
#if XAMARIN
            return Xamarin.Forms.BindableProperty.UnsetValue;
#elif MAUI
            return Microsoft.Maui.Controls.BindableProperty.UnsetValue;
#elif WPF
            return DependencyProperty.UnsetValue;
#elif AVALONIA
            return AvaloniaProperty.UnsetValue;
#endif
        }
    }
    sealed class CosFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Cos(x);
        }
    }
    sealed class SinFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Sin(x);
        }
    }
    sealed class TanFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Tan(x);
        }
    }
    sealed class AbsFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Abs(x);
        }
    }
    sealed class AcosFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Acos(x);
        }
    }
    sealed class AsinFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Asin(x);
        }
    }
    sealed class AtanFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Atan(x);
        }
    }
    sealed class CeilingFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Ceiling(x);
        }
    }
    sealed class FloorFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Floor(x);
        }
    }
    sealed class SqrtFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return Math.Sqrt(x);
        }
    }
    sealed class DegreesFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return x / Math.PI * 180;
        }
    }
    sealed class RadiansFunction : OneDoubleFunction
    {
        public override double? Evaluate(CultureInfo cultureInfo, double x)
        {
            return x / 180 * Math.PI;
        }
    }
    sealed class ToLowerFunction : OneArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object argument)
        {
            return $"{argument}".ToLower();
        }
    }
    sealed class ToUpperFunction : OneArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object argument)
        {
            return $"{argument}".ToUpper();
        }
    }
#if WPF
    sealed class VisibleOrCollapsedFunction : OneArgFunction
    {
        public VisibleOrCollapsedFunction()
        {
            Debug.WriteLine($"{nameof(VisibleOrCollapsedFunction)} is deprecated. Use 'x ? `Visible` : `Collapsed` instead.'");
        }
        public override object Evaluate(CultureInfo cultureInfo, object argument)
        {
            return TryConvert<bool>(argument, out var value) && value ? Visibility.Visible : Visibility.Collapsed;
        }
    }
    sealed class VisibleOrHiddenFunction : OneArgFunction
    {
        public VisibleOrHiddenFunction()
        {
            Debug.WriteLine($"{nameof(VisibleOrCollapsedFunction)} is deprecated. Use 'x ? `Visible` : `Hidden` instead.'");
        }

        public override object Evaluate(CultureInfo cultureInfo, object argument)
        {
            return TryConvert<bool>(argument, out var value) && value ? Visibility.Visible : Visibility.Hidden;
        }
    }
#endif
    sealed class TryParseDoubleFunction : OneArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object argument)
        {
            if (TryConvert<double>(argument, out var @double))
                return @double;
            else if (TryConvert<string>(argument, out var @string) && double.TryParse(@string, NumberStyles.Number, cultureInfo, out @double))
                return @double;
            else
                return null;
        }
    }
    sealed class GetTypeFunction : OneArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object argument)
        {
            return argument?.GetType();
        }
    }
    sealed class StartsWithFunction : TwoArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object x, object y)
        {
            if (TryConvert<string>(x, out var string1))
            {
                if (!TryConvert<string>(y, out var string2))
                {
                    string2 = $"{y}";
                    if (string2.Length == 0)
                        return new bool?();
                }

                return string1.StartsWith(string2);
            }

            return new bool?();
        }
    }
    sealed class EndsWithFunction : TwoArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object x, object y)
        {
            if (TryConvert<string>(x, out var string1))
            {
                if (!TryConvert<string>(y, out var string2))
                {
                    string2 = $"{y}";
                    if (string2.Length == 0)
                        return new bool?();
                }

                return string1.EndsWith(string2);
            }

            return new bool?();
        }
    }
    sealed class Atan2Function : TwoArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object x, object y)
        {
            if (TryConvert<double>(x, out var a) && TryConvert<double>(y, out var b))
                return Math.Atan2(a, b);
            else
                return null;
        }
    }
    sealed class LogFunction : TwoArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object x, object y)
        {
            if (TryConvert<double>(x, out var a) && TryConvert<double>(y, out var b))
                return Math.Log(a, b);
            else
                return null;
        }
    }
    sealed class ContainsFunction : TwoArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object x, object y)
        {
            if (x is string str1 && (y is string || $"{y}".Length > 0))
            {
                return str1.Contains($"{y}");
            }
            else if (x is IEnumerable @enum)
            {
                return @enum.OfType<object>().Contains(y);
            }
            else
            {
                return null;
            }
        }
    }
    sealed class ConvertTypeFunction : TwoArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object x, object y)
        {
            return y is Type type ? MathConverter.ConvertType(x, type) : x;
        }
    }
    sealed class EnumEqualsFunction : TwoArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, object x, object y)
        {
            if (x is null || y is null)
                return x is null && y is null;

            var xType = x.GetType();
            var yType = y.GetType();

            var xIsEnum = xType.GetTypeInfo().IsEnum;
            var yIsEnum = yType.GetTypeInfo().IsEnum;

            // Enums cannot be inherited, so two enums will only be equal if they are the same type.
            // Technically, this is different from the default behavior. By default, enums of disparate types
            // will be equal as long as their integer value is the same (typically this means it's declared in the same order).
            if (xIsEnum && yIsEnum)
                return xType.Equals(yType) && TryConvert<bool>(Operator.Equality.Evaluate(x, y), out var result) && result;

            if (!xIsEnum && !yIsEnum)
                return false;

            var @enum = xIsEnum ? x : y;
            var other = xIsEnum ? y : x;
            var enumType = xIsEnum ? xType : yType;

            try
            {
                return MathConverter.ConvertType(other, enumType) is { } converted
                    && converted.GetType() == enumType
                    && TryConvert<bool>(Operator.Equality.Evaluate(@enum, converted), out var result2)
                    && result2;
            }
            catch (FormatException) // We failed to convert the non-enum to the enum type.
            {
                return false;
            }
        }
    }
    sealed class IsNullFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] arguments)
        {
            return arguments[0]() ?? arguments[1]();
        }
        public override bool IsValidNumberOfParameters(int numParams) => numParams == 2;
    }
    sealed class RoundFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] arguments)
        {
            switch (arguments.Length)
            {
                case 1:
                    if (TryConvert<double>(arguments[0](), out var value))
                        return Math.Round(value);
                    else
                        return null;
                case 2:
                    if (TryConvert<double>(arguments[0](), out var a) && TryConvert<double>(arguments[1](), out var b))
                    {
                        if (b == (int)b)
                            return Math.Round(a, (int)b);
                        else
                            throw new Exception($"The second argument for {FunctionName} (if specified) must be an integer.");
                    }
                    else
                        return null;
                default:
                    throw new Exception($"The function {FunctionName} only accepts one or two arguments. It should be called like \"{FunctionName}(3.4)\" or \"{FunctionName}(3.45;1)\".");
            }
        }
        public override bool IsValidNumberOfParameters(int numParams) => numParams is 1 or 2;
    }
    sealed class AndFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] arguments)
        {
            var currentValueIsDefined = false;
            object currentValue = null;

            foreach (var arg in arguments.Select(x => x()))
            {
                if (currentValueIsDefined)
                {
                    currentValue = Operator.And.Evaluate(currentValue, arg);
                }
                else
                {
                    currentValue = arg;
                    currentValueIsDefined = true;
                }

                if (TryConvert<bool>(currentValue, out var v) && !v)
                {
                    return currentValue;
                }
            }

            return currentValue;
        }
        public override bool IsValidNumberOfParameters(int numParams) => numParams > 0;
    }
    sealed class OrFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] arguments)
        {
            var currentValueIsDefined = false;
            object currentValue = null;

            foreach (var arg in arguments.Select(x => x()))
            {
                if (currentValueIsDefined)
                {
                    currentValue = Operator.Or.Evaluate(currentValue, arg);
                }
                else
                {
                    currentValue = arg;
                    currentValueIsDefined = true;
                }

                if (TryConvert<bool>(currentValue, out var v) && v)
                {
                    return currentValue;
                }
            }

            return false;
        }
        public override bool IsValidNumberOfParameters(int numParams) => numParams > 0;
    }
    sealed class NorFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] arguments)
        {
            return Operator.LogicalNot.Evaluate(new OrFunction().Evaluate(cultureInfo, arguments));
        }
        public override bool IsValidNumberOfParameters(int numParams) => numParams > 0;
    }
    sealed class MaxFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] arguments)
        {
            var currentValueIsDefined = false;
            object max = null;

            foreach (var arg in arguments.Select(x => x()))
            {
                if (currentValueIsDefined)
                {
                    if (TryConvert<bool>(Operator.GreaterThan.Evaluate(arg, max), out var v) && v)
                    {
                        max = arg;
                    }
                }
                else
                {
                    max = arg;
                    currentValueIsDefined = arg != null;
                }
            }

            return max;
        }
    }
    sealed class MinFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] arguments)
        {
            var currentValueIsDefined = false;
            object min = null;

            foreach (var arg in arguments.Select(x => x()))
            {
                if (currentValueIsDefined)
                {
                    if (TryConvert<bool>(Operator.LessThan.Evaluate(arg, min), out var v) && v)
                    {
                        min = arg;
                    }
                }
                else
                {
                    min = arg;
                    currentValueIsDefined = arg != null;
                }
            }

            return min;
        }
    }
    sealed class FormatFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] arguments)
        {
            // Make sure we don't evaluate any of the arguments twice.
            if (arguments.Length > 0 && arguments[0]() is string format)
            {
                return string.Format(cultureInfo, format, arguments.Skip(1).Select(x => x()).ToArray());
            }
            else
            {
                throw new ArgumentException($"The {FunctionName} function must be called with a string as the first argument.");
            }
        }
        public override bool IsValidNumberOfParameters(int numParams) => numParams > 0;
    }
    sealed class ConcatFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] arguments)
        {
            return string.Concat((arguments.Length == 1 && arguments[0]() is IEnumerable enumerable ? enumerable.Cast<object>() : arguments.Select(x => x())).MyToArray());
        }
    }
    sealed class JoinFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] arguments)
        {
            if (arguments[0]() is string separator)
            {
                var argVals = arguments.Skip(1).Select(x => x()).ToArray();

                return string.Join(separator, (argVals.Length == 1 && argVals[0] is IEnumerable enumerable ? enumerable.Cast<object>() : argVals).MyToArray());
            }
            else
            {
                throw new ArgumentException($"{FunctionName}() function must be called with a string as the first argument.");
            }
        }
        public override bool IsValidNumberOfParameters(int numParams) => numParams > 0;
    }
    sealed class AverageFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] arguments)
        {
            var args = arguments.Select(x => TryConvert<double>(x(), out var d) ? d : new double?())
                .Where(x => x.HasValue).Select(x => x.Value).ToList();

            return args.Count == 0 ? new double?() : args.Average();
        }
    }
    sealed class ThrowFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] arguments)
        {
            throw new Exception($"The {FunctionName} function was called with {arguments.Length} argument{(arguments.Length == 1 ? "" : "s")}: {string.Join(", ", arguments.Select(x => x()).MyToArray())}");
        }
    }
    sealed class TryCatchFunction : ArbitraryArgFunction
    {
        public override object Evaluate(CultureInfo cultureInfo, Func<object>[] getArgument)
        {
            int i;

            for (i = 0; i < getArgument.Length - 1; i++)
            {
                try
                {
                    return getArgument[i]();
                }
                catch { }
            }

            // Do not catch any exception thrown by the last argument.
            return getArgument[i]();
        }
        public override bool IsValidNumberOfParameters(int numParams) => numParams >= 2;
    }
}
