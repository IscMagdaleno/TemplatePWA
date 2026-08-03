using EngramaCoreStandar.Dapper.Results;
using EngramaCoreStandar.Results;

using System;

namespace EngramaCoreStandar.Servicios
{
	public class ValidaServicioService : IValidaServicioService
	{



		public SeverityMessage ValidadionServicio<T>(
			HttpResponseWrapper<Response<T>> response,
			Action<T> onSuccess = null,
			bool ContinueWarning = true,
			bool ContinueError = false)
		where T : new()
		{
			if (response.Success)
			{
				if (response.Response.IsSuccess)
				{
					onSuccess?.Invoke(response.Response.Data);

					return new SeverityMessage(
						true,
						response.Response.Message,
						SeverityTag.Success
					);
				}
				return new SeverityMessage(
					ContinueWarning,
					response.Response.Message,
					SeverityTag.Warning);
			}
			return new SeverityMessage(
				ContinueError,
				response.HttpResponseMessage.ToString(),
				SeverityTag.Error);

		}

	}

}
