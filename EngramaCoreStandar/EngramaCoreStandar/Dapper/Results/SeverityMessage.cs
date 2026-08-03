using System;
using System.Collections.Generic;

namespace EngramaCoreStandar.Dapper.Results
{

	public class SeverityMessage : GenericResponse
	{
		public string Position { get; }
		public SeverityTag Severity { get; }

		public SeverityMessage(bool bContinueProcces,
							string vchMessage,
							SeverityTag severity
							)
		: base(bContinueProcces, vchMessage)
		{

			Severity = severity;
		}

		public SeverityMessage(bool bResult, string vchMessage) : base(bResult, vchMessage)
		{
		}

		public static implicit operator List<object>(SeverityMessage v)
		{
			throw new NotImplementedException();
		}
	}

	public enum SeverityTag
	{
		Success,
		Warning,
		Error,
		Info
	}
}
