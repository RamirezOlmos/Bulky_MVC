using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ICategoryRepository _categoryRepo;
		public CategoryController(ICategoryRepository db)
		{
			_categoryRepo = db;
		}

		public IActionResult Index()
		{
			List<Category> objCategoryList = _categoryRepo.GetAll().ToList();
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
				_categoryRepo.Add(newCategory);
				_categoryRepo.Save();
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
			Category? categoryFromDb = _categoryRepo.Get(u => u.Id == id);
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
				_categoryRepo.Update(editedCategory);
				_categoryRepo.Save();
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
			Category? categoryFromDb = _categoryRepo.Get(u => u.Id == id);

			if (categoryFromDb == null) { return NotFound(); }
			return View(categoryFromDb);
		}

		[HttpPost, ActionName("Delete")]

		public IActionResult DeletePost(int? id)
		{
			Category? deleteCategory = _categoryRepo.Get(u => u.Id == id);
			if (deleteCategory == null) { return NotFound(); }
			_categoryRepo.Remove(deleteCategory);
			_categoryRepo.Save();
			TempData["success"] = "Category deleted succesfully";
			return RedirectToAction("Index");
		}
	}
}
