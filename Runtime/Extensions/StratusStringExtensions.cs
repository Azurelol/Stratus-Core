using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

namespace Stratus
{
	public static partial class Extensions
	{
		//--------------------------------------------------------------------/
		// Fields
		//--------------------------------------------------------------------/
		public const char newlineChar = '\n';
		public static readonly string[] newlineSeparators = new string[] { $"{newlineChar}", Environment.NewLine };
		public const char whitespace = ' ';
		public const char underscore = '_';
		private static StringBuilder stringBuilder = new StringBuilder();

		//--------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------/
		/// <summary>
		/// Returns true if the string is null or empty
		/// </summary>
		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

		/// <summary>
		/// Returns true if the string is neither null or empty
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsValid(this string str)
		{
			return !str.IsNullOrEmpty();
		}

		/// <summary>
		/// Counts the number of lines in this string (by splitting it)
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static int CountLines(this string str, StringSplitOptions options = StringSplitOptions.None)
		{
			return str.Split(newlineSeparators, options).Length;
		}

		/// <summary>
		/// Appends a sequence of strings to the end of this string
		/// </summary>
		/// <param name="str"></param>
		/// <param name="sequence"></param>
		/// <returns></returns>
		public static string Append(this string str, IEnumerable<string> sequence)
		{
			StringBuilder builder = new StringBuilder(str);
			foreach (string item in sequence)
			{
				builder.Append(item);
			}
			return builder.ToString();
		}

		/// <summary>
		/// Appends the sequence to the given string
		/// </summary>
		/// <param name="str"></param>
		/// <param name="sequence"></param>
		/// <returns></returns>
		public static string Append(this string str, params string[] sequence)
		{
			return str.Append((IEnumerable<string>)sequence);
		}

		/// <summary>
		/// Appends all the lines to the given string
		/// </summary>
		/// <param name="str"></param>
		/// <param name="lines"></param>
		/// <returns></returns>
		public static string AppendLines(this string str, params string[] lines)
		{
			stringBuilder.Clear();

			stringBuilder.Append(str);
			if (lines.Length > 0)
			{
				lines.ForEach(x => stringBuilder.Append($"{Environment.NewLine}{x}"));
			}

			return stringBuilder.ToString();
		}

		/// <summary>
		/// Sorts the array using the default <see cref="Array.Sort(Array)"/>
		/// </summary>
		public static string[] ToSorted(this string[] source)
		{
			string[] destination = new string[source.Length];
			Array.Copy(source, destination, source.Length);
			Array.Sort(destination);
			return destination;
		}

		/// <summary>
		/// Strips all newlines in the string, replacing them with spaces
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ReplaceNewLines(this string str, string replacement)
		{
			return str.Replace("\n", replacement);
		}

		/// <summary>
		/// Uppercases the first character of this string
		/// </summary>
		public static string UpperFirst(this string input)
		{
			switch (input)
			{
				case null: throw new ArgumentNullException(nameof(input));
				case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
				default: return input.First().ToString().ToUpper() + input.Substring(1);
			}
		}

		/// <summary>
		/// Concatenates the elements of a specified array or the members of a collection, 
		/// using the specified separator between each element or member.
		/// </summary>
		public static string Join(this IEnumerable<string> str, string separator)
		{
			return string.Join(separator, str);
		}

		/// <summary>
		/// Concatenates the elements of a specified array or the members of a collection, 
		/// using the specified separator between each element or member.
		/// </summary>
		public static string Join(this IEnumerable<string> str, char separator)
		{
			return string.Join(separator.ToString(), str);
		}

		public static string Join(this string str, IEnumerable<string> values)
		{
			return string.Join(str, values);
		}

		public static string Join(this string str, params string[] values)
		{
			return string.Join(str, values);
		}

		/// <summary>
		/// Concatenates the elements of a specified array or the members of a collection, 
		/// using the newline separator between each element or member.
		/// </summary>
		public static string JoinLines(this IEnumerable<string> str)
		{
			return string.Join("\n", str);
		}

		/// <summary>
		/// Converts a string to camel case. eg: "Hello There" -> "helloThere"
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ToCamelCase(this string str)
		{
			if (!string.IsNullOrEmpty(str) && str.Length > 1)
			{
				return char.ToLowerInvariant(str[0]) + str.Substring(1);
			}
			return str;
		}

