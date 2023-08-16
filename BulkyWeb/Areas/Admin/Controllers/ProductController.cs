﻿using Bulky.DataAccess.Data;
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
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(IUnitOfWork unitOfWork,
			IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}

		public IActionResult Index()
		{
			List<Product> objProductList = _unitOfWork.Product
			.GetAll(includeProperties: "Category")
			.ToList();

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
				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if (file != null)
				{
					string fileName = Guid.NewGuid().ToString() +
						Path.GetExtension(file.FileName);
					string productPath = Path.Combine(wwwRootPath, @"images\product");

					if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
					{
						//delete old image
						var oldImagePath = Path.Combine(wwwRootPath,
											productVM.Product.ImageUrl.Trim('\\'));

						if (System.IO.File.Exists(oldImagePath))
						{
							System.IO.File.Delete(oldImagePath);
						}
					}

					using (var fileStream = new FileStream(Path.Combine(productPath,
						fileName), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}

					productVM.Product.ImageUrl = @"\images\product\" + fileName;
				}

				if (productVM.Product.Id == 0)
				{
					_unitOfWork.Product.Add(productVM.Product);
				}
				else
				{
					_unitOfWork.Product.Update(productVM.Product);

				}
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


		#region API CALLS

		[HttpGet]
		public IActionResult GetAll()
		{
			List<Product> objProductList = _unitOfWork.Product
			.GetAll(includeProperties: "Category")
			.ToList();

			return Json(new { data = objProductList });
		}

		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
			if (productToBeDeleted == null)
			{
				return Json(new { succes = false, message = "Error while deleting!" });
			}

			//delete old image
			var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
								productToBeDeleted.ImageUrl.Trim('\\'));

			if (System.IO.File.Exists(oldImagePath))
			{
				System.IO.File.Delete(oldImagePath);
			}

			_unitOfWork.Product.Remove(productToBeDeleted);
			_unitOfWork.Save();

			return Json(new { succes = true, message = "Deleted Successful!" });
		}

		#endregion
	}
}
