using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class CompanyController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public CompanyController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			List<Company> objCompanyList = _unitOfWork.Company
			.GetAll()
			.ToList();

			return View(objCompanyList);
		}

		public IActionResult Upsert(int? id)
		{
			if (id == null || id == 0)
			{
				//create
				return View(new Company());
			}
			else
			{
				//update
				Company companyObj = _unitOfWork.Company.Get(u => u.Id == id);
				return View(companyObj);
			}
		}

		[HttpPost]
		public IActionResult Upsert(Company companyObj)
		{
			//if (newProduct.Name == newProduct.DisplayOrder.ToString())
			//{
			//	ModelState.AddModelError("name", "Display Order cannot match the name");
			//}

			if (ModelState.IsValid)
			{
				if (companyObj.Id == 0)
				{
					_unitOfWork.Company.Add(companyObj);
				}
				else
				{
					_unitOfWork.Company.Update(companyObj);

				}
				_unitOfWork.Save();
				TempData["success"] = "Category created succesfully";
				return RedirectToAction("Index");
			}
			else
			{
				return View(companyObj);

			}
		}

		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			Company? CompanyFromDb = _unitOfWork.Company.Get(u => u.Id == id);
			//Company? CompanyFromDb1 = _db.Categories.FirstOrDefault(u => u.Id == id);
			//Company? CompanyFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

			if (CompanyFromDb == null) { return NotFound(); }
			return View(CompanyFromDb);
		}

		[HttpPost]
		public IActionResult Edit(Company editedCompany)
		{

			if (ModelState.IsValid)
			{
				_unitOfWork.Company.Update(editedCompany);
				_unitOfWork.Save();
				TempData["success"] = "Company updated succesfully";
				return RedirectToAction("Index");
			}
			return View();
		}


		#region API CALLS

		[HttpGet]
		public IActionResult GetAll()
		{
			List<Company> objCompanyList = _unitOfWork.Company
			.GetAll()
			.ToList();

			return Json(new { data = objCompanyList });
		}

		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var companyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
			if (companyToBeDeleted == null)
			{
				return Json(new { succes = false, message = "Error while deleting!" });
			}

			_unitOfWork.Company.Remove(companyToBeDeleted);
			_unitOfWork.Save();

			return Json(new { succes = true, message = "Deleted Successful!" });
		}

		#endregion
	}
}
