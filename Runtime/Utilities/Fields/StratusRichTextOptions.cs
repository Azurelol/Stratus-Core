using Stratus.Extensions;

using System;

using UnityEngine;

namespace Stratus
{
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