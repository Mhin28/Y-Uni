using Repositories.Models;
using Repositories.Repositories;
using Repositories.ViewModels.ExpensesCategoryModel;
using Repositories.ViewModels.ResultModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Services.Services.ExpensesCategoryService
{
	public class ExpensesCategoryService : IExpensesCategoryService
	{
		private readonly IExpensesCategoryRepo _repo;

		public ExpensesCategoryService(IExpensesCategoryRepo repo)
		{
			_repo = repo;
		}

		public async Task<ResultModel> GetAllAsync()
		{
			var result = new ResultModel();
			try
			{
				var expensesCategories = await _repo.GetAllAsync(ec => ec.Expenses, ec => ec.Budgets);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = expensesCategories;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> GetByIdAsync(Guid id)
		{
			var result = new ResultModel();
			try
			{
				var expensesCategorie = await _repo.GetByIdAsync(id);
				if (expensesCategorie == null)
				{
					result.IsSuccess = false;
					result.Code = (int)HttpStatusCode.NotFound;
					result.Message = "Expenses Categorie not found.";
					return result;
				}
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = expensesCategorie;
			}
			catch (Exception ex)
			{
				result.IsSuccess = false;
				result.Code = (int)HttpStatusCode.InternalServerError;
				result.Message = ex.Message;
			}
			return result;
		}

		public async Task<ResultModel> AddAsync(PostExpensesCategoryModel model)
		{
			var result = new ResultModel
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Invalid request"
			};

			try
			{
				var expensesCategory = new ExpensesCategory
				{
					ExCid = Guid.NewGuid(),
					CategoryName = model.CategoryName,
					Description = model.Description
				};
				await _repo.CreateAsync(expensesCategory);
				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.Created;
				result.Data = model;
				result.Message = "Expense Category created successfully";
			}
			catch (Exception ex)
			{
				result.Message = ex.Message;
			}

			return result;
		}

		public async Task<ResultModel> UpdateAsync(ExpensesCategoryModel model)
		{
			var result = new ResultModel
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Update failed"
			};

			try
			{
				var existing = await _repo.GetByIdAsync(model.ExCid);
				if (existing == null)
				{
					result.Message = "Category not found";
					return result;
				}

				if (!string.IsNullOrEmpty(model.CategoryName))
					existing.CategoryName = model.CategoryName;

				if (!string.IsNullOrEmpty(model.Description))
					existing.Description = model.Description;

				await _repo.UpdateAsync(existing);

				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Data = existing;
				result.Message = "Updated successfully";
			}
			catch (Exception ex)
			{
				result.Message = ex.Message;
			}

			return result;
		}

		public async Task<ResultModel> DeleteAsync(Guid id)
		{
			var result = new ResultModel
			{
				IsSuccess = false,
				Code = (int)HttpStatusCode.BadRequest,
				Message = "Delete failed"
			};

			try
			{
				var model = await _repo.GetByIdAsync(id);
				if (model == null)
				{
					result.Message = "Category not found";
					return result;
				}

				await _repo.RemoveAsync(model);

				result.IsSuccess = true;
				result.Code = (int)HttpStatusCode.OK;
				result.Message = "Deleted successfully";
			}
			catch (Exception ex)
			{
				result.Message = ex.Message;
			}

			return result;
		}
	}
}
