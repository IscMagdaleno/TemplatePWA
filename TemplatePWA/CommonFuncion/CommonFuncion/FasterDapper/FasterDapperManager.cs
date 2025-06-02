using CommonFuncion.FasterDapper.Extensions;

using Dapper;

using FasterDapper.Core.DapperManager;

using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using static Dapper.SqlMapper;

namespace CommonFuncion.FasterDapper
{
	public class FasterDapperManager : IFasterDapperManager, IDisposable
	{
		private readonly string ConnectionString;

		public FasterDapperManager(string connectionString)
		{
			ConnectionString = connectionString;
		}

		public TResult Get<TResult>(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		)
		{
			using SqlConnection sqlConnection = new(ConnectionString);

			TResult result = sqlConnection.QueryFirst<TResult>(
				storeProcedure, dynamicParameters, commandTimeout: 0, commandType: CommandType.StoredProcedure
			);

			SqlConnection.ClearPool(sqlConnection);

			return result;
		}

		public IEnumerable<TResult> GetAll<TResult>(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		)
		{
			using SqlConnection sqlConnection = new(ConnectionString);

			IEnumerable<TResult> result = sqlConnection.Query<TResult>(
				storeProcedure, dynamicParameters, commandTimeout: 0, commandType: CommandType.StoredProcedure
			);

			SqlConnection.ClearPool(sqlConnection);

			return result;
		}

		public async Task<IEnumerable<TResult>> GetAllAsync<TResult>(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		)
		{
			using SqlConnection sqlConnection = new(ConnectionString);

			IEnumerable<TResult> result = await sqlConnection.QueryAsync<TResult>(
				storeProcedure, dynamicParameters, commandTimeout: 0, commandType: CommandType.StoredProcedure
			);

			SqlConnection.ClearPool(sqlConnection);

			return result;
		}

		public async Task<TResult> GetAsync<TResult>(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		)
		{
			using SqlConnection sqlConnection = new(ConnectionString);

			TResult result = await sqlConnection.QueryFirstAsync<TResult>(
				storeProcedure, dynamicParameters, commandTimeout: 0, commandType: CommandType.StoredProcedure
			);

			SqlConnection.ClearPool(sqlConnection);

			return result;
		}

		public TResult GetCombined<TResult>(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		)
		{
			var result = Activator.CreateInstance<TResult>();

			using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
			{
				using SqlCommand command = new SqlCommand(storeProcedure, sqlConnection);
				SqlParameter[] sqlParameters = dynamicParameters.ParseSqlParameters();

				command.Parameters.AddRange(sqlParameters);
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = 0;

				try
				{
					sqlConnection.Open();

					result = command.GetResults<TResult>().FirstOrDefault();
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.StackTrace);
				}
				finally
				{
					sqlConnection.Close();
					SqlConnection.ClearPool(sqlConnection);
				}
			}

			return result;
		}

		public async Task<TResult> GetCombinedAsync<TResult>(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		)
		{
			var result = Activator.CreateInstance<TResult>();

			using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
			{
				using SqlCommand command = new SqlCommand(storeProcedure, sqlConnection);
				SqlParameter[] sqlParameters = dynamicParameters.ParseSqlParameters();

				command.Parameters.AddRange(sqlParameters);
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = 0;

				try
				{
					sqlConnection.Open();

					result = (await command.GetResultsAsync<TResult>()).FirstOrDefault();
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.StackTrace);
				}
				finally
				{
					sqlConnection.Close();
					SqlConnection.ClearPool(sqlConnection);
				}
			}

