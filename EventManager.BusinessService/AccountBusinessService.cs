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
	public interface IAccountBusinessServiceService : IService<AspNetUser>
	{
		ApiAccountModel GetAccountInfo(string userId);
	}

	public class AccountBusinessServiceService : Service<AspNetUser>, IAccountBusinessServiceService
	{
		private readonly IRepositoryAsync<AspNetUser> _repository;
		public AccountBusinessServiceService(IRepositoryAsync<AspNetUser> repository)
			: base(repository)
		{
			_repository = repository;
		}

		public ApiAccountModel GetAccountInfo(string userId)
		{
			ApiAccountModel model = null;
            using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				//_repository = new Repository<AspNetUser>(context, unitOfWork);
				var uAccount = _repository.Queryable().Where(x => x.Id == userId).FirstOrDefault();
				model = new ApiAccountModel();
				model.Id = uAccount.Id;
				model.FirstName = uAccount.FirstName;
				model.LastName = uAccount.LastName;
				model.Email = uAccount.Email;
				model.PhoneNumber = uAccount.PhoneNumber;
				model.CityId = uAccount.CityId;
				model.BirthDate = uAccount.BirthDate;
				model.Address = uAccount.Address;
			}
			return model;
		}
	}
}
