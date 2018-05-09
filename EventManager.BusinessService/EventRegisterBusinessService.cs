using EventManager.DataModel.Models;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManager.BusinessService
{


	public interface IEventRegisterBusinessService : IService<EventRegister>
	{

	}

	public class EventRegisterBusinessService : Service<EventRegister>, IEventRegisterBusinessService
	{
		private readonly IRepositoryAsync<EventRegister> _repository;
		public EventRegisterBusinessService(IRepositoryAsync<EventRegister> repository)
			: base(repository)
		{
			_repository = repository;
		}
	}
}
