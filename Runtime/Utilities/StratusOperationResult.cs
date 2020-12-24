using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Stratus
{
	/// <summary>
	/// The result of an operation or procedure
	/// </summary>
	public class StratusOperationResult
	{
		public bool valid { get; protected set; }
		public string message { get; protected set; }

		public StratusOperationResult(bool valid, string message)
			: this(valid)
		{
			this.message = message;
		}

		public StratusOperationResult(bool valid)
		{
			this.valid = valid;
		}

		public StratusOperationResult(Exception exception)
		{
			this.valid = false;
			this.message = exception.Message;
		}

		public override string ToString()
		{
			return message != null ? $"{valid} ({message})" : $"{valid}";
		}

		public static implicit operator bool(StratusOperationResult validation) => validation.valid;
		public static implicit operator StratusOperationResult(bool valid) => new StratusOperationResult(valid, null);
	}

	/// <summary>
	/// The result of an operation which returns a value
	/// </summary>
	public class StratusOperationResult<T> : StratusOperationResult
		where T : class
	{
		public T value { get; private set; }

		public StratusOperationResult(bool valid, T value) : base(valid)
		{
			this.value = value;
		}

		public StratusOperationResult(Exception exception) : base(exception)
		{
		}

		public StratusOperationResult(bool valid, string message) : base(valid, message)
		{
		}

		public StratusOperationResult(T value, bool valid, string message) : this(valid, message)
		{
			this.value = value;
		}

		public static implicit operator StratusOperationResult<T>(T value) => new StratusOperationResult<T>(true, value);
	}
}