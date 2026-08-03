using Dapper;

using Microsoft.Data.SqlClient;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace EngramaCoreStandar.Dapper
{

	public class DapperManager : IDapperManager
	{


		public DapperManager()
		{
		}



		public async Task<T> GetAsync<T>(string query, string connectionString)
		{
			try
			{
				await using var db = new SqlConnection(connectionString);
				await db.OpenAsync();

				return await db.QueryFirstOrDefaultAsync<T>(new CommandDefinition(query, null, commandTimeout: 250, commandType: CommandType.Text));
			}
			catch (SqlException ex)
			{
				Console.WriteLine("An error occurred while executing the query: {Query}" + ex.Message);
				throw;
			}
		}

		public async Task<IEnumerable<T>> GetAllAsync<T>(string query, string connectionString)
		{
			try
			{
				await using var db = new SqlConnection(connectionString);
				await db.OpenAsync();

				return await db.QueryAsync<T>(new CommandDefinition(query, null, commandTimeout: 250, commandType: CommandType.Text));
			}
			catch (SqlException ex)
			{
				Console.WriteLine("An error occurred while executing the query: {Query}" + ex.Message);
				throw;
			}
		}

		public async Task<T> GetAsync<T>(string storedProcedure, DynamicParameters parameters, string connectionString)
		{
			try
			{
				await using var db = new SqlConnection(connectionString);
				await db.OpenAsync();

				return (await db.QueryAsync<T>(storedProcedure, parameters, commandTimeout: 250, commandType: CommandType.StoredProcedure)).FirstOrDefault();
			}
			catch (SqlException ex)
			{
				Console.WriteLine("An error occurred while executing the query: {Query}" + ex.Message);
				throw;
			}
		}

		public async Task<IEnumerable<T>> GetAllAsync<T>(string storedProcedure, DynamicParameters parameters, string connectionString)
		{
			try
			{
				await using var db = new SqlConnection(connectionString);
				await db.OpenAsync();

				return await db.QueryAsync<T>(storedProcedure, parameters, commandTimeout: 250, commandType: CommandType.StoredProcedure);
			}
			catch (SqlException ex)
			{
				Console.WriteLine("An error occurred while executing the query: {Query}" + ex.Message);
				throw;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// Release managed resources if any
			}
		}
	}
}