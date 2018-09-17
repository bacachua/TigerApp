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
using EventManager.ApiModels;
using Repository.Pattern.Infrastructure;

namespace EventManager.BusinessService
{
	public interface IAccountBusinessService : IService<AspNetUser>
	{
		ApiAccountModel GetAccountInfo(string userId);
		ApiAccountModel GetAccountInfoByEmail(string email);

		ApiAccountModel GetAccountInfoByPhone(string phoneNumber);

		ApiAccountModel UpdateQRCode(string userId,string qrCode);

		IList<ApiAccountModel> GetAccounts(int page, int pageSize, int cityId, out int totalRow);

	}

	public class AccountBusinessService : Service<AspNetUser>, IAccountBusinessService
	{
		private readonly IRepositoryAsync<AspNetUser> _repository;
		public AccountBusinessService(IRepositoryAsync<AspNetUser> repository)
			: base(repository)
		{
			_repository = repository;
		}

		public ApiAccountModel UpdateQRCode(string userId, string qrCode)
		{
			ApiAccountModel model = null;
            using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				//_repository = new Repository<AspNetUser>(context, unitOfWork);
				AspNetUser uAccount = _repository.Queryable().Where(x => x.Id == userId).FirstOrDefault();
				if (uAccount == null) return null;
				model = new ApiAccountModel();
				uAccount.ObjectState = ObjectState.Modified;
				uAccount.QRCode = qrCode;
				//unitOfWork.BeginTransaction();

				_repository.InsertOrUpdateGraph(uAccount);
				unitOfWork.SaveChanges();
				//unitOfWork.Commit();
				model.Id = uAccount.Id;
				model.FirstName = uAccount.FirstName;
				model.LastName = uAccount.LastName;
				model.Email = uAccount.Email;
				model.PhoneNumber = uAccount.PhoneNumber;
				model.CityId = uAccount.CityId;
				model.BirthDate = uAccount.BirthDate;
				model.Address = uAccount.Address;
				model.QRCode = uAccount.QRCode;
				model.PasswordHash = uAccount.PasswordHash;
				model.DeviceId = uAccount.DeviceId;
				model.IdentityNumber = uAccount.IdentityNumber;
				model.SignatureImgPath = uAccount.SignatureImgPath;
				model.UserType = uAccount.UserType;
			}
			return model;
		}
		
		public ApiAccountModel GetAccountInfo(string userId)
		{
			ApiAccountModel model = null;
            using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				//_repository = new Repository<AspNetUser>(context, unitOfWork);
				var uAccount = _repository.Queryable().Where(x => x.Id == userId).FirstOrDefault();
				if (uAccount == null) return null;
				model = new ApiAccountModel();
				model.Id = uAccount.Id;
				model.FirstName = uAccount.FirstName;
				model.LastName = uAccount.LastName;
				model.Email = uAccount.Email;
				model.PhoneNumber = uAccount.PhoneNumber;
				model.CityId = uAccount.CityId;
				model.BirthDate = uAccount.BirthDate;
				model.Address = uAccount.Address;
				model.QRCode = uAccount.QRCode;
				model.PasswordHash = uAccount.PasswordHash;
				model.DeviceId = uAccount.DeviceId;
				model.IdentityNumber = uAccount.IdentityNumber;
				model.SignatureImgPath = uAccount.SignatureImgPath;
				model.UserType = uAccount.UserType;
			}
			return model;
		}

		public ApiAccountModel GetAccountInfoByEmail(string email)
		{
			ApiAccountModel model = null;
			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				//_repository = new Repository<AspNetUser>(context, unitOfWork);
				var uAccount = _repository.Queryable().Where(x => x.Email == email).FirstOrDefault();
				if (uAccount == null) return null;
				model = new ApiAccountModel();
				if (uAccount != null)
				{
					model.Id = uAccount.Id;
					model.FirstName = uAccount.FirstName;
					model.LastName = uAccount.LastName;
					model.Email = uAccount.Email;
					model.PhoneNumber = uAccount.PhoneNumber;
					model.CityId = uAccount.CityId;
					model.BirthDate = uAccount.BirthDate;
					model.Address = uAccount.Address;
					model.QRCode = uAccount.QRCode;
					model.PasswordHash = uAccount.PasswordHash;
					model.DeviceId = uAccount.DeviceId;
					model.IdentityNumber = uAccount.IdentityNumber;
					model.SignatureImgPath = uAccount.SignatureImgPath;
					model.UserType = uAccount.UserType;
				}
			}
			return model;
		}

		public ApiAccountModel GetAccountInfoByPhone(string phoneNumber)
		{
			ApiAccountModel model = null;
			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				//_repository = new Repository<AspNetUser>(context, unitOfWork);
				var uAccount = _repository.Queryable().Where(x => x.PhoneNumber.Trim() == phoneNumber.Trim()).FirstOrDefault();
				if (uAccount == null) return null;
				model = new ApiAccountModel();
				if (uAccount != null)
				{
					model.Id = uAccount.Id;
					model.FirstName = uAccount.FirstName;
					model.LastName = uAccount.LastName;
					model.Email = uAccount.Email;
					model.PhoneNumber = uAccount.PhoneNumber;
					model.CityId = uAccount.CityId;
					model.BirthDate = uAccount.BirthDate;
					model.Address = uAccount.Address;
					model.QRCode = uAccount.QRCode;
					model.PasswordHash = uAccount.PasswordHash;
					model.DeviceId = uAccount.DeviceId;
					model.IdentityNumber = uAccount.IdentityNumber;
					model.SignatureImgPath = uAccount.SignatureImgPath;
					model.UserType = uAccount.UserType;
				}
			}
			return model;
		}

		public IList<ApiAccountModel> GetAccounts(int page, int pageSize, int cityId, out int totalRow)
		{
			
			totalRow = 0;
			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				//_repository = new Repository<AspNetUser>(context, unitOfWork);
				var model = _repository.Query(x => x.CityId == cityId).Include(x=>x.UserCity).OrderBy(x => x.OrderByDescending(z => z.BirthDate)).SelectPage(page, pageSize, out totalRow).ToList().Select(
					c => new ApiAccountModel()
						{
							City = c.City,
							BirthDate = c.BirthDate,
							DeviceId = c.DeviceId,
							UserName = c.UserName,
							FirstName = c.FirstName,
							LastName = c.LastName,
							PhoneNumber = c.PhoneNumber,
							CityId = c.CityId,
							Email = c.Email,
							Id = c.Id,
							QRCode  = c.QRCode,
							UserType = c.UserType,
							SignatureImgPath = c.SignatureImgPath,
							Address = c.Address,
							CityName = c.UserCity.Name
						}
					);
				return model.ToList();				
			}
			
		}
	}
}
