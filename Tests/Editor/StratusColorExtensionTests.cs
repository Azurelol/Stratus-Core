using NUnit.Framework;

using Stratus.Extensions;

using UnityEngine;

namespace Stratus.Editor.Tests
{
	public class StratusColorExtensionTests
	{
		public void TestScaleAlpha()
		{
			Color color = Color.red;
			Assert.AreEqual(new Color(1, 0, 0, 0.7f), color.ScaleAlpha(0.7f));
			Assert.AreEqual(new Color(1, 0, 0, 0.5f), color.ScaleAlpha(0.5f));
			Assert.AreEqual(new Color(1, 0, 0, 1f), color.ScaleAlpha(1f));
		}

		[TestCase(1, 0, 0, 1)]
		[TestCase(1, 1, 0, 1)]
		[TestCase(1, 1, 1, 1)]
		[TestCase(1, 0.5f, 1, 0.5f)]
		public void TestHex(float r, float g, float b, float a)
		{
			Color color = new Color(r, g, b, a);
			Assert.AreEqual(ColorUtility.ToHtmlStringRGBA(color), color.ToHex());
		}
	}


}