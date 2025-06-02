using CommonFuncion.Dapper.Interfaces;
using CommonFuncion.Extensions;
using CommonFuncion.FasterDapper;
using CommonFuncion.FasterDapper.Extensions;
using CommonFuncion.Logger;
using CommonFuncion.Results;

using Dapper;

using Microsoft.Extensions.Configuration;

using System.Data.SqlClient;
using System.Reflection;

namespace CommonFuncion.Dapper
{
	/// <summary>
	/// Auxiliar para el uso de dapper al convertir los modelos ya en la lista de parámetros 
	/// </summary>
	public class DapperMangerHelper : IDapperManagerHelper
	{
		private ILoggerHelper LoggerHelper { get; }

		private IConfiguration Configuration { get; }

		private FasterDapperManager FasterDapperManager { get; }

		public string ConnectionString { get; }

		public DapperMangerHelper(ILoggerHelper loggerHelper, IConfiguration configuration)
		{
			ConnectionString = configuration.GetConnectionString("EngramaCloudConnection");
			LoggerHelper = loggerHelper;

			FasterDapperManager = new FasterDapperManager(ConnectionString);
		}

		#region Gets from script

		public async Task<DataResult<TResult>> GetAsync<TResult>(
			string script
		) where TResult : class, DbResult, new()
		{
			DataResult<TResult> result = new();

			try
			{
				TResult data = EvaluateObject(
					await FasterDapperManager.GetQueryAsync<TResult>(script)
				);

				LoggerHelper.ValidateSPResult(data);

				result = DataResult<TResult>.Success(data);
			}
			catch (Exception ex)
			{
				if (ex is SqlException sqlEx)
				{
					result = DataResult<TResult>.Fail(script, sqlEx);
				}
				else
				{
					result = DataResult<TResult>.Fail(script);
				}
			}

			LoggerHelper.ValidateResult(result);

			return result;
		}

		public async Task<DataResult<TResult>> GetAsync<TResult>(
			string script, string connectioString
		) where TResult : class, DbResult, new()
		{
			DataResult<TResult> result = new();

			try
			{
				TResult data = EvaluateObject(
					await FasterDapperManager.GetQueryAsync<TResult>(script)
				);

				LoggerHelper.ValidateSPResult(data);

				result = DataResult<TResult>.Success(data);
			}
			catch (Exception ex)
			{
				if (ex is SqlException sqlEx)
				{
					result = DataResult<TResult>.Fail(script, sqlEx);
				}
				else
				{
					result = DataResult<TResult>.Fail(script);
				}
			}

			LoggerHelper.ValidateResult(result);

			return result;
		}

		public async Task<DataResult<IEnumerable<TResult>>> GetAllAsync<TResult>(
			string script
		) where TResult : class, DbResult, new()
		{
			DataResult<IEnumerable<TResult>> result = new();

			try
			{
				IEnumerable<TResult> data = EvaluateEnumerable(
					await FasterDapperManager.GetAllQueryAsync<TResult>(script)
				);

				result = DataResult<IEnumerable<TResult>>.Success(data);
			}
			catch (Exception ex)
			{
				if (ex is SqlException sqlEx)
				{
					result = DataResult<IEnumerable<TResult>>.Fail(script, sqlEx);
				}
				else
				{
					result = DataResult<IEnumerable<TResult>>.Fail(script);
				}
			}

			LoggerHelper.ValidateResult(result);

			return result;

		}

		public async Task<DataResult<IEnumerable<TResult>>> GetAllAsync<TResult>(
			string script, string connectioString
		) where TResult : class, DbResult, new()
		{
			DataResult<IEnumerable<TResult>> result = new();

			try
			{
				IEnumerable<TResult> data = EvaluateEnumerable(
					await FasterDapperManager.GetAllQueryAsync<TResult>(script)
				);

				result = DataResult<IEnumerable<TResult>>.Success(data);
			}
			catch (Exception ex)
			{
				if (ex is SqlException sqlEx)
				{
					result = DataResult<IEnumerable<TResult>>.Fail(script, sqlEx);
				}
				else
				{
					result = DataResult<IEnumerable<TResult>>.Fail(script);
				}
			}

			LoggerHelper.ValidateResult(result);

			return result;

		}

		#endregion Gets from script

		#region Gets from TRequest