		/// <summary>
		/// Converts a string to title case. eg: "HelloThere" -> "Hello There")
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string ToTitleCase(this string input)
		{
			StringBuilder builder = new StringBuilder();
			bool previouslyUppercase = false;

			for (int i = 0; i < input.Length; i++)
			{
				char current = input[i];

				if ((current == underscore || current == whitespace)
					&& (i + 1 < input.Length))
				{
					if (i > 0)
					{
						builder.Append(whitespace);
					}

					char next = input[i + 1];
					if (char.IsLower(next))
					{
						next = char.ToUpper(next, CultureInfo.InvariantCulture);
					}
					builder.Append(next);
					i++;
				}
				else
				{
					// Special case for first char
					if (i == 0)
					{
						builder.Append(current.ToUpper());
						previouslyUppercase = true;
					}
					// Upper
					else if (current.IsUpper())
					{
						if (previouslyUppercase)
						{
							builder.Append(current.ToLower());
						}
						else
						{
							builder.Append(whitespace);
							builder.Append(current);
							previouslyUppercase = true;
						}

					}
					// Lower
					else
					{
						builder.Append(current);
						previouslyUppercase = false;
					}
				}
			}

			return builder.ToString();
		}

		/// <summary>
		/// Formats this string, applying rich text formatting to it
		/// </summary>
		public static string ToRichText(this string input, FontStyle style, string hexColor, int size = 0)
		{
			StringBuilder builder = new StringBuilder();

			switch (style)
			{
				case FontStyle.Normal:
					break;
				case FontStyle.Bold:
					builder.Append("<b>");
					break;
				case FontStyle.Italic:
					builder.Append("<i>");
					break;
				case FontStyle.BoldAndItalic:
					builder.Append("<b><i>");
					break;
			}

			bool applyColor = hexColor.IsValid();
			bool applySize = size > 0;
			if (applyColor)
			{
				builder.Append($"<color=#{hexColor}>");
			}
			if (applySize)
			{
				builder.Append($"<size={size}>");
			}
			builder.Append(input);
			if (applyColor)
			{
				builder.Append("</color>");
			}
			if (applySize)
			{
				builder.Append("</size>");
			}

			switch (style)
			{
				case FontStyle.Normal:
					break;
				case FontStyle.Bold:
					builder.Append("</b>");
					break;
				case FontStyle.Italic:
					builder.Append("</i>");
					break;
				case FontStyle.BoldAndItalic:
					builder.Append("</i></b>");
					break;
			}

			return builder.ToString();
		}


		/// <summary>
		/// Formats this string, applying rich text formatting to it
		/// </summary>
		public static string ToRichText(this string input, FontStyle style, Color color, int size = 0)
			=> input.ToRichText(style, color.ToHex(), size);

		/// <summary>
		/// Formats this string, applying rich text formatting to it
		/// </summary>
		public static string ToRichText(this string input, int size)
			=> input.ToRichText(FontStyle.Normal, null, size);

		/// <summary>
		/// Formats this string, applying rich text formatting to it
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string ToRichText(this string input, FontStyle style) => input.ToRichText(style, null, 0);

		/// <summary>
		/// Formats this string, applying rich text formatting to it
		/// </summary>
		public static string ToRichText(this string input, Color color) => input.ToRichText(FontStyle.Normal, color);

		/// <summary>
		/// Strips Unity's rich text from the given string
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string StripRichText(this string input)
		{
			const string pattern = @"</?(b|i|size(=?.*?)|color(=?.*?))>";
			return Regex.Replace(input, pattern, string.Empty);
		}

		/// <summary>
		/// If the string exceeds the given length, truncates any characters at the cutoff length,
		/// appending the replacement string to the end instead
		/// </summary>
		/// <param name="input"></param>
		/// <param name="length"></param>
		/// <param name="replacement"></param>
		/// <returns></returns>
		public static string Truncate(this string input, int length, string replacement = "...")
		{			
			if (input.Length > length)
			{
				input = $"{input.Substring(0, length)}{replacement}";
			}
			return input;
		}

		/// <summary>
		/// Removes null or empty strings from the sequence
		/// </summary>
		public static IEnumerable<string> TrimNullOrEmpty(this IEnumerable<string> sequence)
			=> sequence.TrimNullOrEmpty(null);

		/// <summary>
		/// Removes strings that are null, empty or that fail the predicate from the sequence
		/// </summary>
		public static IEnumerable<string> TrimNullOrEmpty(this IEnumerable<string> sequence, Predicate<string> predicate)
		{
			foreach (var item in sequence)
			{
				if (item.IsValid())
				{
					if (predicate != null && !predicate.Invoke(item))
					{
						continue;
					}
					yield return item;
				}
			}
		}

		/// <summary>
		/// Removes null or empty strings from the sequence
		/// </summary>
		public static string[] TrimNullOrEmpty(this string[] sequence) => ((IEnumerable<string>)sequence).TrimNullOrEmpty().ToArray();

		/// <summary>
		/// Removes strings that are null, empty or that fail the predicate from the sequence
		/// </summary>
		public static string[] TrimNullOrEmpty(this string[] sequence, Predicate<string> predicate) => ((IEnumerable<string>)sequence).TrimNullOrEmpty(predicate).ToArray();
	}

}