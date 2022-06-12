#region License
// Copyright 2020 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Reflection;

using AppMotor.CliApp.Terminals;
using AppMotor.Core.Exceptions;
using AppMotor.Core.Utils;

using Shouldly;

using Xunit;

namespace AppMotor.CliApp.Tests.Terminals;

public sealed class ColoredStringTests
{
    /// <summary>
    /// Tests that the various <c>TextIn...</c> classes can be created correctly
    /// and have the correct color set.
    /// </summary>
    [Fact]
    public void TestTextInClasses()
    {
        const string TEST_TEXT = "my test text";

        foreach (var consoleColor in EnumUtils.GetValues<ConsoleColor>())
        {
            var textInValue = CreateTextInClassInstance(consoleColor, TEST_TEXT);

            textInValue.Color.ShouldBe(consoleColor);
            textInValue.Text.ShouldBe(TEST_TEXT);
            textInValue.ToString().ShouldBe(TEST_TEXT);
        }
    }

    [Fact]
    public void TestTextInClasses_NullValue()
    {
        foreach (var consoleColor in EnumUtils.GetValues<ConsoleColor>())
        {
            var textInValue = CreateTextInClassInstance(consoleColor, value: null);

            textInValue.Color.ShouldBe(consoleColor);
            textInValue.Text.ShouldBe("");
        }
    }

    [Fact]
    public void TestTextInClasses_EmptyValue()
    {
        foreach (var consoleColor in EnumUtils.GetValues<ConsoleColor>())
        {
            var textInValue = CreateTextInClassInstance(consoleColor, "");

            textInValue.Color.ShouldBe(consoleColor);
            textInValue.Text.ShouldBe("");
        }
    }

    private static ColoredSubstring CreateTextInClassInstance(ConsoleColor color, string? value)
    {
        var textInClassesAssembly = typeof(TextInBlack).Assembly;
        var textInClassesNamespace = typeof(TextInBlack).Namespace;

        var textInType = textInClassesAssembly.GetType($"{textInClassesNamespace}.TextIn{color}");
        textInType.ShouldNotBeNull();

        // See: https://stackoverflow.com/a/11113450/614177
        var createOperator = textInType.GetMethod("op_Explicit", BindingFlags.Static | BindingFlags.Public);
        createOperator.ShouldNotBeNull();

        var textInValue = (ColoredSubstring)createOperator.Invoke(null, new object[] { value! })!;
        textInValue.ShouldNotBeNull();

        return textInValue;
    }

    [Fact]
    public void TestAppendOperators()
    {
        // setup
        // ReSharper disable once ConvertToConstant.Local
        var regularText = "my regular text1";
        var coloredSubstring1 = (TextInYellow)"my text in yellow";
        var coloredSubstring2 = (TextInBlue)"my text in blue";
        var coloredString1 = ColoredString.New().Append(ConsoleColor.Red, "my text in red");
        var coloredString2 = ColoredString.New().Append(ConsoleColor.Cyan, "my text in cyan");

        // test
        VerifyColoredStringResult(regularText       + coloredSubstring1,    regularText,        coloredSubstring1);
        VerifyColoredStringResult(regularText       + coloredString1,       regularText,        coloredString1);
        VerifyColoredStringResult(coloredSubstring1 + regularText,          coloredSubstring1,  regularText);
        VerifyColoredStringResult(coloredString1    + regularText,          coloredString1,     regularText);

        VerifyColoredStringResult(coloredString1    + coloredString2,       coloredString1,     coloredString2);
        VerifyColoredStringResult(coloredString1    + coloredString1,       coloredString1,     coloredString1);
        VerifyColoredStringResult(coloredString1    + coloredSubstring1,    coloredString1,     coloredSubstring1);
        VerifyColoredStringResult(coloredSubstring1 + coloredString1,       coloredSubstring1,  coloredString1);

        VerifyColoredStringResult(coloredSubstring1 + coloredSubstring2,    coloredSubstring1,  coloredSubstring2);
    }

    [Fact]
    public void TestImplicitOperators()
    {
        ColoredString string1 = "abc";
        string1.Count.ShouldBe(1);
        string1[0].Text.ShouldBe("abc");
        string1[0].Color.ShouldBe(null);

        ColoredString string2 = (TextInRed)"def";
        string2.Count.ShouldBe(1);
        string2[0].Text.ShouldBe("def");
        string2[0].Color.ShouldBe(ConsoleColor.Red);
    }

