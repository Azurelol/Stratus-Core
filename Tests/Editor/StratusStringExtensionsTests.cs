using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NUnit.Framework;

using UnityEngine;

namespace Stratus.Tests
{
	public class StratusStringExtensionsTests
	{
		[Test]
		public void TestEnclosure()
		{
			string input = "foo";
			Assert.AreEqual("(foo)", input.Enclose(StratusStringEnclosure.Parenthesis));
			Assert.AreEqual("[foo]", input.Enclose(StratusStringEnclosure.SquareBracket));
			Assert.AreEqual("{foo}", input.Enclose(StratusStringEnclosure.CurlyBracket));
			Assert.AreEqual("<foo>", input.Enclose(StratusStringEnclosure.AngleBracket));
			Assert.AreEqual("\"foo\"", input.Enclose(StratusStringEnclosure.DoubleQuote));
			Assert.AreEqual("'foo'", input.Enclose(StratusStringEnclosure.Quote));
		}

		[Test]
		public void TestNullOrEmpty()
		{
			string value = null;
			Assert.True(value.IsNullOrEmpty());
			value = string.Empty;
			Assert.True(value.IsNullOrEmpty());
			value = "Boo!";
			Assert.True(value.IsValid());
			value = "";
			Assert.False(value.IsValid());
			value = null;
			Assert.False(value.IsValid());
		}

		[Test]
		public void TestRichText()
		{
			string input = "Boo";
			Assert.AreEqual($"<b>Boo</b>", input.ToRichText(FontStyle.Bold));
			Assert.AreEqual($"<i>Boo</i>", input.ToRichText(FontStyle.Italic));
			Assert.AreEqual($"<b><i>Boo</i></b>", input.ToRichText(FontStyle.BoldAndItalic));

			Color inputColor = Color.red;
			Assert.AreEqual($"<b><color=#{inputColor.ToHex()}>Boo</color></b>", input.ToRichText(FontStyle.Bold, inputColor));

			string cleanText = "Hello there!";
			void compareCleanText()
			{
				input = input.StripRichText();
				Assert.AreEqual(input, cleanText);
			}

			input = cleanText.ToRichText(FontStyle.Italic);
			compareCleanText();

			input = cleanText.ToRichText(FontStyle.BoldAndItalic);
			compareCleanText();

			input = cleanText.ToRichText(FontStyle.Bold);
			compareCleanText();

			input = cleanText.ToRichText(Color.green);
			compareCleanText();

			input = cleanText.ToRichText(34);
			compareCleanText();
		}

		[Test]
		public void TestJoin()
		{
			string[] values = new string[]
			{
				"A",
				"B",
				"C"
			};
			Assert.AreEqual("A B C", values.Join(" "));
			Assert.AreEqual("A,B,C", values.Join(","));
			Assert.AreEqual("A\nB\nC", values.JoinLines());


		}
		
		[Test]
		public void TestAppend()
		{
			string cat = "cat", dog = "dog", bird = "bird";
			Assert.AreEqual($"{dog}{Environment.NewLine}{cat}", dog.AppendLines(cat));
			Assert.AreEqual($"{dog}{Environment.NewLine}{cat}{Environment.NewLine}{bird}", dog.AppendLines(cat, bird));
			Assert.AreEqual($"{dog}{bird}{cat}", dog.Append(bird, cat));
		}

		[Test]
		public void TestTrim()
		{
			string predicatedString = "Waaagh";
			string[] input = new string[]
			{
				"Foo",
				"Bar",
				"",
				predicatedString,
				null,
				"Ya!"
			};

			string[] output;
			output = input.TrimNullOrEmpty();
			Assert.AreEqual(4, output.Length);
			output = input.TrimNullOrEmpty(x => !x.Contains(predicatedString));
			Assert.AreEqual(3, output.Length);
			Assert.False(output.Contains(predicatedString));
		}

		[Test]
		public void TestCase()
		{
			// Title Case
			{
				void TestTitleCase(string value, string expected) => Assert.AreEqual(expected, value.ToTitleCase());
				TestTitleCase("COOL_MEMBER_NAME", "Cool Member Name");
				TestTitleCase("war and peace", "War And Peace");
				TestTitleCase("cool_class_name", "Cool Class Name");
				TestTitleCase("_cool_class_name", "Cool Class Name");
				TestTitleCase("_coolClassName", "Cool Class Name");
			}

			// Upper First
			{
				string value = "cat";
				Assert.AreEqual("Cat", value.UpperFirst());
			}
		}
		
		[Test]
		public void TestTruncation()
		{
			string input = "Hello there brown cow";
			
			string text;
			text = input.Truncate(5);
			Assert.AreEqual("Hello...", text);

			text = input.Truncate(11);
			Assert.AreEqual("Hello there...", text);

			text = input.Truncate(input.Length);
			Assert.AreEqual(input, text);

			text = input.Truncate(5, "!!!");
			Assert.AreEqual("Hello!!!", text);
		}

		[Test]
		public void TestLines()
		{
			// Count Lines, Trim Lines
			{
				string value = "hello\nthere\ncat";
				Assert.AreEqual(3, value.CountLines());
				value = value.ReplaceNewLines(" ");
				Assert.AreEqual("hello there cat", value);
			}
		}

		[Test]
		public void TestSort()
		{
			string a = "a", b = "b", c = "c";
			string[] input = new string[]
			{
				a, c, b
			};
			string[] output = input.ToSorted();
			Assert.AreEqual(output[0], a);
			Assert.AreEqual(output[1], b);
			Assert.AreEqual(output[2], c);
		}
	}

}