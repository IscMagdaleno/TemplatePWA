using Dapper;

using EngramaCoreStandar;
using EngramaCoreStandar.Dapper.Interfaces;
using EngramaCoreStandar.Extensions;
using EngramaCoreStandar.Logger;
using EngramaCoreStandar.Results;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngramaCoreStandar.Dapper
{
	/// <summary>
	/// Auxiliar para el uso de dapper al convertir los modelos ya en la lista de parámetros 
	/// </summary>
	public class DapperMangerHelper : IDapperManagerHelper
	{
		public IDapperManager DapperManager { get; set; }
		public ILoggerHelper LoggerHelper { get; }
		private readonly ILogger<DapperMangerHelper> _logger;

		/***/

		private readonly Dictionary<string, DbType> DbTypeMap = new Dictionary<string, DbType>();

		/**/

		private string ConnectionString { get; set; }


		public DapperMangerHelper(IDapperManager dapperManager, ILoggerHelper loggerHelper, IConfiguration configuration, ILogger<DapperMangerHelper> logger)
		{
			ConnectionString = configuration.GetConnectionString("EngramaCloudConnection");

			DapperManager = dapperManager;
			LoggerHelper = loggerHelper;
			_logger = logger;

			DbTypeMap.Add("System.Boolean", DbType.Boolean);

			DbTypeMap.Add("System.Byte", DbType.Byte);
			DbTypeMap.Add("System.SByte", DbType.SByte);
			DbTypeMap.Add("System.Byte[]", DbType.Binary);

			//DbTypeMap.Add("System.Char", DbType.);

			DbTypeMap.Add("System.Decimal", DbType.Decimal);
			DbTypeMap.Add("System.Double", DbType.Decimal);
			DbTypeMap.Add("System.Single", DbType.Decimal);

			DbTypeMap.Add("System.Int32", DbType.Int32);
			DbTypeMap.Add("System.UInt32", DbType.UInt32);

			//DbTypeMap.Add("System.IntPtr", DbType.Int32);
			//DbTypeMap.Add("System.UIntPtr", DbType.Int32);


			DbTypeMap.Add("System.Int64", DbType.Int64);
			DbTypeMap.Add("System.UInt64", DbType.UInt64);

			DbTypeMap.Add("System.Int16", DbType.Int16);
			DbTypeMap.Add("System.UInt16", DbType.UInt16);

			DbTypeMap.Add("System.Object", DbType.Object);
			DbTypeMap.Add("System.String", DbType.String);

			DbTypeMap.Add("System.Guid", DbType.Guid);
			DbTypeMap.Add("System.Datetime", DbType.DateTime);
			DbTypeMap.Add("System.DateTime", DbType.DateTime);
		}

		#region Gets from script



		public async Task<DataResult<TResult>> GetAsync<TResult>(string Script, string? connectionString = null, string shema = "dbo")
			where TResult : class, DbResult, new()
		{
			if (connectionString.IsEmpty())
			{
				connectionString = ConnectionString;
			}


			var resp = new DataResult<TResult>();
			if (string.IsNullOrWhiteSpace(Script))
			{
				var msg = ("The script cannot be null or empty.");
				resp = DataResult<TResult>.Fail(msg);
				return resp;
			}



			try
			{
				var data = await DapperManager.GetAsync<TResult>(Script, connectionString);

				data ??= new TResult { bResult = false, vchMessage = "Sin resultados" };


				LoggerHelper.ValidateSPResult(data);

				resp = (DataResult<TResult>.Success(data));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "[Dapper] GetAsync<{Type}> failed. Script={Script}", typeof(TResult).Name, Script);
				resp = DataResult<TResult>.Fail(ex.Message);
			}

			LoggerHelper.ValidateResult(resp);
			return resp;
		}

		public async Task<DataResult<IEnumerable<TResult>>> GetAllAsync<TResult>(string script, string? connectionString = null, string shema = "dbo")
	where TResult : class, DbResult, new()
		{
			var result = new DataResult<IEnumerable<TResult>>();

			if (connectionString.IsEmpty())
			{
				connectionString = ConnectionString;
			}

			var resp = new DataResult<TResult>();
			if (script.IsEmpty())
			{
				var msg = ("The script cannot be null or empty.");
				result = DataResult<IEnumerable<TResult>>.Fail(msg);
				return result;
			}


			try
			{
				// Execute the command asynchronously to get the data
				var dataList = await DapperManager.GetAllAsync<TResult>(script, connectionString);

				// Use the system collections string; if empty, add a 'no results' message
				if (dataList.IsEmpty())
				{
					dataList = new List<TResult> { new TResult { bResult = false, vchMessage = "Sin resultados" } };
				}

				// Indicate a successful operation
				result = DataResult<IEnumerable<TResult>>.Success(dataList);

				// Validate the received list of results
				LoggerHelper.ValidateResultLst(result);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "[Dapper] GetAllAsync<{Type}> failed.", typeof(TResult).Name);
				result = DataResult<IEnumerable<TResult>>.Fail(ex.Message);
			}

			// Validate the entire result and log appropriately
			LoggerHelper.ValidateResultLst(result);

			return result;
		}

		#endregion Gets from script


		#region Gets from TRequest

		public async Task<DataResult<TResult>> GetAsync<TResult, TRequest>(TRequest request, string? connectionString = null, string shema = "dbo")
	where TRequest : SpRequest
	where TResult : class, DbResult, new()
		{

			if (connectionString.IsEmpty())
			{
				connectionString = ConnectionString;
			}

			// Initialize result holder and stored procedure name
			var result = new DataResult<TResult>();
			var storedProcedure = $"[{shema}].[{request.StoredProcedure}]";

			string executableString = string.Empty;

			try
			{
				// Create dynamic parameters from the request
				var dynamicParameters = CreateDynamicParameters(request, shema);
				executableString = dynamicParameters.ExcecutableString;

				// Asynchronously retrieve data
				var data = await DapperManager.GetAsync<TResult>(storedProcedure, dynamicParameters.Parameters, connectionString);

				data ??= new TResult { bResult = false, vchMessage = "Sin resultados" };

				result = DataResult<TResult>.Success(data);
			}
			catch (SqlException sqlEx)
			{
				_logger.LogError(sqlEx, "[Dapper] GetAsync<{Type}> SQL error. SP={SP} Exec={Exec}", typeof(TResult).Name, storedProcedure, executableString);
				result = DataResult<TResult>.Fail(executableString, sqlEx);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "[Dapper] GetAsync<{Type}> error. SP={SP} Exec={Exec}", typeof(TResult).Name, storedProcedure, executableString);
				result = DataResult<TResult>.Fail(ex.Message);
			}

			// Log and validate the operation's result
			LoggerHelper.ValidateResult(result);

			return result;
		}


		public async Task<DataResult<IEnumerable<TResult>>> GetAllAsync<TResult, TRequest>(TRequest request, string? connectionString = null, string shema = "dbo")
	where TRequest : SpRequest
	where TResult : class, DbResult, new()
		{
			if (connectionString.IsEmpty())
			{
				connectionString = ConnectionString;
			}

			var storedProcedure = $"[{shema}].[{request.StoredProcedure}]";
			var result = new DataResult<IEnumerable<TResult>>();
			string executableString = string.Empty;

			try
			{
				// Create dynamic parameters from the request
				var dynamicParameters = CreateDynamicParameters(request, shema);
				executableString = dynamicParameters.ExcecutableString;

				// Asynchronously retrieve all data
				var data = (await DapperManager.GetAllAsync<TResult>(storedProcedure, dynamicParameters.Parameters, connectionString))
						  ?? new List<TResult> { new TResult() { bResult = false, vchMessage = "Sin resultados" } };

				result = DataResult<IEnumerable<TResult>>.Success(data);
			}
			catch (SqlException sqlEx)
			{
				_logger.LogError(sqlEx, "[Dapper] GetAllAsync<{Type}> SQL error. SP={SP}", typeof(TResult).Name, storedProcedure);
				result = DataResult<IEnumerable<TResult>>.Fail(sqlEx.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "[Dapper] GetAllAsync<{Type}> error. SP={SP}", typeof(TResult).Name, storedProcedure);
				result = DataResult<IEnumerable<TResult>>.Fail(ex.Message);
			}

			// Log and validate the operation's result
			LoggerHelper.ValidateResultLst(result);

			return result;
		}


		#endregion Gets from TRequest



		#region Gets from Objetc with List<TRequest>


		public async Task<DataResult<TResult>> GetWithListAsync<TResult, TRequest>(
			 TRequest request, string? connectionString = null
		, string shema = "dbo")
			where TRequest : SpRequest
			where TResult : class, DbResult, new()
		{
			if (connectionString.IsEmpty())
			{
				connectionString = ConnectionString;
			}

			BuildSqlCommand
			(request, shema);

			DataResult<TResult> result;
			TResult responseObject = default(TResult);

			try
			{
				var storedProcedure = $"[{shema}].[{request.StoredProcedure}]";
				using (var connection = new SqlConnection(connectionString))
				{
					await connection.OpenAsync();

					using (var command = new SqlCommand(storedProcedure, connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						// Add parameters from TRequest to the command
						AddParametersToCommand(command, request, shema);

						// Execute the reader and map to TResult
						responseObject = await ExecuteReaderAndMapResults<TResult>(command);
					}
				}

				result = DataResult<TResult>.Success(responseObject);
			}
			catch (SqlException sqlEx)
			{
				// Use a descriptive message or have sqlCadena defined elsewhere
				result = DataResult<TResult>.Fail(Error.DBException(sqlEx.Number, "SQL query or description"), sqlEx);
			}
			catch (Exception ex)
			{
				result = DataResult<TResult>.Fail(Error.Exception("DPGA", ex.Message, "Additional Information"), ex);
			}

			LoggerHelper.ValidateResult(result);
			return result;

		}

		private static void AddParametersToCommand<TRequest>(SqlCommand command, TRequest request, string shema = "dbo")
		{
			var properties = typeof(TRequest).GetProperties();

			foreach (var property in properties.Where(e => e.Name != "StoredProcedure"))
			{
				var propertyValue = property.GetValue(request);

				if (TryGetListElementType(property.PropertyType, out var elementType))
				{
					var typeName = $"{shema}.{elementType.Name}";
					var listParameter = new SqlParameter
					{
						ParameterName = "@" + property.Name,
						SqlDbType = SqlDbType.Structured,
						TypeName = typeName,
						Value = ConvertToDataTable(elementType, propertyValue as System.Collections.IEnumerable)
					};
					command.Parameters.Add(listParameter);
				}
				else
				{
					// SqlDateTime rejects dates before 1753-01-01 (CLR default is 0001-01-01)
					command.Parameters.AddWithValue("@" + property.Name, NormalizeSqlParameterValue(propertyValue));
				}
			}
		}

		/// <summary>
		/// Ensures DateTime values are within SQL Server datetime range; otherwise uses Engrama SqlMinDate / DBNull.
		/// </summary>
		private static object NormalizeSqlParameterValue(object? propertyValue)
		{
			if (propertyValue == null)
			{
				return DBNull.Value;
			}

			if (propertyValue is DateTime dateTime)
			{
				return dateTime < Defaults.SqlMinDate() ? Defaults.SqlMinDate() : dateTime;
			}

			return propertyValue;
		}

		/// <summary>
		/// True when the property is a list/enumerable of a row type (TVP), excluding string and byte[].
		/// </summary>
		private static bool TryGetListElementType(Type propertyType, out Type elementType)
		{
			elementType = null!;
			if (propertyType == typeof(string) || propertyType == typeof(byte[]))
			{
				return false;
			}

			var enumerable = propertyType
				.GetInterfaces()
				.Concat(new[] { propertyType })
				.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

			if (enumerable == null)
			{
				return false;
			}

			elementType = enumerable.GetGenericArguments()[0];
			return elementType != typeof(char);
		}

		/// <summary>
		/// Builds a DataTable for a TVP. Always creates columns from <paramref name="elementType"/>
		/// so empty lists still produce a valid Structured parameter schema.
		/// </summary>
		private static DataTable ConvertToDataTable(Type elementType, System.Collections.IEnumerable? list)
		{
			var dataTable = new DataTable();
			var properties = elementType.GetProperties();

			foreach (var property in properties)
			{
				dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
			}

			if (list == null)
			{
				return dataTable;
			}

			foreach (var item in list)
			{
				if (item == null)
				{
					continue;
				}

				var row = dataTable.NewRow();
				foreach (var property in properties)
				{
					var value = property.GetValue(item);
					if (value is DateTime dateTime && dateTime < Defaults.SqlMinDate())
					{
						value = Defaults.SqlMinDate();
					}
					row[property.Name] = value ?? DBNull.Value;
				}
				dataTable.Rows.Add(row);
			}

			return dataTable;
		}

		private static async Task<TResult> ExecuteReaderAndMapResults<TResult>(SqlCommand command) where TResult : new()
		{
			TResult result = new TResult();

			using (var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow))
			{
				if (await reader.ReadAsync())
				{
					MapReaderRow(reader, result);
				}
			}

			return result;
		}

		public async Task<DataResult<IEnumerable<TResult>>> GetAllWithListAsync<TResult, TRequest>(
			 TRequest request, string? connectionString = null
		, string shema = "dbo")
			where TRequest : SpRequest
			where TResult : class, DbResult, new()
		{
			if (connectionString.IsEmpty())
			{
				connectionString = ConnectionString;
			}

			var storedProcedure = $"[{shema}].[{request.StoredProcedure}]";
			BuildSqlCommand(request, shema);

			IEnumerable<TResult> responseObjects = Enumerable.Empty<TResult>();
			DataResult<IEnumerable<TResult>> result;

			try
			{
				using (var connection = new SqlConnection(connectionString))
				{
					await connection.OpenAsync();

					using (var command = new SqlCommand(storedProcedure, connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						AddParametersToCommand(command, request, shema);
						responseObjects = await ExecuteReaderAndMapResultsToList<TResult>(command);
					}
				}

				result = DataResult<IEnumerable<TResult>>.Success(responseObjects);
			}
			catch (SqlException sqlEx)
			{
				result = DataResult<IEnumerable<TResult>>.Fail(Error.DBException(sqlEx.Number, "SQL query or description"), sqlEx);
			}
			catch (Exception ex)
			{
				result = DataResult<IEnumerable<TResult>>.Fail(Error.Exception("DPGA", ex.Message, "Additional Information"), ex);
			}

			LoggerHelper.ValidateResultLst(result);
			return result;
		}

		private static async Task<IEnumerable<TResult>> ExecuteReaderAndMapResultsToList<TResult>(SqlCommand command)
	where TResult : new()
		{
			var results = new List<TResult>();

			using (var reader = await command.ExecuteReaderAsync())
			{
				while (await reader.ReadAsync())
				{
					var result = new TResult();
					MapReaderRow(reader, result);
					results.Add(result);
				}
			}

			return results;
		}

		private static void MapReaderRow<TResult>(System.Data.Common.DbDataReader reader, TResult result)
		{
			foreach (var property in typeof(TResult).GetProperties())
			{
				var ordinal = TryGetOrdinal(reader, property.Name);
				if (ordinal == null || reader.IsDBNull(ordinal.Value))
				{
					continue;
				}

				var value = reader.GetValue(ordinal.Value);
				var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
				property.SetValue(result, Convert.ChangeType(value, targetType));
			}
		}

		private static int? TryGetOrdinal(System.Data.Common.DbDataReader reader, string name)
		{
			for (var i = 0; i < reader.FieldCount; i++)
			{
				if (string.Equals(reader.GetName(i), name, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}

			return null;
		}



		#endregion Gets from Objetc with List<TRequest>


		#region Métodos extras 


		private DynamicParametersPair CreateDynamicParameters<T>(T obj, string shema) where T : SpRequest
		{
			var parameters = new DynamicParameters();
			var builder = new StringBuilder();

			builder.Append("EXEC").Append(' ').Append($"{shema}.").Append(obj.StoredProcedure).Append(' ');

			/**/

			var properties = obj.GetType().GetProperties();

			if (properties.NotEmpty())
			{

				var OnlyValidAtributos = properties.Where(e => e.Name != "StoredProcedure");
				if (OnlyValidAtributos.NotEmpty())
				{

					foreach (var prop in OnlyValidAtributos)
					{
						var fullname = prop.PropertyType.FullName ?? prop.PropertyType.Name;
						if (fullname.Contains("System.Nullable"))
						{
							fullname = fullname.Replace("System.Nullable`1[[", "");
							var comma = fullname.IndexOf(',');
							if (comma > 0)
							{
								fullname = fullname.Substring(0, comma);
							}
						}

						if (!DbTypeMap.TryGetValue(fullname, out var dbType))
						{
							dbType = DbType.Object;
						}

						var value = prop.GetValue(obj);

						var property = prop.Name;
						parameters.Add(property, value, dbType);

						builder.Append("@").Append(property).Append(" = ").Append(Beautify(value)).Append(',');
					}

					builder.Remove(builder.ToString().LastIndexOf(","), 1);
				}
			}

			var excecutableString = builder.ToString();

			LoggerHelper.Info(excecutableString);

			return new DynamicParametersPair(parameters, excecutableString);
		}


		private object Beautify(object value)
		{
			if (value.NotNull())
			{
				var type = value.GetType();

				switch (type.FullName)
				{
					case "System.String":

						value = $"'{value}'";

						break;

					case "System.DateTime":

						value = $"'{((DateTime)value):yyyy-MM-dd}'";

						break;
				}
			}

			return value;
		}

		private sealed class DynamicParametersPair
		{
			public DynamicParameters Parameters { get; }

			public string ExcecutableString { get; }

			public DynamicParametersPair(DynamicParameters parameters, string excecutableString)
			{
				Parameters = parameters;
				ExcecutableString = excecutableString;
			}
		}

		private void BuildSqlCommand<TRequest>(TRequest request, string shema = "dbo") where TRequest : SpRequest
		{
			var sqlBuilder = new StringBuilder();
			var sqlBuilderList = new StringBuilder();
			var properties = typeof(TRequest).GetProperties();

			sqlBuilder.AppendLine($"EXEC {shema}.{request.StoredProcedure} ");

			foreach (var property in properties)
			{

				var propertyValue = property.GetValue(request);

				if (property.Name != "StoredProcedure")
				{

					if (TryGetListElementType(property.PropertyType, out var elementType)
						&& propertyValue is System.Collections.IEnumerable list)
					{
						sqlBuilderList.AppendLine($"DECLARE @{property.Name} {shema}.{elementType.Name};");

						foreach (var item in list)
						{
							if (item == null) continue;
							var itemBuilder = new StringBuilder();
							sqlBuilderList.Append($"INSERT INTO @{property.Name} (");

							var itemProperties = item.GetType().GetProperties();
							foreach (var itemProperty in itemProperties)
							{
								sqlBuilderList.Append($"{itemProperty.Name}, ");
							}
							sqlBuilderList.Length -= 2;  // Remove last comma
							itemBuilder.Append(") VALUES (");

							foreach (var itemProperty in itemProperties)
							{
								var itemValue = itemProperty.GetValue(item);
								var formattedValue = FormatSqlValue(itemValue);
								itemBuilder.Append($"{formattedValue}, ");
							}
							itemBuilder.Length -= 2;  // Remove last comma
							itemBuilder.Append(");");

							sqlBuilderList.AppendLine(itemBuilder.ToString());
						}
						sqlBuilder.AppendLine($" @{property.Name}  = @{property.Name},");


					}
					else
					{


						var formattedValue = FormatSqlValue(propertyValue);
						sqlBuilder.AppendLine($" @{property.Name}  = {formattedValue},");

					}
				}
			}
			var sqlText = sqlBuilder.ToString();
			var lastComma = sqlText.LastIndexOf(',');
			if (lastComma >= 0)
			{
				sqlBuilder[lastComma] = ';';
			}

			sqlBuilderList.AppendLine("");
			sqlBuilderList.Append(sqlBuilder);

			_logger.LogDebug("[Dapper] BuildSqlCommand:\n{Sql}", sqlBuilderList.ToString());

		}

		private static string FormatSqlValue(object value)
		{
			if (value == null || value == DBNull.Value)
				return "NULL";

			if (value is string)
			{

				return $"'{value}'";
			}

			if (value is DateTime)
			{
				var tmpDate = value.ToString();

				DateTime date = DateTime.Parse(tmpDate);

				return $"'{date:yyyy-MM-dd}'";
			}

			if (value is bool boolValue)
				return boolValue ? "1" : "0";

			return value.ToString();
		}



		#endregion Métodos extras 

	}
}
