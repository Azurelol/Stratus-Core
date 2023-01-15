using Stratus.Extensions;

using System;

using UnityEngine;

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

}