			return result;
		}

		public IEnumerable<TResult> GetAllCombined<TResult>(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		)
		{
			IEnumerable<TResult> result = new List<TResult>();

			using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
			{
				using SqlCommand command = new SqlCommand(storeProcedure, sqlConnection);
				SqlParameter[] sqlParameters = dynamicParameters.ParseSqlParameters();

				command.Parameters.AddRange(sqlParameters);
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = 0;

				try
				{
					sqlConnection.Open();

					result = command.GetResults<TResult>();
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.StackTrace);
				}
				finally
				{
					sqlConnection.Close();
					SqlConnection.ClearPool(sqlConnection);
				}
			}

			return result;
		}

		public async Task<IEnumerable<TResult>> GetAllCombinedAsync<TResult>(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		)
		{
			IEnumerable<TResult> result = new List<TResult>();

			using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
			{
				using SqlCommand command = new SqlCommand(storeProcedure, sqlConnection);
				SqlParameter[] sqlParameters = dynamicParameters.ParseSqlParameters();

				command.Parameters.AddRange(sqlParameters);
				command.CommandType = CommandType.StoredProcedure;
				command.CommandTimeout = 0;

				try
				{
					sqlConnection.Open();

					result = await command.GetResultsAsync<TResult>();
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.StackTrace);
				}
				finally
				{
					sqlConnection.Close();
					SqlConnection.ClearPool(sqlConnection);
				}
			}

			return result;
		}

		public TResult GetQuery<TResult>(
			string query, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		)
		{
			using SqlConnection sqlConnection = new(ConnectionString);

			TResult result = sqlConnection.QueryFirst<TResult>(
				query, commandTimeout: 0, commandType: CommandType.Text
			);

			SqlConnection.ClearPool(sqlConnection);

			return result;
		}

		public async Task<TResult> GetQueryAsync<TResult>(
			string query, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		)
		{
			using SqlConnection sqlConnection = new(ConnectionString);

			TResult result = await sqlConnection.QueryFirstAsync<TResult>(
				query, commandTimeout: 0, commandType: CommandType.Text
			);

			SqlConnection.ClearPool(sqlConnection);

			return result;
		}

		public IEnumerable<TResult> GetAllQuery<TResult>(
			string query, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		)
		{
			using SqlConnection sqlConnection = new(ConnectionString);

			IEnumerable<TResult> result = sqlConnection.Query<TResult>(
				query, commandTimeout: 0, commandType: CommandType.Text
			);

			SqlConnection.ClearPool(sqlConnection);

			return result;
		}

		public async Task<IEnumerable<TResult>> GetAllQueryAsync<TResult>(
			string query, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		)
		{
			using SqlConnection sqlConnection = new(ConnectionString);

			IEnumerable<TResult> result = await sqlConnection.QueryAsync<TResult>(
				query, commandTimeout: 0, commandType: CommandType.Text
			);

			SqlConnection.ClearPool(sqlConnection);

			return result;
		}

		public IEnumerable<IEnumerable<dynamic>> GetQueryMultiple(
			string query, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		)
		{
			using SqlConnection sqlConnection = new(ConnectionString);
			List<IEnumerable<dynamic>> resultSets = new();

			using GridReader gridReader = sqlConnection.QueryMultiple(
				query, commandTimeout: 0, commandType: CommandType.Text
			);

			while (!gridReader.IsConsumed)
			{
				resultSets.Add(gridReader.Read());
			}

			SqlConnection.ClearPool(sqlConnection);

			return resultSets;
		}

		public async Task<IEnumerable<IEnumerable<dynamic>>> GetQueryMultipleAsync(
			string query, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		)
		{
			using SqlConnection sqlConnection = new(ConnectionString);
			List<IEnumerable<dynamic>> resultSets = new();

			using GridReader gridReader = await sqlConnection.QueryMultipleAsync(
				query, commandTimeout: 0, commandType: CommandType.Text
			);

			while (!gridReader.IsConsumed)
			{
				resultSets.Add(gridReader.Read());
			}

			SqlConnection.ClearPool(sqlConnection);

			return resultSets;
		}

		public IEnumerable<IEnumerable<dynamic>> GetQueryMultiple(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		)
		{
			using SqlConnection sqlConnection = new(ConnectionString);
			List<IEnumerable<dynamic>> resultSets = new();

			GridReader gridReader = sqlConnection.QueryMultiple(
				storeProcedure, dynamicParameters, commandTimeout: 0, commandType: CommandType.StoredProcedure
			);

			while (!gridReader.IsConsumed)
			{
				resultSets.Add(gridReader.Read());
			}

			SqlConnection.ClearPool(sqlConnection);

			return resultSets;
		}

		public async Task<IEnumerable<IEnumerable<dynamic>>> GetQueryMultipleAsync(
			DynamicParameters dynamicParameters, string storeProcedure, [CallerFilePath] string path = null, [CallerMemberName] string method = null
		)
		{
			using SqlConnection sqlConnection = new(ConnectionString);
			List<IEnumerable<dynamic>> resultSets = new();

			GridReader gridReader = await sqlConnection.QueryMultipleAsync(
				storeProcedure, dynamicParameters, commandTimeout: 0, commandType: CommandType.StoredProcedure
			);

			while (!gridReader.IsConsumed)
			{
				resultSets.Add(gridReader.Read());
			}

			SqlConnection.ClearPool(sqlConnection);

			return resultSets;
		}

		public void Dispose() { }
	}
}