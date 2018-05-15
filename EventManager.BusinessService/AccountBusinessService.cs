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
using EventManager.ApiModels;
using Repository.Pattern.DataContext;
using Repository.Pattern.UnitOfWork;
using Repository.Pattern.Ef6;

namespace EventManager.BusinessService
{
	public interface IAccountBusinessService : IService<AspNetUser>
	{
		ApiAccountModel GetUserInfo(string userId);
	}

	public class AccountBusinessService : Service<AspNetUser>, IAccountBusinessService
	{
		private IRepositoryAsync<AspNetUser> _repository;

		public AccountBusinessService(IRepositoryAsync<AspNetUser> repository)
			: base(repository)
		{
			_repository = repository;
		}

	
		public ApiAccountModel GetUserInfo(string userId)
		{
			ApiAccountModel apiAccountModel = null;
			
			using (IDataContextAsync context = new GameManagerContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                _repository = new Repository<AspNetUser>(context, unitOfWork);                
                var currentTime = DateTime.Now;
                var entity = _repository.Find(userId);
				apiAccountModel = new ApiAccountModel();
				apiAccountModel.Id = entity.Id;
				apiAccountModel.FullName = entity.FullName;
				apiAccountModel.FirstName = entity.FirstName;
				apiAccountModel.LastName = entity.LastName;
				apiAccountModel.BirthDate = entity.BirthDate;
				apiAccountModel.Email = entity.Email;
				apiAccountModel.PhoneNumber = entity.PhoneNumber;
				apiAccountModel.Address = entity.Address;				
				apiAccountModel.CityId = entity.CityId;
			}
			return apiAccountModel;
		}
	}
}
