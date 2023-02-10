using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using Stratus;

namespace Stratus.Extensions
{
	public static partial class StringExtensions
	{
		//--------------------------------------------------------------------/
		// Fields
		//--------------------------------------------------------------------/
		public const char newlineChar = '\n';
		public const string newlineString = "\n";
		public static readonly string[] newlineSeparators = new string[]
		{
			"\r\n",
			"\n",
			"\r",
		};
		public const char whitespace = ' ';
		public const char underscore = '_';
		private static StringBuilder stringBuilder = new StringBuilder();

		//--------------------------------------------------------------------/
		// Methods
		//--------------------------------------------------------------------/
		/// <summary>
		/// Returns true if the rich text is null or empty
		/// </summary>
		public static bool IsNullOrEmpty(this StratusRichText richText)
		{
			return richText == null || string.IsNullOrEmpty(richText.text);
		}

		/// <summary>
		/// Returns true if the richtext is neither null or empty
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static bool IsValid(this StratusRichText str)
		{
			return !str.IsNullOrEmpty();
		}

		/// <summary>
		/// Formats this string, applying rich text formatting to it
		/// </summary>
		public static string ToRichText(this string input, FontStyle style, string hexColor, int size = 0)
			=> input.ToRichText(new StratusRichTextOptions(style, hexColor, size));

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

		public static string ToRichText(this string input, StratusRichTextOptions options)
		{
			StringBuilder builder = new StringBuilder();

			switch (options.style)
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

			bool applyColor = options.hexColor.IsValid();
			bool applySize = options.size > 0;
			if (applyColor)
			{
				builder.Append($"<color=#{options.hexColor}>");
			}
			if (applySize)
			{
				builder.Append($"<size={options.size}>");
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

			switch (options.style)
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
	}
}