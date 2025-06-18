using EngramaCoreStandar.Extensions;

namespace TemplatePWA.API.Helpers
{
	public class Extenciones
	{

		public Extenciones()
		{
			ExtencionBool();
			ExtencionDecimal();
			ExtencionListas();
			ExtencionIntResultBool();
			ExtencionIntResultString();
			ExtencionStringResultBool();
			ExtencionStringResultString();
		}

		private void ExtencionBool()
		{
			bool UsersExists = false;

			// devuelve "false" si UsersExists es false, de lo contrario devuelve "true"
			var resultado = UsersExists.False();

		}


		private void ExtencionDecimal()
		{
			//Algun valor que se obtenga de la base de datos o de una validación
			decimal precioProducto = 3500.52m;
			Console.WriteLine(
				$"El precio del producto es {precioProducto.ToDecimalCurrency()}"
			);
			//Si el precio es 3500.52, se mostrará "El precio del producto es $3,500.52"
		}


		private void ExtencionListas()
		{
			List<string> AlgunaLista = new List<string>
			{
				"apple",
				"banana",
				"cherry"
			};

			// Verifica si la lista está vacía
			var resutl = AlgunaLista.IsEmpty();

			// Verifica si la lista no está vacía
			var resutl2 = AlgunaLista.NotEmpty();

			// Verifica si la lista tiene un solo elemento
			var resutl3 = AlgunaLista.SingleOne();

			// Convierte la lista a una cadena unida por comas
			var resutl4 = AlgunaLista.ToStringJoin();

		}


		private void ExtencionIntResultBool()
		{
			int numEntero = 5;

			bool resultado = false;

			//Valida si e ntero es igual a alguno de los valores del array
			resultado = numEntero.IsAny(new int[3] { 4, 5, 6 });

			// Valida si el entero no es igual a ninguno de los valores del array
			resultado = numEntero.NotAny(new int[3] { 4, 5, 6 });

			// Valida si el entero está entre 8 y 9 (exclusivo)
			resultado = numEntero.Between(8, 9);

			// Valida si el entero no está entre 8 y 9 (exclusivo)
			resultado = numEntero.NotBetween(8, 9);


		}

		private void ExtencionIntResultString()
		{
			int numero = 5;
			string Resultado = string.Empty;

			//retorna el nombre del mes correspondiente al número (1-12) o una cadena vacía si no es válido
			Resultado = numero.GetMonth();

			// retorna el nombre del día de la semana correspondiente al número (1-7) o una cadena vacía si no es válido
			Resultado = numero.GetDay();
		}


		private void ExtencionStringResultBool()
		{
			string cadena = "Hola Mundo";
			bool resultado = false;

			// Verifica si la cadena es nula o está vacía
			resultado = cadena.IsEmpty();

			//verifica si la cadena no es nula o no está vacía
			resultado = cadena.NotEmpty();

			// verifica si la cadena contiene dígitos
			resultado = cadena.HasDigits();

			// verifica si la cadena contiene letras
			resultado = cadena.HasLetters();

		}


		private void ExtencionStringResultString()
		{
			string cadena = "Hola Mundo";
			string resultado = string.Empty;

			// Elimina espacios en blanco al inicio y al final de la cadena
			resultado = cadena.TrimAll();

			// genera un nuevo GUID basado en la cadena
			resultado = cadena.NewGuid();


			// Capitaliza la primera letra de la cadena 

			resultado = cadena.Capitalize();

			// obtiene una subcadena de la cadena original con un máximo de 5 caracteres
			resultado = cadena.MaxSubstring(5);

		}

	}
}
