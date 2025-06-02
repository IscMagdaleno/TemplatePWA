using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CommonFuncion.Extensions
{
	public static class GenericExt
	{
		public static T DeepCopy<T>(this T self)
		{
			var serialized = Jsons.Stringify(self);

			return Jsons.Parse<T>(serialized);
		}

		public static T Clone<T>(this T source)
		{
			if (typeof(T).IsSerializable.False())
			{
				throw new ArgumentException("The type must be serializable.", "source");
			}

			if (ReferenceEquals(source, null))
			{
				return default;
			}

			IFormatter formatter = new BinaryFormatter();
			Stream stream = new MemoryStream();

			using (stream)
			{
				formatter.Serialize(stream, source);
				stream.Seek(0, SeekOrigin.Begin);

				return (T)formatter.Deserialize(stream);
			}
		}

		public static T TrimSpaces<T>(this T self)
		{
			if (self != null)
			{
				var properties = self.GetType().GetProperties().Where(p => p.Name.Contains("vch"));

				foreach (var property in properties)
				{
					if (property.CanWrite)
					{
						var value = property.GetValue(self);

						if (value.NotNull())
						{
							if (value.ToString().StartsWith(" ") || value.ToString().EndsWith("  "))
							{
								var stringValue = value.ToString();

								if (stringValue.NotEmpty())
								{
									value = stringValue.Trim();

									property.SetValue(self, value);
								}
							}
						}
					}
				}
			}

			return self;
		}

		public static bool AreEquals<T, K>(this T self, K another)
		{
			var a = Jsons.Stringify(self);
			var b = Jsons.Stringify(another);

			return a.Equals(b);
		}

		public static bool Validate<T>(this T model, Predicate<T>[] validaciones)
		{
			var errores = validaciones.Count(x => x(model).False());

			return errores == 0;
		}

		public static string Join<T>(this IEnumerable<T> self, string separator = ", ")
		{
			return string.Join(separator, self);
		}
	}
}
