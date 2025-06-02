using Dapper;

using System.Runtime.CompilerServices;

namespace FasterDapper.Core.DapperManager
{
	public interface IFasterDapperManager
	{
		public TResult Get<TResult>(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		);

		public Task<TResult> GetAsync<TResult>(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		);

		public IEnumerable<TResult> GetAll<TResult>(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		);

		public Task<IEnumerable<TResult>> GetAllAsync<TResult>(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		);

		public TResult GetCombined<TResult>(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		);

		public Task<TResult> GetCombinedAsync<TResult>(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		);

		public IEnumerable<TResult> GetAllCombined<TResult>(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		);

		public Task<IEnumerable<TResult>> GetAllCombinedAsync<TResult>(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		);

		public TResult GetQuery<TResult>(
			string query, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		);

		public Task<TResult> GetQueryAsync<TResult>(
			string query, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		);

		public IEnumerable<TResult> GetAllQuery<TResult>(
			string query, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		);

		public Task<IEnumerable<TResult>> GetAllQueryAsync<TResult>(
			string query, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		);

		public IEnumerable<IEnumerable<dynamic>> GetQueryMultiple(
			string query, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		);

		public Task<IEnumerable<IEnumerable<dynamic>>> GetQueryMultipleAsync(
			string query, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		);

		public IEnumerable<IEnumerable<dynamic>> GetQueryMultiple(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		);

		public Task<IEnumerable<IEnumerable<dynamic>>> GetQueryMultipleAsync(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		);
	}
}