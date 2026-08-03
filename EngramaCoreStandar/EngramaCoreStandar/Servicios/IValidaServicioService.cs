using EngramaCoreStandar.Dapper.Results;
using EngramaCoreStandar.Results;

using System;

namespace EngramaCoreStandar.Servicios
{
	public interface IValidaServicioService
	{
		SeverityMessage ValidadionServicio<T>(HttpResponseWrapper<Response<T>> response, Action<T> onSuccess = null, bool ContinueWarning = true, bool ContinueError = false) where T : new();

	}
}
