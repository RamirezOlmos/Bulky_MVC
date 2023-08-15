using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public ProductController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();

			return View(objProductList);
		}

		public IActionResult Upsert(int? id)
		{
			ProductVM productVM = new()
			{
				CategoryList = _unitOfWork.Category
				.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				}),
				Product = new Product()
			};

			if (id == null || id == 0)
			{
				//create
				return View(productVM);
			}
			else
			{
				//update
				productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
				return View(productVM);
			}
		}

		[HttpPost]
		public IActionResult Upsert(ProductVM productVM, IFormFile? file)
		{
			//if (newProduct.Name == newProduct.DisplayOrder.ToString())
			//{
			//	ModelState.AddModelError("name", "Display Order cannot match the name");
			//}

			if (ModelState.IsValid)
			{
				_unitOfWork.Product.Add(productVM.Product);
				_unitOfWork.Save();
				TempData["success"] = "Product created succesfully";
				return RedirectToAction("Index");
			}
			else
			{
				productVM.CategoryList = _unitOfWork.Category
				.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString(),
				});
				return View(productVM);
			}
		}

		public IActionResult Edit(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			Product? ProductFromDb = _unitOfWork.Product.Get(u => u.Id == id);
			//Product? ProductFromDb1 = _db.Categories.FirstOrDefault(u => u.Id == id);
			//Product? ProductFromDb2 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();

			if (ProductFromDb == null) { return NotFound(); }
			return View(ProductFromDb);
		}

		[HttpPost]
		public IActionResult Edit(Product editedProduct)
		{

			if (ModelState.IsValid)
			{
				_unitOfWork.Product.Update(editedProduct);
				_unitOfWork.Save();
				TempData["success"] = "Product updated succesfully";
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
			Product? ProductFromDb = _unitOfWork.Product.Get(u => u.Id == id);

			if (ProductFromDb == null) { return NotFound(); }
			return View(ProductFromDb);
		}

		[HttpPost, ActionName("Delete")]

		public IActionResult DeletePost(int? id)
		{
			Product? deleteProduct = _unitOfWork.Product.Get(u => u.Id == id);
			if (deleteProduct == null) { return NotFound(); }
			_unitOfWork.Product.Remove(deleteProduct);
			_unitOfWork.Save();
			TempData["success"] = "Product deleted succesfully";
			return RedirectToAction("Index");
		}
	}
}