		public async Task<DataResult<TResult>> GetAsync<TResult, TRequest>(
			TRequest request
		) where TRequest : SpRequest where TResult : class, DbResult, new()
		{
			DataResult<TResult> result = new();
			string storedProcedure = $"[dbo].[{request.StoredProcedure}]";
			string executableString = ExecutableString(request, storedProcedure);

			try
			{
				DynamicParameters dynamicParameters = request.ParseDynamicParameters();

				TResult data = EvaluateObject(
					await FasterDapperManager.GetAsync<TResult>(dynamicParameters, storedProcedure)
				);

				result = DataResult<TResult>.Success(data);
			}
			catch (Exception ex)
			{
				if (ex is SqlException sqlEx)
				{
					result = DataResult<TResult>.Fail(executableString, sqlEx);
				}
				else
				{
					result = DataResult<TResult>.Fail(executableString);
				}
			}

			LoggerHelper.ValidateResult(result);

			return result;
		}

		public async Task<DataResult<TResult>> GetAsync<TResult, TRequest>(
			TRequest request, string connectioString
		) where TRequest : SpRequest where TResult : class, DbResult, new()
		{
			DataResult<TResult> result = new();
			string storedProcedure = $"[dbo].[{request.StoredProcedure}]";
			string executableString = ExecutableString(request, storedProcedure);

			try
			{
				DynamicParameters dynamicParameters = request.ParseDynamicParameters();

				TResult data = EvaluateObject(
					await FasterDapperManager.GetAsync<TResult>(dynamicParameters, storedProcedure)
				);

				result = DataResult<TResult>.Success(data);
			}
			catch (Exception ex)
			{
				if (ex is SqlException sqlEx)
				{
					result = DataResult<TResult>.Fail(executableString, sqlEx);
				}
				else
				{
					result = DataResult<TResult>.Fail(executableString);
				}
			}

			LoggerHelper.ValidateResult(result);

			return result;
		}

		public async Task<DataResult<IEnumerable<TResult>>> GetAllAsync<TResult, TRequest>(
			TRequest request
		) where TRequest : SpRequest where TResult : class, DbResult, new()
		{
			DataResult<IEnumerable<TResult>> result = new();
			string storedProcedure = $"[dbo].[{request.StoredProcedure}]";
			string executableString = ExecutableString(request, storedProcedure);

			try
			{
				DynamicParameters dynamicParameters = request.ParseDynamicParameters();

				IEnumerable<TResult> data = EvaluateEnumerable(
					await FasterDapperManager.GetAllAsync<TResult>(dynamicParameters, storedProcedure)
				);

				result = DataResult<IEnumerable<TResult>>.Success(data);
			}
			catch (Exception ex)
			{
				if (ex is SqlException sqlEx)
				{
					result = DataResult<IEnumerable<TResult>>.Fail(executableString, sqlEx);
				}
				else
				{
					result = DataResult<IEnumerable<TResult>>.Fail(executableString);
				}
			}

			LoggerHelper.ValidateResult(result);

			return result;
		}

		public async Task<DataResult<IEnumerable<TResult>>> GetAllAsync<TResult, TRequest>(
			TRequest request, string connectioString
		) where TRequest : SpRequest where TResult : class, DbResult, new()
		{
			DataResult<IEnumerable<TResult>> result = new();
			string storedProcedure = $"[dbo].[{request.StoredProcedure}]";
			string executableString = ExecutableString(request, storedProcedure);

			try
			{
				DynamicParameters dynamicParameters = request.ParseDynamicParameters();

				IEnumerable<TResult> data = EvaluateEnumerable(
					await FasterDapperManager.GetAllAsync<TResult>(dynamicParameters, storedProcedure)
				);

				result = DataResult<IEnumerable<TResult>>.Success(data);
			}
			catch (Exception ex)
			{
				if (ex is SqlException sqlEx)
				{
					result = DataResult<IEnumerable<TResult>>.Fail(executableString, sqlEx);
				}
				else
				{
					result = DataResult<IEnumerable<TResult>>.Fail(executableString);
				}
			}

			LoggerHelper.ValidateResult(result);

			return result;
		}

		#endregion Gets from TRequest

		#region Gets from Objetc with List<TRequest>

