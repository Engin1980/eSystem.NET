using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace ESystem
{

	/// <summary>
	/// Exception raised as typed application exception.
	/// </summary>
	/// <remarks></remarks>
	public class ProgramException : ApplicationException
	{

		public ProgramException(string message) : base(message)
		{
		}

	}

}
