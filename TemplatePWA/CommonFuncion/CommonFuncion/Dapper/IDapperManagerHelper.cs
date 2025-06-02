using CommonFuncion.Dapper.Interfaces;
using CommonFuncion.Results;

namespace CommonFuncion.Dapper
{
	public interface IDapperManagerHelper
	{
		Task<DataResult<TResult>> GetAsync<TResult>(
			string Script, string conectionString = null
		) where TResult : class, DbResult, new();

		Task<DataResult<TResult>> GetAsync<TResult, TRequest>(
			TRequest request, string? conectionString = null
		) where TResult : class, DbResult, new() where TRequest : SpRequest;

		Task<DataResult<IEnumerable<TResult>>> GetAllAsync<TResult>(
			string Script, string conectionString = null
		) where TResult : class, DbResult, new();

		Task<DataResult<IEnumerable<TResult>>> GetAllAsync<TResult, TRequest>(
			TRequest request, string? conectionString = null
		) where TResult : class, DbResult, new() where TRequest : SpRequest;

		Task<DataResult<TResult>> GetCombinedAsync<TResult, TRequest>(
			string connectionString, TRequest request
		) where TResult : class, DbResult, new() where TRequest : SpRequest;

		Task<DataResult<IEnumerable<TResult>>> GetAllCombinedAsync<TResult, TRequest>(
			string connectionString, TRequest request
		) where TResult : class, DbResult, new() where TRequest : SpRequest;
	}
}