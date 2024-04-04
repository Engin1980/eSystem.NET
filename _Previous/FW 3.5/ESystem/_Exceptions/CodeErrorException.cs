using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace ESystem
{

	/// <summary>
	/// Represents error in application, which is caused by fail in code and should not be raised in final application.
	/// </summary>
	/// <remarks></remarks>
	public class CodeErrorException : ApplicationException
	{

		public CodeErrorException() : this(null, null)
		{
		}
		public CodeErrorException(string description) : this(description, null)
		{
		}
		public CodeErrorException(string description, Exception innerException) : base("Invalid state of application. Programmer's error detected. More info: " + description, innerException)
		{
		}

	}

}
