﻿using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
	public interface IPaymentMethodRepo : IGenericRepository<PaymentMethod>
	{
		Task<List<PaymentMethod>> GetActiveAsync();
	}
}
