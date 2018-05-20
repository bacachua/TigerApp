using EventManager.ApiModels;
using EventManager.DataModel.Models;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
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
		bool SetEventRegisterStatus(int eventRegisterId, eEventRegisterStatus eventRegisterStatus);
	}

	public class EventRegisterBusinessService : Service<EventRegister>, IEventRegisterBusinessService
	{
		private  IRepositoryAsync<EventRegister> _repository;
		public EventRegisterBusinessService(IRepositoryAsync<EventRegister> repository)
			: base(repository)
		{
			_repository = repository;
		}

		public bool SetEventRegisterStatus(int eventRegisterId, eEventRegisterStatus eventRegisterStatus)
		{
			bool ret = true;
			try
			{
				using (IDataContextAsync context = new GameManagerContext())
				using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
				{
					_repository = new Repository<EventRegister>(context, unitOfWork);
					var eventRegister = _repository.Find(eventRegisterId);
					eventRegister.Status = (int)eventRegisterStatus;
					_repository.Update(eventRegister);
					unitOfWork.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				ret = false;
			}

			return ret;
		}
	}


}
