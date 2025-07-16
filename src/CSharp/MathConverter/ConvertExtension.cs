using System.ComponentModel;

#if WPF
using BindableProperty = System.Windows.DependencyProperty;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
#else
using System;
using System.Collections.Generic;
#endif

#if MAUI
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
#elif XAMARIN
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
#elif AVALONIA
using Avalonia;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
#endif

namespace HexInnovation;


/// <summary>
/// A wrapper around MultiBinding that simplifies the syntax of creating a multi-input MathConverter binding.
/// </summary>
#if WPF
[MarkupExtensionReturnType(typeof(object))]
[Localizability(LocalizationCategory.None, Modifiability = Modifiability.Unmodifiable, Readability = Readability.Unreadable)]
#elif MAUI || XAMARIN
[ContentProperty(nameof(Expression))]
[AcceptEmptyServiceProvider]
#endif
public sealed class ConvertExtension
#if WPF
    : MultiBinding
#elif AVALONIA
    : MarkupExtension
#else
    : IMarkupExtension<BindingBase>
#endif
{
    /// <summary>
    /// This is the default <see cref="MathConverter"/> used by the <see cref="ConvertExtension"/>. 
    /// </summary>
    public static MathConverter DefaultConverter { get; }

    static ConvertExtension()
    {
#if WPF
        if (Application.Current?.TryFindResource("Math") is MathConverter mathConverter)
#else
        if (Application.Current?.Resources.TryGetValue("Math", out var obj) == true && obj is MathConverter mathConverter)
#endif
            DefaultConverter = mathConverter;
        else
            DefaultConverter = new();
    }

    /// <summary>
    /// The <see cref="MultiBinding"/> of which we are a wrapper.
    /// </summary>
    internal readonly MultiBinding _binding;

    /// <summary>
    /// A binding to <see cref="BindableProperty.UnsetValue"/> that is used if we skip a value when adding bindings.
    /// </summary>
    private static readonly Binding unsetValueBinding = new Binding
    {
        Source =
#if AVALONIA
            AvaloniaProperty.UnsetValue
#else
            BindableProperty.UnsetValue
#endif
    };

    /// <summary>
    /// Creates a new ConvertExtension
    /// </summary>
    public ConvertExtension()
    {
#if WPF
        Converter = DefaultConverter;
        Mode = BindingMode.OneWay;
        _binding = this;
#else
        _binding = new MultiBinding { Converter = DefaultConverter, Mode = BindingMode.OneWay };
#endif
    }


#if WPF
    /// <summary>
    /// Creates a new ConvertExtension
    /// </summary>
    /// <param name="expression">The ConverterParameter</param>
    public ConvertExtension(string expression)
        : this()
    {
        Expression = expression;
    }
#elif AVALONIA
    /// <summary>
    /// Returns a MultiBinding with a x<see cref="MathConverter"/> to convert the value as specified.
    /// </summary>
    /// <param name="serviceProvider">The service that provides the value.</param>
    /// <returns>A MultiBinding that uses a MathConverter</returns>
    public override object ProvideValue(IServiceProvider serviceProvider) => _binding;
#else
    /// <summary>
    /// Returns a MultiBinding with a x<see cref="MathConverter"/> to convert the value as specified.
    /// </summary>
    /// <param name="serviceProvider">The service that provides the value.</param>
    /// <returns>A MultiBinding that uses a MathConverter</returns>
    public BindingBase ProvideValue(IServiceProvider serviceProvider) => _binding;
    /// <summary>
    /// Returns a MultiBinding with a x<see cref="MathConverter"/> to convert the value as specified.
    /// </summary>
    /// <param name="serviceProvider">The service that provides the value.</param>
    /// <returns>A MultiBinding that uses a MathConverter</returns>
    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
#endif


    /// <summary>
    /// The parameter to pass to converter.
    /// </summary>
    [DefaultValue(null)]
    public string Expression
    {
#if WPF
        get => ConverterParameter as string;
        set => ConverterParameter = value;
#else
        get => _binding.ConverterParameter as string;
        set => _binding.ConverterParameter = value;
#endif
    }

#if !WPF
    /// <summary>
    ///     Value to use when source cannot provide a value
    /// </summary>
    /// <remarks>
    ///     Initialized to DependencyProperty.UnsetValue; if FallbackValue is not set, BindingExpression
    ///     will return target property's default when Binding cannot get a real value.
    /// </remarks>
    public object FallbackValue
    {
        get => _binding.FallbackValue;
        set => _binding.FallbackValue = value;
    }


    /// <summary>
    /// Value used to represent "null" in the target property.
    /// </summary>
    public object TargetNullValue
    {
        get => _binding.TargetNullValue;
        set => _binding.TargetNullValue = value;
    }
#endif

#if WPF || MAUI || XAMARIN
    private void SetBinding(int index, BindingBase binding)
    {
        while (Bindings.Count < index)
            Bindings.Add(unsetValueBinding);

        if (Bindings.Count == index)
             Bindings.Add(binding);
        else
            Bindings[index] = binding;

    }
#elif Avalonia
    private void SetBinding(int index, IBinding binding)
    {
        while (_binding.Bindings.Count < index)
            _binding.Bindings.Add(unsetValueBinding);

        if (_binding.Bindings.Count == index)
            _binding.Bindings.Add(binding);
        else
            _binding.Bindings[index] = binding;
    }
#endif



#if !WPF && (MAUI || XAMARIN)
    private IList<BindingBase> Bindings => _binding.Bindings;
#endif

#if WPF || MAUI || XAMARIN
/// <summary>
    /// The first variable (accessed by [0] or x)
    /// </summary>
    public BindingBase x
    {
        get => Bindings[0];
        set => SetBinding(0, value);
    }
    /// <summary>
    /// The second variable (accessed by [1] or y)
    /// </summary>
    public BindingBase y
    {
        get => Bindings[1];
        set => SetBinding(1, value);
    }
    /// <summary>
    /// The third variable (accessed by [2] or z)
    /// </summary>
    public BindingBase z
    {
        get => Bindings[2];
        set => SetBinding(2, value);
    }
    /// <summary>
    /// The fourth variable (accessed by [3])
    /// </summary>
    public BindingBase Var3
    {
        get => Bindings[3];
        set => SetBinding(3, value);
    }
    /// <summary>
    /// The fifth variable (accessed by [4])
    /// </summary>
    public BindingBase Var4
    {
        get => Bindings[4];
        set => SetBinding(4, value);
    }
    /// <summary>
    /// The sixth variable (accessed by [5])
    /// </summary>
    public BindingBase Var5
    {
        get => Bindings[5];
        set => SetBinding(5, value);
    }
    /// <summary>
    /// The seventh variable (accessed by [6])
    /// </summary>
    public BindingBase Var6
    {
        get => Bindings[6];
        set => SetBinding(6, value);
    }
    /// <summary>
    /// The eighth variable (accessed by [7])
    /// </summary>
    public BindingBase Var7
    {
        get => Bindings[7];
        set => SetBinding(7, value);
    }
    /// <summary>
    /// The ninth variable (accessed by [8])
    /// </summary>
    public BindingBase Var8
    {
        get => Bindings[8];
        set => SetBinding(8, value);
    }
    /// <summary>
    /// The tenth variable (accessed by [9])
    /// </summary>
    public BindingBase Var9
    {
        get => Bindings[9];
        set => SetBinding(9, value);
    }
#elif Avalonia
    /// <summary>
    /// The first variable (accessed by [0] or x)
    /// </summary>
    public IBinding x
    {
        get => _binding.Bindings[0];
        set => SetBinding(0, value);
    }
    /// <summary>
    /// The second variable (accessed by [1] or y)
    /// </summary>
    public IBinding y
    {
        get => _binding.Bindings[1];
        set => SetBinding(1, value);
    }
    /// <summary>
    /// The third variable (accessed by [2] or z)
    /// </summary>
    public IBinding z
    {
        get => _binding.Bindings[2];
        set => SetBinding(2, value);
    }
    /// <summary>
    /// The fourth variable (accessed by [3])
    /// </summary>
    public IBinding Var3
    {
        get => _binding.Bindings[3];
        set => SetBinding(3, value);
    }
    /// <summary>
    /// The fifth variable (accessed by [4])
    /// </summary>
    public IBinding Var4
    {
        get => _binding.Bindings[4];
        set => SetBinding(4, value);
    }
    /// <summary>
    /// The sixth variable (accessed by [5])
    /// </summary>
    public IBinding Var5
    {
        get => _binding.Bindings[5];
        set => SetBinding(5, value);
    }
    /// <summary>
    /// The seventh variable (accessed by [6])
    /// </summary>
    public IBinding Var6
    {
        get => _binding.Bindings[6];
        set => SetBinding(6, value);
    }
    /// <summary>
    /// The eighth variable (accessed by [7])
    /// </summary>
    public IBinding Var7
    {
        get => _binding.Bindings[7];
        set => SetBinding(7, value);
    }
    /// <summary>
    /// The ninth variable (accessed by [8])
    /// </summary>
    public IBinding Var8
    {
        get => _binding.Bindings[8];
        set => SetBinding(8, value);
    }
    /// <summary>
    /// The tenth variable (accessed by [9])
    /// </summary>
    public IBinding Var9
    {
        get => _binding.Bindings[9];
        set => SetBinding(9, value);
    }
#endif


}
