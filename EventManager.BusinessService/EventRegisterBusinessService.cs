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
using Repository.Pattern.Infrastructure;

namespace EventManager.BusinessService
{


	public interface IEventRegisterBusinessService : IService<EventRegister>
	{
		bool SetEventRegisterStatus(int eventRegisterId, eEventRegisterStatus eventRegisterStatus);
		bool ConfirmRegisterStatus(int eventRegisterId, eEventRegisterStatus eventRegisterStatus);
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
					if (eventRegister.Status == 0)
					{
						//eventRegister.ObjectState = ObjectState.
						eventRegister.ObjectState = ObjectState.Deleted;
						_repository.Delete(eventRegister);

						//eventRegister.Status = (int)eventRegisterStatus;
						//_repository.Update(eventRegister);
						unitOfWork.SaveChanges();
					}
					else
					{
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				ret = false;
			}

			return ret;
		}

		public bool ConfirmRegisterStatus(int eventRegisterId, eEventRegisterStatus eventRegisterStatus)
		{
			bool ret = true;
			EventRegister eventRegister = null;
			try
			{
				using (IDataContextAsync context = new GameManagerContext())
				using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
				{
					_repository = new Repository<EventRegister>(context, unitOfWork);
					eventRegister = _repository.Find(eventRegisterId);
					eventRegister.ObjectState = ObjectState.Modified;				
				    eventRegister.Status = (int)eventRegisterStatus;
					eventRegister.PlayedDate = DateTime.Now;
					_repository.Update(eventRegister);
					unitOfWork.SaveChanges();
				}			
				
			}
			catch (Exception ex)
			{
				ret = false;
			}

			try
			{
				if (eventRegister != null)
				{
					using (IDataContextAsync context = new GameManagerContext())
					using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
					{
						DateTime currentDate  = new DateTime();
						currentDate = DateTime.Now;
						_repository = new Repository<EventRegister>(context, unitOfWork);
						List<EventRegister> list = _repository.Query(x => x.UserId == eventRegister.UserId && x.Status ==  4 && x.Rewarded == "N").Select().ToList();
						if(list.Count >= 2)
						{
							IRepositoryAsync<AspNetUser> _repositoryUser = new Repository<AspNetUser>(context, unitOfWork);
							IContentBusinessService contentBusinessService = new ContentBusinessService();
							MessageContent messageContent = new MessageContent();
							AspNetUser aspNetUser = _repositoryUser.Find(eventRegister.UserId);
							if (!string.IsNullOrEmpty(aspNetUser.DeviceId))
							{
								messageContent.Receiver = aspNetUser.DeviceId;
								messageContent.BodyMessage = "Bạn sẽ được tặng 01 áo mưa khi hoàn thành 02 thử thách";
								messageContent.ServiceTypeID = 0;
								messageContent.Status = 0;
								messageContent.ModifiedDate = DateTime.Now;
								messageContent.CreatedDate = DateTime.Now;
								messageContent.Sender = "TigerWall";
								messageContent.UserId = aspNetUser.Id;
								contentBusinessService.InserContentMessage(messageContent);
								foreach(EventRegister itm in list)
								{
									itm.Rewarded = "Y";
									itm.ObjectState = ObjectState.Modified;
									_repository.Update(itm);
								}
								unitOfWork.SaveChanges();
							}		
						}
					}
				}
			}
			catch (Exception ex1)
			{ 
			}
			

			return ret;
		}
	}


}
