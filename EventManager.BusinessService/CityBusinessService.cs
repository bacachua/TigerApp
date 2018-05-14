using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventManager.DataModel.Models;
using Service.Pattern;
using EventManager.Repository;
using Repository.Pattern;
using Repository.Pattern.Repositories;

namespace EventManager.BusinessService
{

	public interface ICityBusinessService : IService<City>
	{

	}

	public class CityBusinessService : Service<City>, ICityBusinessService
	{
		private readonly IRepositoryAsync<City> _repository;
		public CityBusinessService(IRepositoryAsync<City> repository)
			: base(repository)
		{
			_repository = repository;
		}
	}
}
