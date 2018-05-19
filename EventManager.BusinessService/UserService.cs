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
    }
}
