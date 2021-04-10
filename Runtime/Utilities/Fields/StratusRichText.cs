using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	[Serializable]
	public class StratusRichText
	{
		[SerializeField]
		private string _text;
		[SerializeField]
		private StratusRichTextOptions _options;

		/// <summary>
		/// The raw, unformatted text
		/// </summary>
		public string text => _text;

		/// <summary>
		/// The formatted text
		/// </summary>
		public string richText
		{
			get
			{
				if (!generated)
				{
					_richText = text.ToRichText(_options);
					generated = true;
				}
				return _richText;
			}
		}
		private string _richText;
		private bool generated;

		public StratusRichText()
		{
		}

		public StratusRichText(string text, StratusRichTextOptions options)
		{
			this._text = text;
			this._options = options;
		}

		public StratusRichText(string text)
		{
			this._text = text;
			this._options = default;
		}

		public override string ToString() => text;

		public static implicit operator string(StratusRichText richText) => richText.text;
		public static implicit operator StratusRichText(string text) => new StratusRichText(text);
	}

	[Serializable]
	public struct StratusRichTextOptions
	{
		public FontStyle style;
		public int size;

		[SerializeField]
		private Color color;

		public string hexColor
		{
			get
			{
				if (_hexColor == null && color != default)
				{
					_hexColor = color.ToHex();
				}
				return _hexColor;
			}
		}
		[NonSerialized]
		private string _hexColor;

		private static Lazy<StratusRichTextOptions> _default = new Lazy<StratusRichTextOptions>(() => new StratusRichTextOptions());

		public StratusRichTextOptions(FontStyle style, string hexColor, int size)
		{
			this.style = style;
			this._hexColor = hexColor;
			this.color = default;
			this.size = size;
		}

		public StratusRichTextOptions(FontStyle style, Color color, int size)
			: this(style, color.ToHex(), size)
		{
		}

		public static StratusRichTextOptions Default() => _default.Value;
	}

}