using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
	public class DiscountRepo : GenericRepository<Discount>, IDiscountRepo
	{
		public DiscountRepo(YUniContext context) : base(context)
		{
		}


		public async Task<List<Discount>> GetActiveDiscountsAsync()
		{
			return await _context.Discounts
				.Where(d => d.IsActive == true)
				.ToListAsync();
		}

		public async Task<List<Discount>> GetDiscountsByNameAsync(string discountName)
		{
			return await _context.Discounts
				.Where(d => d.DiscountName.Contains(discountName))
				.ToListAsync();
		}
	}
	
}