    [Fact]
    public void TestAppendOperatorDoesNotChangeOperands()
    {
        // setup
        var coloredString1 = ColoredString.New().Append(ConsoleColor.Red, "my text in red");
        var coloredString2 = ColoredString.New().Append(ConsoleColor.Cyan, "my text in cyan");

        // test
        var coloredString3 = coloredString1 + coloredString2;

        // verify
        coloredString3.ShouldNotBeSameAs(coloredString1);
        coloredString3.ShouldNotBeSameAs(coloredString2);
        coloredString3.Count.ShouldBe(2);
        coloredString1.Count.ShouldBe(1);
        coloredString2.Count.ShouldBe(1);
    }

    private static void VerifyColoredStringResult(
            ColoredString? result,
            object operand1,
            object operand2
        )
    {
        static ColoredSubstring CreateExpectedSubstring(object operand)
        {
            switch (operand)
            {
                case ColoredString coloredString:
                    return coloredString[0];

                case ColoredSubstring coloredSubstring:
                    return coloredSubstring;

                case string uncoloredString:
                    return ColoredString.New().Append(color: null, uncoloredString)[0];

                default:
                    throw new UnexpectedSwitchValueException(nameof(operand), operand.GetType());
            }
        }

        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);

        var expectedSubstring1 = CreateExpectedSubstring(operand1);
        var expectedSubstring2 = CreateExpectedSubstring(operand2);

        result[0].ShouldBe(expectedSubstring1);
        result[1].ShouldBe(expectedSubstring2);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TestAppend_ColoredString(bool overrideColor)
    {
        var overrideColor1 = overrideColor ? ConsoleColor.Blue : (ConsoleColor?)null;
        var overrideColor2 = overrideColor ? ConsoleColor.Red : (ConsoleColor?)null;

        var coloredString = ColoredString.New();

        coloredString.Append(color: null, (ColoredString)"abc");
        coloredString.Count.ShouldBe(1);
        coloredString[0].Text.ShouldBe("abc");
        coloredString[0].Color.ShouldBe(null);

        coloredString.Append(color: null, (ColoredString?)null);
        coloredString.Count.ShouldBe(1);

        coloredString.Append(color: null, (ColoredString)"");
        coloredString.Count.ShouldBe(1);

        coloredString.Append(color: overrideColor1, (ColoredString)"def");
        coloredString.Count.ShouldBe(2);
        coloredString[0].Text.ShouldBe("abc");
        coloredString[0].Color.ShouldBe(null);
        coloredString[1].Text.ShouldBe("def");
        coloredString[1].Color.ShouldBe(overrideColor1);

        coloredString.Append(color: overrideColor2, coloredString);
        coloredString.Count.ShouldBe(4);
        coloredString[0].Text.ShouldBe("abc");
        coloredString[0].Color.ShouldBe(null);
        coloredString[1].Text.ShouldBe("def");
        coloredString[1].Color.ShouldBe(overrideColor1);
        coloredString[2].Text.ShouldBe("abc");
        coloredString[2].Color.ShouldBe(overrideColor2);
        coloredString[3].Text.ShouldBe("def");
        coloredString[3].Color.ShouldBe(overrideColor1);
    }

    [Fact]
    public void TestAppend_WithColor_ColoredSubString_Null()
    {
        // Setup
        ColoredString str = (TextInWhite)"abc";

        // Test
        str.Append(ConsoleColor.Black, (TextInBlack?)null);

        // Verify
        var subStringList = str.ToList();
        subStringList.Count.ShouldBe(1);
        subStringList[0].Color.ShouldBe(ConsoleColor.White);
        subStringList[0].Text.ShouldBe("abc");
    }


    [Fact]
    public void TestAppend_ColoredSubString_Null()
    {
        // Setup
        ColoredString str = (TextInWhite)"abc";
        TextInBlack? nullSubString = null;

        // Test
        str.Append(nullSubString);

        // Verify
        var subStringList = str.ToList();
        subStringList.Count.ShouldBe(1);
        subStringList[0].Color.ShouldBe(ConsoleColor.White);
        subStringList[0].Text.ShouldBe("abc");
    }

