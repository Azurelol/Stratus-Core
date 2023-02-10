using NUnit.Framework;

using Stratus.Serialization;

using System;

namespace Stratus.Editor.Tests
{
	public class StratusPrefsVariableTests
	{
		const string key = nameof(StratusPrefsVariableTests);

		private class MockData
		{
			public int a;
			public string b;
		}

		[Test]
		public void TestPlayerPrefs()
		{
			StratusPlayerPrefsVariable variable;

			variable = new StratusPlayerPrefsVariable(key, StratusPrefsVariable.VariableType.Boolean);
			TestBoolean(variable);
			variable = new StratusPlayerPrefsVariable(key, StratusPrefsVariable.VariableType.Integer);
			TestInteger(variable);
			variable = new StratusPlayerPrefsVariable(key, StratusPrefsVariable.VariableType.String);
			TestString(variable);
			variable = new StratusPlayerPrefsVariable(key, StratusPrefsVariable.VariableType.Float);
			TestFloat(variable);
			variable = new StratusPlayerPrefsVariable(key, StratusPrefsVariable.VariableType.Object);
			TestObject(variable);
		}

		[Test]
		public void TestEditorPrefs()
		{
			StratusEditorPrefsVariable variable;

			variable = new StratusEditorPrefsVariable(key, StratusPrefsVariable.VariableType.Boolean);
			TestBoolean(variable);
			variable = new StratusEditorPrefsVariable(key, StratusPrefsVariable.VariableType.Integer);
			TestInteger(variable);
			variable = new StratusEditorPrefsVariable(key, StratusPrefsVariable.VariableType.String);
			TestString(variable);
			variable = new StratusEditorPrefsVariable(key, StratusPrefsVariable.VariableType.Float);
			TestFloat(variable);
			variable = new StratusEditorPrefsVariable(key, StratusPrefsVariable.VariableType.Object);
			TestObject(variable);
		}

		private void TestBoolean(StratusPrefsVariable prefsVariable)
		{
			prefsVariable.Set(true);
			Assert.AreEqual(true, prefsVariable.Get());

			prefsVariable.Set(false);
			Assert.AreEqual(false, prefsVariable.Get());

			prefsVariable.Delete();
			Assert.Catch(typeof(ArgumentException), () => prefsVariable.Set(3f));
			Assert.False(prefsVariable.Exists());
		}

		private void TestInteger(StratusPrefsVariable prefsVariable)
		{
			prefsVariable.Set(42);
			Assert.AreEqual(42, prefsVariable.Get());

			prefsVariable.Set(395);
			Assert.AreEqual(395, prefsVariable.Get());

			prefsVariable.Delete();
			Assert.Catch(typeof(ArgumentException), () => prefsVariable.Set("lol"));
			Assert.False(prefsVariable.Exists());
		}

		private void TestFloat(StratusPrefsVariable prefsVariable)
		{
			prefsVariable.Set(42f);
			Assert.AreEqual(42f, prefsVariable.Get());

			prefsVariable.Set(395f);
			Assert.AreEqual(395f, prefsVariable.Get());

			prefsVariable.Delete();
			Assert.False(prefsVariable.Exists());
			Assert.Catch(typeof(ArgumentException), () => prefsVariable.Set("lol"));
		}

		private void TestString(StratusPrefsVariable prefsVariable)
		{
			string a = "FOO", b = "BAR";

			prefsVariable.Set(a);
			Assert.AreEqual(a, prefsVariable.Get());

			prefsVariable.Set(b);
			Assert.AreEqual(b, prefsVariable.Get());

			prefsVariable.Delete();
			Assert.False(prefsVariable.Exists());
			Assert.Catch(typeof(ArgumentException), () => prefsVariable.Set(3f));
		}

		private void TestObject(StratusPrefsVariable prefsVariable)
		{
			MockData input = new MockData()
			{
				a = 7,
				b = "lol"
			};

			prefsVariable.Set(input);

			MockData output = prefsVariable.Get<MockData>();
			Assert.AreEqual(input.a, output.a);
			Assert.AreEqual(input.b, output.b);
			prefsVariable.Delete();
			Assert.False(prefsVariable.Exists());
		}

	}

}