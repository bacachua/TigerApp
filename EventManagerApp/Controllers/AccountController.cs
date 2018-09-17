using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using EventManager.Web.Models;
using EventManager.Web.Providers;
using EventManager.Web.Results;
using Repository.Pattern.DataContext;
using Repository.Pattern.UnitOfWork;
using EventManager.DataModel.Models;
using Repository.Pattern.Ef6;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using EventManager.BusinessService;
using System.Data.Entity.Validation;
using System.Linq;
using EventManager.ApiModels;
using System.Drawing;
using System.IO;
using EventManager.Web.Filters;
using System.Net;

namespace EventManager.Web.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // GET api/Account/UserInfo
       
        [Route("GetUserInfo")]
		[AllowAnonymous]
		public APIResponse GetUserInfo(string userid)
        {
            using (IDataContextAsync context = new GameManagerContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IRepositoryAsync<AspNetUser> customerRepository = new Repository<AspNetUser>(context, unitOfWork);
				AccountBusinessService AccountBusinessServiceService = new AccountBusinessService(customerRepository);
                ApiAccountModel apiAccountModel  = AccountBusinessServiceService.GetAccountInfo(userid);
				if (apiAccountModel == null)
				{
					return new APIResponse() { Status = eResponseStatus.Fail, Result = "Không tìm thấy tài khoản thành viên" };
				}
				return new APIResponse() { Status = eResponseStatus.Success, Result = apiAccountModel };
            }
        }

		[Route("UpdateUserQRCode")]
		[AllowAnonymous]
		[HttpPost]
		public APIResponse UpdateUserQRCode(string userid, string qrCode)
		{
			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				IRepositoryAsync<AspNetUser> customerRepository = new Repository<AspNetUser>(context, unitOfWork);
				AccountBusinessService AccountBusinessServiceService = new AccountBusinessService(customerRepository);
				//ApiAccountModel apiAccountModel = AccountBusinessServiceService.UpdateQRCode(userid, qrCode);
				Tuple<bool, string> ret = UpdateUserQRCodetoDB(userid, qrCode);
				if (ret.Item1 == false)
				{
					return new APIResponse() { Status = eResponseStatus.Fail, Result = "Cập nhật không thành công" };
				}
				return new APIResponse() { Status = eResponseStatus.Success, Result = "Cậo nhật thành công" };
			}
		}


		//[Authorize]
		[Route("GetUserInfoByEmail")]
		[AllowAnonymous]
		public APIResponse GetUserInfoByEmail(string email)
		{

			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				IRepositoryAsync<AspNetUser> customerRepository = new Repository<AspNetUser>(context, unitOfWork);
				AccountBusinessService AccountBusinessServiceService = new AccountBusinessService(customerRepository);
				ApiAccountModel accountModel = AccountBusinessServiceService.GetAccountInfoByEmail(email);
				if (accountModel == null)
				{
					return new APIResponse() { Status = eResponseStatus.Fail, Result = "Không tìm thấy tài khoản thành viên" };
				}
				return new APIResponse() { Status = eResponseStatus.Success, Result = accountModel };
			}
		}

		[Route("GetUserInfoByPhoneNumber")]
		[AllowAnonymous]
		public APIResponse GetUserInfoByPhoneNumber(string phoneNumber)
		{

			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				IRepositoryAsync<AspNetUser> customerRepository = new Repository<AspNetUser>(context, unitOfWork);
				AccountBusinessService AccountBusinessServiceService = new AccountBusinessService(customerRepository);
				ApiAccountModel accountModel = AccountBusinessServiceService.GetAccountInfoByPhone(phoneNumber);
				if (accountModel == null)
				{
					return new APIResponse() { Status = eResponseStatus.Fail, Result = "Không tìm thấy tài khoản thành viên" };
				}
				return new APIResponse() { Status = eResponseStatus.Success, Result = accountModel };
			}
		}

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        // POST api/Account/ChangePassword
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        //public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)

		public async Task<APIResponse> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
				return new APIResponse() { Status = eResponseStatus.Fail, Result = BadRequest(ModelState) };
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                //return BadRequest("External login failure.");
				return new APIResponse() { Status = eResponseStatus.Fail, Result = "Không đăng nhập được" };
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                //return BadRequest("The external login is already associated with an account.");
				return new APIResponse() { Status = eResponseStatus.Fail, Result = "The external login is already associated with an account." };
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                //return GetErrorResult(result);
				return new APIResponse() { Status = eResponseStatus.Fail, Result = result };
            }
			return new APIResponse() { Status = eResponseStatus.Success, Result = "Đăng nhập thành công." };
            //return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        //public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)

		public async Task<APIResponse> GetExternalLogin(string provider, string error = null)		
        {
			//if (error != null)
			//{
			//	return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
			//}

            if (!User.Identity.IsAuthenticated)
            {
                ///return new ChallengeResult(provider, this);
				///
				return new APIResponse() { Status = eResponseStatus.Fail, Result = provider };
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
               // return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
				return new APIResponse() { Status = eResponseStatus.Fail, Result = provider };
               // return new ChallengeResult(provider, this);
            }

            ApplicationUser user =  await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

				//ClaimsIdentity oAuthIdentity =  user.GenerateUserIdentityAsync(UserManager,
				//   OAuthDefaults.AuthenticationType);
				//ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
				//	CookieAuthenticationDefaults.AuthenticationType);

				ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
				   OAuthDefaults.AuthenticationType);
				ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
					CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }
			return new APIResponse() { Status = eResponseStatus.Success, Result = "Đăng nhập thành công" };
          //  return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
		public APIResponse Register(RegisterBindingModel model)
		{
			
			if (!ModelState.IsValid)
			{
				
				return new APIResponse() { Status = eResponseStatus.Fail, Result = "Thông tin nhập vào không đúng, Vui lòng nhập đầy đủ thông tin" };
			}

			//var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
			var user = new ApplicationUser() { UserName = model.PhoneNumber, Email = model.Email };

            IdentityResult result =  UserManager.Create(user, model.Password);
            model.Id = user.Id;
			model.UserName = model.PhoneNumber;
            model.SecurityStamp = user.SecurityStamp;
            model.PasswordHash = user.PasswordHash;
			try
			{
				Tuple<bool, string> updateResult = AddUpdateUserInfo(model);				
			}catch(Exception ex)
			{

			}
            
			if (!result.Succeeded)
			{
				return new APIResponse() { Status = eResponseStatus.Fail, Result = string.Join(",",result.Errors.ToArray()) };
			}
		
			return new APIResponse() { Status = eResponseStatus.Success, Result = "Đăng ký thành viên thành công" };			
        }

        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            return Ok();
        }

		// PUT: api/EventRegister/5
		public APIResponse Put(AccountUpdateBindingModel model)
		{
			Tuple<bool, string> ret = UpdateUserInfo(model);
			return new APIResponse() { Status = eResponseStatus.Success, Result = ret.Item1 };
		}

		private Tuple<bool, string> AddUpdateUserInfo(RegisterBindingModel model)
		{
			Tuple<bool, string> ret = new Tuple<bool,string>(true,"Success");
			try
			{
				// TODO: Add update logic here
				AspNetUser aspNetUser = new AspNetUser();
				
				using (IDataContextAsync context = new GameManagerContext())
				using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
				{
					IRepositoryAsync<AspNetUser> customerRepository = new Repository<AspNetUser>(context, unitOfWork);
					AccountBusinessService AccountBusinessServiceService = new AccountBusinessService(customerRepository);
					
					aspNetUser = customerRepository.Queryable().Where(x => x.Id == model.Id).SingleOrDefault<AspNetUser>();
					aspNetUser.ObjectState = ObjectState.Modified;
					//aspNetUser.Id = model.Id;
					//DateTime actualDate = new DateTime(Convert.ToInt16(model.BirthDate.Substring(0, 4)), Convert.ToInt16(model.BirthDate.Substring(5, 2)), Convert.ToInt16(model.BirthDate.Substring(8, 2)));
					//aspNetUser.BirthDate = actualDate;
					aspNetUser.BirthDate = model.BirthDate;
					aspNetUser.Email = model.Email;
					aspNetUser.CityId = model.CityId;
					aspNetUser.FullName = model.FullName;
					aspNetUser.FirstName = model.FirstName;
					aspNetUser.LastName = model.LastName;
					aspNetUser.QRCode = model.QRCode;
				//	aspNetUser.CityId = model.CityId;
					aspNetUser.PhoneNumber = aspNetUser.UserName;
					aspNetUser.Address = model.Address;
				//	aspNetUser.PhoneNumber = aspNetUser.UserName;
					aspNetUser.SignatureImgPath = model.SignatureImgPath;
					aspNetUser.UserType = 0;
					customerRepository.InsertOrUpdateGraph(aspNetUser);
					unitOfWork.SaveChanges();
				}
			}
			catch (DbEntityValidationException ex)
			{
				ret = new Tuple<bool, string>(false, ex.Message);
			}
			catch (Exception ex)
			{			
				ret = new Tuple<bool, string>(false, ex.Message);
			}		
			return ret;
		}

		private Tuple<bool, string> UpdateUserInfo(AccountUpdateBindingModel model)
		{
			Tuple<bool, string> ret = new Tuple<bool, string>(true, "Success");
			try
			{
				// TODO: Add update logic here
				AspNetUser aspNetUser = new AspNetUser();

				using (IDataContextAsync context = new GameManagerContext())
				using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
				{
					IRepositoryAsync<AspNetUser> customerRepository = new Repository<AspNetUser>(context, unitOfWork);
					AccountBusinessService AccountBusinessServiceService = new AccountBusinessService(customerRepository);

					aspNetUser = customerRepository.Queryable().Where(x => x.Id == model.Id).SingleOrDefault<AspNetUser>();
					aspNetUser.ObjectState = ObjectState.Modified;
					//aspNetUser.Id = model.Id;
					//DateTime actualDate = new DateTime(Convert.ToInt16(model.BirthDate.Substring(0, 4)), Convert.ToInt16(model.BirthDate.Substring(5, 2)), Convert.ToInt16(model.BirthDate.Substring(8, 2)));
					//aspNetUser.BirthDate = actualDate;
					aspNetUser.BirthDate = model.BirthDate;
					aspNetUser.Email = model.Email;
					aspNetUser.CityId = model.CityId;
					aspNetUser.FullName = model.FullName;
					aspNetUser.FirstName = model.FirstName;
					aspNetUser.LastName = model.LastName;
					aspNetUser.QRCode = model.QRCode;		
					aspNetUser.Address = model.Address;			
					customerRepository.Update(aspNetUser);
					unitOfWork.SaveChanges();
				}
			}
			catch (DbEntityValidationException ex)
			{
				ret = new Tuple<bool, string>(false, ex.Message);
			}
			catch (Exception ex)
			{
				ret = new Tuple<bool, string>(false, ex.Message);
			}
			return ret;
		}

		private Tuple<bool, string> UpdateUserQRCodetoDB(string userid, string qrCode)
		{
			Tuple<bool, string> ret = new Tuple<bool, string>(true, "Success");
			try
			{
				// TODO: Add update logic here
				AspNetUser aspNetUser = new AspNetUser();

				using (IDataContextAsync context = new GameManagerContext())
				using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
				{
					IRepositoryAsync<AspNetUser> customerRepository = new Repository<AspNetUser>(context, unitOfWork);
					AccountBusinessService AccountBusinessServiceService = new AccountBusinessService(customerRepository);

					aspNetUser = customerRepository.Queryable().Where(x => x.Id == userid).SingleOrDefault<AspNetUser>();
					aspNetUser.ObjectState = ObjectState.Modified;
					
					aspNetUser.QRCode = qrCode;

				
					customerRepository.InsertOrUpdateGraph(aspNetUser);
					unitOfWork.SaveChanges();
				}
			}
			catch (DbEntityValidationException ex)
			{
				ret = new Tuple<bool, string>(false, ex.Message);
			}
			catch (Exception ex)
			{
				ret = new Tuple<bool, string>(false, ex.Message);
			}
			return ret;
		}
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UserManager.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
		[ValidateMimeMultipartContentFilter]
        [Route("PostSignatureImage")]
        [ImportFileParamType.SwaggerFormAttribute("ImportImage", "Upload image file")]
        [AllowAnonymous]
		[HttpPost]
		public APIResponse PostSignatureImage()
        {
            try
            {
                eResponseStatus status = eResponseStatus.Success;
				var httpPostedFile =  HttpContext.Current.Request.Files[0];
                var filePath = "Images/" + new Random().Next().ToString() + httpPostedFile.FileName;
                if (httpPostedFile != null)
                {
                    var fileSavePath = Path.Combine(HttpContext.Current.Server.MapPath("~/"), filePath);
                    httpPostedFile.SaveAs(fileSavePath);
                }
                //IUserService user = new UserService();
                //user.SaveSignatureImage(userId, filePath);
                return new APIResponse() { Status = status, Result = filePath };
            }
            catch (Exception ex)
            {
                return new APIResponse() { Status = eResponseStatus.Fail, Message = "Please Upload image of type .jpg,.gif,.png." };
            }

        }
		[AllowAnonymous]
		[HttpPost]
		public  HttpResponseMessage  PostFormData()
		{
			HttpResponseMessage response = new HttpResponseMessage();  
			var httpRequest = HttpContext.Current.Request;  
			if (httpRequest.Files.Count > 0)  
			{  
				foreach (string file in httpRequest.Files)  
				{  
				var postedFile = httpRequest.Files[file];
				var filePath = HttpContext.Current.Server.MapPath("~/Images/" + postedFile.FileName);  
				postedFile.SaveAs(filePath);  
				}  
			}  
			return response;
		}


        [ValidateMimeMultipartContentFilter]
        [HttpPost, Route("softwarepackage")]
        [AllowAnonymous]
        public APIResponse UploadSingleFile(HttpPostedFile file)
        {
            var streamProvider = new MultipartFormDataStreamProvider(HttpContext.Current.Server.MapPath("~/Images"));
            var task = Request.Content.ReadAsMultipartAsync(streamProvider);
            var firstFile = streamProvider.FileData.FirstOrDefault();

            return null;
        }


        [Route("SaveDeviceToken/{userId}")]
        [AllowAnonymous]
        public APIResponse SaveDeviceToken(string userId,string token)
        {
            try
            {
                IUserService user = new UserService();
                user.SaveDeviceToken(userId, token);
                return new APIResponse();
            }
            catch (Exception ex)
            {
                return new APIResponse() { Status = eResponseStatus.Fail, Message = "Please Upload image of type .jpg,.gif,.png." };
            }
        }

		[HttpPost]
		[Route("SendNotificationByDevice")]
		[AllowAnonymous]
		public APIResponse SendNotificationByDevice(string token, string message)
		{
			//INotificationService user = new NotificationService();
			//user.NotifyAsync(token, message);

			string test = RequestContext.Principal.Identity.Name;
			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				IRepositoryAsync<AspNetUser> _repository = new Repository<AspNetUser>(context, unitOfWork);
				List<AspNetUser> uAccounts = _repository.Queryable().Where(x => x.DeviceId.Trim() == token.Trim()).ToList();
				foreach (AspNetUser user in uAccounts)
				{
					IContentBusinessService contentBusinessService = new ContentBusinessService();
					MessageContent messageContent = new MessageContent();
					if (!string.IsNullOrEmpty(user.DeviceId))
					{
						messageContent.Receiver = user.DeviceId;
						messageContent.BodyMessage = message;
						messageContent.ServiceTypeID = 0;
						messageContent.Status = 0;
						messageContent.ModifiedDate = DateTime.Now;
						messageContent.CreatedDate = DateTime.Now;
						messageContent.Sender = "TigerWall";
						messageContent.UserId = user.Id;
						contentBusinessService.InserContentMessage(messageContent);
						
					}
				}
			}
			return new APIResponse() { Status = eResponseStatus.Success , Result = "success" };
		}

		[HttpPost]
		[Route("SendNotificationBeforeOrLate")]
		[AllowAnonymous]
		public APIResponse SendNotificationBeforeOrLateEventTime(int numOfMinute, string token, string message,int status)
		{
			if (status != (int)eEventRegisterStatus.Late || status == (int)eEventRegisterStatus.Reminded)
			{
				return new APIResponse() { Status = eResponseStatus.Fail, Result = "Hệ thống chỉ hỗ trợ gửi thông báo trước giờ chơi, hoặc trễ giờ chơi" };
			}
			try
			{
				IEventCampaignBusinessService eventCampaignBusinessService = new EventCampaignBusinessService();
				if (status == (int)eEventRegisterStatus.Late)
					eventCampaignBusinessService.SendNotificationBeforeOrLateEventTime(numOfMinute, token, message, eEventRegisterStatus.Late);
				else
					eventCampaignBusinessService.SendNotificationBeforeOrLateEventTime(numOfMinute, token, message, eEventRegisterStatus.Reminded);
				return new APIResponse() { Status = eResponseStatus.Success, Result = "Thông báo đã gửi thành công" };
			}
			catch(Exception ex)
			{
				return new APIResponse() { Status = eResponseStatus.Fail, Result = "Lỗi gửi thông báo tới thiết bị:" + ex.Message };
			}			
		}

		[HttpPost]
		[Route("SendNotificationByType")]
		[AllowAnonymous]
		public APIResponse SendNotificationByType(string token, string message)
		{
			INotificationService user = new NotificationService();
			user.NotifyAsync(token, message);
			return new APIResponse() { Status = eResponseStatus.Success, Result = "success" };
		}

		
    }
}
