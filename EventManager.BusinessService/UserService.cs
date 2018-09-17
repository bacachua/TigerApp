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
using System.Linq.Expressions;

namespace EventManager.BusinessService
{    
    public interface IUserService
    {
        void SaveSignatureImage(string userId, string imgPath);
        void SaveDeviceToken(string userId, string token);
        void SendNotificationToUserByCity(int cityId, string message);
    }
    public class UserService: IUserService
    {
        private IRepositoryAsync<AspNetUser> _userRepository;
        public void SaveSignatureImage(string userId, string imgPath)
        {
            using (IDataContextAsync context = new GameManagerContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                _userRepository = new Repository<AspNetUser>(context, unitOfWork);
                var user = _userRepository.Find(userId);
                user.SignatureImgPath = imgPath;
                _userRepository.Update(user);
                unitOfWork.SaveChanges();
            }
        }
        public void SaveDeviceToken(string userId, string token)
        {
            using (IDataContextAsync context = new GameManagerContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                _userRepository = new Repository<AspNetUser>(context, unitOfWork);
                var user = _userRepository.Find(userId);
                user.DeviceId = token;
                _userRepository.Update(user);
                unitOfWork.SaveChanges();
            }
        }
        public void SendNotificationToUserByCity(int cityId,string message)
        {			
			INotificationService srvNotification = new NotificationService();
			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				_userRepository = new Repository<AspNetUser>(context, unitOfWork);
				var users = _userRepository.Filter(c => c.CityId == cityId).ToList();
				foreach (var user in users)
				{
					if (!string.IsNullOrEmpty(user.DeviceId))
					{
						srvNotification.NotifyAsync(user.DeviceId, message);
					}
				}
			}
        }
    }
}
