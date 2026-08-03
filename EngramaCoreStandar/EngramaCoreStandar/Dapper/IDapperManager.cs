

using Dapper;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EngramaCoreStandar.Dapper
{
	public interface IDapperManager : IDisposable
	{
		Task<IEnumerable<T>> GetAllAsync<T>(string query, string connectionString);
		Task<IEnumerable<T>> GetAllAsync<T>(string storedProcedure, DynamicParameters parameters, string connectionString);
		Task<T> GetAsync<T>(string query, string connectionString);
		Task<T> GetAsync<T>(string storedProcedure, DynamicParameters parameters, string connectionString);
	}
}
