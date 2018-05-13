﻿using System;
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
	public interface IAccountBusinessServiceService : IService<AspNetUser>
	{
		
	}

	public class AccountBusinessServiceService : Service<AspNetUser>, IAccountBusinessServiceService
	{
		private readonly IRepositoryAsync<AspNetUser> _repository;
		public AccountBusinessServiceService(IRepositoryAsync<AspNetUser> repository)
			: base(repository)
		{
            //_repository = null;
            _repository = repository;
		}

	
	}
}