using Bulk.DataAccess.Repository;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController: Controller
{
       private readonly IUnitOfWork _unitOfWork;
       private readonly IWebHostEnvironment _webHostEnvironment;

       public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
       {
              _unitOfWork = unitOfWork;
              _webHostEnvironment = webHostEnvironment;
       }
       
       // GET
       public IActionResult Index()
       {
              List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();
              return this.View(objProductList);
       }
       
       // CREATE
       public IActionResult Upsert(int? id)
       {
              ProductVM ProductVM = new()
              {
                     CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                     {
                            Text = u.Name,
                            Value = u.Id.ToString()
                     }),
                     Product = new Product()
              };
              if (id == null || id == 0) // Create
              {
                     return this.View(ProductVM);
              }
              else // Update
              {
                     ProductVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                     return this.View(ProductVM);
              }
       }
       
       [HttpPost]
       public IActionResult Upsert(ProductVM productVM, IFormFile file)
       {
              if (this.ModelState.IsValid)
              {
                     string wwwRootPath = _webHostEnvironment.WebRootPath;
                     if (file != null)
                     {
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            string productPath = Path.Combine(wwwRootPath, @"images\product");

                            if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                            {
                                   // delete old file
                                   string oldFilePath = Path.Combine(wwwRootPath,productVM.Product.ImageUrl.TrimStart('\\'));
                                   if (System.IO.File.Exists(oldFilePath))
                                   {
                                          System.IO.File.Delete(oldFilePath);
                                   }
                            }
                            
                            using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
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
                     this.TempData["Success"] = "Product created successfully";
                     return this.RedirectToAction("Index");
              }
              else
              {
                     productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                     {
                            Text = u.Name,
                            Value = u.Id.ToString()
                     });
                     return this.View(productVM);
              };
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