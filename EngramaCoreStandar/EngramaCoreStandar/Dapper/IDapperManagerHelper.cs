using EngramaCoreStandar.Dapper.Interfaces;
using EngramaCoreStandar.Results;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace EngramaCoreStandar.Dapper
{
	public interface IDapperManagerHelper
	{


		Task<DataResult<TResult>> GetAsync<TResult>(string Script, string connectionString = null, string shema = "dbo") where TResult : class, DbResult, new();
		Task<DataResult<IEnumerable<TResult>>> GetAllAsync<TResult>(string script, string? connectionString = null, string shema = "dbo") where TResult : class, DbResult, new();


		Task<DataResult<TResult>> GetAsync<TResult, TRequest>(TRequest request, string? connectionString = null, string shema = "dbo")
			where TResult : class, DbResult, new()
			where TRequest : SpRequest;
		Task<DataResult<IEnumerable<TResult>>> GetAllAsync<TResult, TRequest>(TRequest request, string? connectionString = null, string shema = "dbo")
			where TResult : class, DbResult, new()
			where TRequest : SpRequest;


		Task<DataResult<TResult>> GetWithListAsync<TResult, TRequest>(TRequest request, string? connectionString = null, string shema = "dbo")
			where TRequest : SpRequest
			where TResult : class, DbResult, new();

		Task<DataResult<IEnumerable<TResult>>> GetAllWithListAsync<TResult, TRequest>(TRequest request, string? connectionString = null, string shema = "dbo")
			where TResult : class, DbResult, new()
			where TRequest : SpRequest;
	}
}