		public async Task<DataResult<TResult>> GetCombinedAsync<TResult, TRequest>(
			string connectionString, TRequest request
		) where TRequest : SpRequest where TResult : class, DbResult, new()
		{
			DataResult<TResult> result = new();
			string executableString = ExecutableString(request, request.StoredProcedure);

			try
			{
				DynamicParameters dynamicParameters = request.ParseDynamicParameters();

				TResult data = EvaluateObject(
					await FasterDapperManager.GetCombinedAsync<TResult>(dynamicParameters, request.StoredProcedure)
				);

				result = DataResult<TResult>.Success(data);
			}
			catch (Exception ex)
			{
				if (ex is SqlException sqlEx)
				{
					result = await Task.FromResult(DataResult<TResult>.Fail(Error.DBException(sqlEx.Number, executableString), sqlEx));
				}
				else
				{
					result = await Task.FromResult(DataResult<TResult>.Fail(Error.Exception("DPGA", ex.Message, executableString)));
				}
			}

			LoggerHelper.ValidateResult(result);

			return result;
		}

		public async Task<DataResult<IEnumerable<TResult>>> GetAllCombinedAsync<TResult, TRequest>(
			string connectionString, TRequest request
		) where TRequest : SpRequest where TResult : class, DbResult, new()
		{
			DataResult<IEnumerable<TResult>> result = new();
			string executableString = ExecutableString(request, request.StoredProcedure);

			try
			{
				DynamicParameters dynamicParameters = request.ParseDynamicParameters();

				IEnumerable<TResult> data = EvaluateEnumerable(
					await FasterDapperManager.GetAllCombinedAsync<TResult>(dynamicParameters, request.StoredProcedure)
				);

				result = DataResult<IEnumerable<TResult>>.Success(data);
			}
			catch (Exception ex)
			{
				if (ex is SqlException sqlEx)
				{
					result = DataResult<IEnumerable<TResult>>.Fail(Error.DBException(sqlEx.Number, executableString), sqlEx);
				}
				else
				{
					result = DataResult<IEnumerable<TResult>>.Fail(Error.Exception("DPGA", ex.Message, executableString));
				}
			}

			LoggerHelper.ValidateResult(result);

			return result;
		}

		#endregion Gets from Objetc with List<TRequest>

		private TResult EvaluateObject<TResult>(TResult obj) where TResult : class, DbResult, new()
		{
			if (obj.IsNull())
			{
				return new TResult() { bResultado = false, vchMensaje = "Sin resultados" };
			}

			return obj;
		}

		private IEnumerable<TResult> EvaluateEnumerable<TResult>(IEnumerable<TResult> enumerable) where TResult : class, DbResult, new()
		{
			if (enumerable.IsNull())
			{
				return new List<TResult>() { new TResult() { bResultado = false, vchMensaje = "Sin resultados" } };
			}

			return enumerable;
		}

		private string ExecutableString<TRequest>(TRequest request, string storeProcedure)
		{
			PropertyInfo[] properties = request.GetType().GetProperties();
			string excecutableString = string.Empty;
			string parameterString = string.Empty;
			string parameterNameObject = string.Empty;
			string declare = string.Empty;
			string columns = string.Empty;
			string values = string.Empty;

			foreach (PropertyInfo property in properties)
			{
				//if (property.GetType().GetTypeInfo().IsClass)
				//{
				//	object value = property.GetValue(request, null);
				//	IList<PropertyInfo> propertiesObject = value.GetType().GetProperties().ToList();

				//	foreach (var propertyObject in propertiesObject)
				//	{
				//		int indexOf = propertiesObject.IndexOf(propertyObject);
				//		object valueObject = propertyObject.GetValue(value, null);

				//		columns += $"{propertyObject.Name}";
				//		columns += indexOf < propertiesObject.Count - 1 ? ", " : string.Empty;

				//		values += $"{BeautyObject(valueObject)}";
				//		values += indexOf < propertiesObject.Count - 1 ? ", " : string.Empty;
				//	}

				//	parameterNameObject = $"@{property.Name}";
				//}

				parameterString += $"\t@{property.Name} = @{property.Name}\n";
			}

			if (parameterNameObject.NotEmpty())
			{
				excecutableString += $"DECLARE {parameterNameObject} @{parameterNameObject};\n\n";
				excecutableString += $"INSERT INTO @{parameterNameObject} ({columns}) VALUES ({values})\n\n";

				excecutableString += $"EXEC dbo.{storeProcedure}\n";
				excecutableString += parameterString;
			}
			else
			{
				excecutableString += $"EXEC dbo.{storeProcedure}\n";
				excecutableString += parameterString;
			}

			return excecutableString;
		}

		private string BeautyObject(object obj)
		{
			if (obj is DateTime | obj is string)
			{
				return $"'{obj}'";
			}

			return obj.ToString();
		}
	}
}