    [Fact]
    public void TestAppend_ColoredSubString_Empty()
    {
        // Setup
        ColoredString str = (TextInWhite)"abc";

        // Test
        str.Append((TextInBlack)"");

        // Verify
        var subStringList = str.ToList();
        subStringList.Count.ShouldBe(1);
        subStringList[0].Color.ShouldBe(ConsoleColor.White);
        subStringList[0].Text.ShouldBe("abc");
    }

    [Theory]
    [InlineData(null)]
    [InlineData(ConsoleColor.Cyan)]
    public void TestAppend_String(ConsoleColor? color)
    {
        var coloredString = ColoredString.New();

        coloredString.Append(color: color, "abc");
        coloredString.Count.ShouldBe(1);
        coloredString[0].Text.ShouldBe("abc");
        coloredString[0].Color.ShouldBe(color);

        coloredString.Append(color: color, (string?)null);
        coloredString.Count.ShouldBe(1);

        coloredString.Append(color: color, "");
        coloredString.Count.ShouldBe(1);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(ConsoleColor.Cyan)]
    public void TestAppend_Object(ConsoleColor? color)
    {
        var coloredString = ColoredString.New();

        coloredString.Append(color: color, (object)"abc");
        coloredString.Count.ShouldBe(1);
        coloredString[0].Text.ShouldBe("abc");
        coloredString[0].Color.ShouldBe(color);

        coloredString.Append(color: color, (object?)null);
        coloredString.Count.ShouldBe(1);

        coloredString.Append(color: color, (object)"");
        coloredString.Count.ShouldBe(1);

        coloredString.Append(color: color, (object)(TextInRed)"def");
        coloredString.Count.ShouldBe(2);
        coloredString[1].Text.ShouldBe("def");
        coloredString[1].Color.ShouldBe(color ?? ConsoleColor.Red);

        coloredString.Append(color: color, (object)coloredString);
        coloredString.Count.ShouldBe(4);
        coloredString[2].Text.ShouldBe("abc");
        coloredString[2].Color.ShouldBe(color);
        coloredString[3].Text.ShouldBe("def");
        coloredString[3].Color.ShouldBe(color ?? ConsoleColor.Red);
    }

    /// <summary>
    /// Basically tests that the desired overloading method is chosen
    /// by the compiler.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData(ConsoleColor.Cyan)]
    public void TestAppend_DesiredOverload(ConsoleColor? color)
    {
        // ReSharper disable RedundantCast

        var coloredString = ColoredString.New().Append(color, 42);
        coloredString[0].Color.ShouldBe(color);
        coloredString[0].Text.ShouldBe("42");

        coloredString = ColoredString.New().Append(color, "abc");
        coloredString[0].Color.ShouldBe(color);
        coloredString[0].Text.ShouldBe("abc");

        coloredString = ColoredString.New().Append(color, (TextInWhite)"def");
        coloredString[0].Color.ShouldBe(color ?? ConsoleColor.White);
        coloredString[0].Text.ShouldBe("def");

        coloredString = ColoredString.New().Append(color, (ColoredString)"ghi");
        coloredString[0].Color.ShouldBe(color);
        coloredString[0].Text.ShouldBe("ghi");

        coloredString = ColoredString.New().Append(color, (ColoredString)(TextInGreen)"jkl");
        coloredString[0].Color.ShouldBe(ConsoleColor.Green);
        coloredString[0].Text.ShouldBe("jkl");

        // ReSharper restore RedundantCast
    }

    [Fact]
    public void TestToString()
    {
        var coloredString = "abc" + (TextInRed)"def";
        coloredString.ToString().ShouldBe("abcdef");
    }

    [Fact]
    public void TestToString_Empty()
    {
        var coloredString = ColoredString.New();
        coloredString.ToString().ShouldBe("");
    }

    [Fact]
    public void TestClone()
    {
        // Setup
        var sourceString = "abc" + (TextInRed)"def";

        // Test
        var clone = sourceString.Clone();
        sourceString += "ghi";
        clone += "jkl";

        // Verify
        sourceString.ToString().ShouldBe("abcdefghi");
        clone.ToString().ShouldBe("abcdefjkl");
    }
}
