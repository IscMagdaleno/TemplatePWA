using Dapper;

using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;

namespace CommonFuncion.FasterDapper.Extensions
{
	public static class SqlCommandExtension
	{
		public static IEnumerable<T> GetResults<T>(
			this SqlCommand sqlCommand
		)
		{
			IList<T> result = new List<T>();

			using (SqlDataReader executeReader = sqlCommand.ExecuteReader(CommandBehavior.SingleRow))
			{
				while (executeReader.Read())
				{
					T instance = Activator.CreateInstance<T>();
					ReadOnlyCollection<DbColumn> columns = executeReader.GetColumnSchema();

					foreach (DbColumn column in columns)
					{
						object value = executeReader.GetValue(column.ColumnName);

						instance.GetType().GetProperty(column.ColumnName).SetValue(instance, value);
					}

					result.Add(instance);
				}
			}

			return result;
		}

		public static async Task<IEnumerable<T>> GetResultsAsync<T>(
			this SqlCommand sqlCommand
		)
		{
			IList<T> result = new List<T>();

			using (SqlDataReader executeReader = sqlCommand.ExecuteReader(CommandBehavior.SingleRow))
			{
				while (await executeReader.ReadAsync())
				{
					T instance = Activator.CreateInstance<T>();
					ReadOnlyCollection<DbColumn> columns = executeReader.GetColumnSchema();

					foreach (DbColumn column in columns)
					{
						object value = executeReader.GetValue(column.ColumnName);

						instance.GetType().GetProperty(column.ColumnName).SetValue(instance, value);
					}

					result.Add(instance);
				}
			}

			return result;
		}

		public static SqlParameter[] ParseSqlParameters(this DynamicParameters dynamicParameters)
		{
			SqlParameter[] sqlParameters = new SqlParameter[dynamicParameters.ParameterNames.Count()];

			foreach (var parameter in dynamicParameters.ParameterNames)
			{
				var param = dynamicParameters.Get<object>(parameter);
				var sqlParameter = new SqlParameter(parameter, param ?? DBNull.Value);

				if (param is IDbDataParameter dbParam)
				{
					sqlParameter.Direction = dbParam.Direction;
					sqlParameter.DbType = dbParam.DbType;
					sqlParameter.Size = dbParam.Size;
					sqlParameter.Precision = dbParam.Precision;
					sqlParameter.Scale = dbParam.Scale;
				}

				sqlParameters.Append(sqlParameter);
			}

			return sqlParameters;
		}

		public static SqlParameter[] ToSqlParameterCollection<T>(
			this T self
		)
		{
			List<PropertyInfo> properties = self.GetType().GetProperties().ToList();
			SqlParameter[] sqlParameters = new SqlParameter[properties.Count];

			foreach (var property in properties)
			{
				SqlDbType sqlDbType = property.PropertyType.GetSqlDbType();
				object value = property.GetValue(self);
				string parameterName = $"@{property.Name}";

				var sqlParameter = new SqlParameter(parameterName, sqlDbType);

				if (value is IDbDataParameter dbParam)
				{
					sqlParameter.Direction = dbParam.Direction;
					sqlParameter.DbType = dbParam.DbType;
					sqlParameter.Size = dbParam.Size;
					sqlParameter.Precision = dbParam.Precision;
					sqlParameter.Scale = dbParam.Scale;
				}

				sqlParameters.Append(sqlParameter);
			}

			return sqlParameters;
		}
	}
}