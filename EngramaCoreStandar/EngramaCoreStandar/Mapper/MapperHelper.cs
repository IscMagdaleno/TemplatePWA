using AutoMapper;

using Microsoft.Extensions.Logging.Abstractions; // Agrega este using

using System;
using System.Collections.Generic;
using System.Linq;

namespace EngramaCoreStandar.Mapper
{
	public class MapperHelper
	{
		private IMapper _mapper;
		private MapperConfiguration _configuration;
		private readonly HashSet<(Type SourceType, Type DestType)> _mappedTypes;

		public MapperHelper()
		{
			_mappedTypes = new HashSet<(Type, Type)>();

			// AutoMapper 15.x: Requiere configuración inicial y un ILoggerFactory (NullLoggerFactory para ignorarlo)
			_configuration = new MapperConfiguration(cfg => { }, NullLoggerFactory.Instance);
			_mapper = _configuration.CreateMapper();
		}

		private void EnsureMapping<T, K>()
		{
			var typePair = (typeof(T), typeof(K));
			if (!_mappedTypes.Contains(typePair))
			{
				lock (_mappedTypes)
				{
					if (!_mappedTypes.Contains(typePair))
					{
						// Crea una nueva configuración con los mapeos existentes más el nuevo
						// AutoMapper 15.x: Agregamos NullLoggerFactory.Instance como segundo parámetro
						var newConfig = new MapperConfiguration(cfg =>
						{
							// Reconstruye los mapeos existentes
							foreach (var pair in _mappedTypes.ToArray())
							{
								cfg.CreateMap(pair.SourceType, pair.DestType).ReverseMap();
							}
							// Agrega el nuevo mapeo
							cfg.CreateMap<T, K>().ReverseMap();
						}, NullLoggerFactory.Instance);

						_configuration = newConfig;
						_mapper = _configuration.CreateMapper();
						_mappedTypes.Add(typePair);
					}
				}
			}
		}

		public K Get<T, K>(T modelo)
		{
			EnsureMapping<T, K>();
			return _mapper.Map<T, K>(modelo);
		}

		public IList<K> Get<T, K>(IList<T> modelo)
		{
			EnsureMapping<T, K>();
			return _mapper.Map<IList<T>, IList<K>>(modelo);
		}

		public K Map<T, K>(T emisor, K receptor)
		{
			EnsureMapping<T, K>();
			return _mapper.Map(emisor, receptor);
		}

		public IList<K> Map<T, K>(IList<T> emisor, IList<K> receptor)
		{
			EnsureMapping<T, K>();
			return _mapper.Map(emisor, receptor);
		}

		public IEnumerable<K> GetEnumerable<T, K>(IEnumerable<T> modelo)
		{
			EnsureMapping<T, K>();
			return _mapper.Map<IEnumerable<T>, IEnumerable<K>>(modelo);
		}
	}
}