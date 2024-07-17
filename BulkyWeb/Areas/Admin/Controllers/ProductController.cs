using Bulk.DataAccess.Repository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController: Controller
{
       private readonly IUnitOfWork _unitOfWork;
       public ProductController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
       
       // GET
       public IActionResult Index()
       {
              List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();
              return this.View(objProductList);
       }
       
       // CREATE
       public IActionResult Create() => this.View();
       
       [HttpPost]
       public IActionResult Create(Product obj)
       {
              if (this.ModelState.IsValid)
              {
                     _unitOfWork.Product.Add(obj);
                     _unitOfWork.Save();
                     this.TempData["Success"] = "Product created successfully";
                     return this.RedirectToAction("Index");
              }
              return this.View();
       }
       
       // EDIT
       public IActionResult Edit(int? id)
       {
              if (id is null || id == 0) return this.NotFound();
              Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
              if (productFromDb is null) return this.NotFound();
              return this.View(productFromDb);
       }

       [HttpPost]
       public IActionResult Edit(Product obj)
       {
              if (this.ModelState.IsValid)
              {
                     _unitOfWork.Product.Update(obj);
                     _unitOfWork.Save();
                     this.TempData["Success"] = "Product updated successfully.";
                     return this.RedirectToAction("Index");
              }
              return this.View();
       }
       
       // DELETE
       public IActionResult Delete(int? id)
       {
              if (id is null || id == 0) return this.NotFound();
              Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
              if (productFromDb is null) return this.NotFound();
              return this.View(productFromDb);
       }

       [HttpPost, ActionName("Delete")]
       public IActionResult DeletePOST(int? id)
       {
              Product obj = _unitOfWork.Product.Get(u => u.Id == id);
              if (obj is null) return this.NotFound();
              _unitOfWork.Product.Remove(obj);
              this.TempData["Success"] = "Product deleted successfully.";
              _unitOfWork.Save();
              return this.RedirectToAction("Index");
       }
}