using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CategoryController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public CategoryController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
			return View(objCategoryList);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Create(Category newCategory)
		{
			//if (newCategory.Name == newCategory.DisplayOrder.ToString())
			//{
			//	ModelState.AddModelError("name", "Display Order cannot match the name");
			//}

			if (ModelState.IsValid)
			{
				_unitOfWork.Category.Add(newCategory);
				_unitOfWork.Save();
				TempData["success"] = "Category created succesfully";
				return RedirectToAction("Index");
			}
			return View();
		}

		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
			//Category? categoryFromDb1 = _db.Categories.FirstOrDefault(u => u.Id == id);
			//Category? categoryFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

			if (categoryFromDb == null) { return NotFound(); }
			return View(categoryFromDb);
		}

		[HttpPost]
		public IActionResult Edit(Category editedCategory)
		{

			if (ModelState.IsValid)
			{
				_unitOfWork.Category.Update(editedCategory);
				_unitOfWork.Save();
				TempData["success"] = "Category updated succesfully";
				return RedirectToAction("Index");
			}
			return View();
		}

		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			Category? categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);

			if (categoryFromDb == null) { return NotFound(); }
			return View(categoryFromDb);
		}

		[HttpPost, ActionName("Delete")]

		public IActionResult DeletePost(int? id)
		{
			Category? deleteCategory = _unitOfWork.Category.Get(u => u.Id == id);
			if (deleteCategory == null) { return NotFound(); }
			_unitOfWork.Category.Remove(deleteCategory);
			_unitOfWork.Save();
			TempData["success"] = "Category deleted succesfully";
			return RedirectToAction("Index");
		}
	}
}
