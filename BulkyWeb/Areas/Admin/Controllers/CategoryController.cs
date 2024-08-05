using Bulk.DataAccess.Repository;
using Bulky.Models;
using Bulky.Utility;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers;

[Area("Admin")]
// [Authorize(Roles = StaticDetails.Role_Admin)]
public class CategoryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public CategoryController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    // GET
    public IActionResult Index()
    {
        List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
        return this.View(objCategoryList);
    }

    public IActionResult Create() => this.View();

    [HttpPost]
    public IActionResult Create(Category obj)
    {
        if (obj.Name == obj.DisplayOrder.ToString())
        {
            this.ModelState.AddModelError("Name", "The display order cannot exactly match the name");
        }
        
        if (this.ModelState.IsValid)
        {
            _unitOfWork.Category.Add(obj);
            _unitOfWork.Save();
            this.TempData["Success"] = "Category created successfully.";
            return this.RedirectToAction("Index");
        }
        return this.View();
    }

    public IActionResult Edit(int? id)
    {
        if (id is null || id == 0) return this.NotFound();
        Category? categoryFromDb1 = _unitOfWork.Category.Get(u => u.Id == id); // .Find() only works on Primary Keys
        if (categoryFromDb1 is null) return this.NotFound();
        return this.View(categoryFromDb1);
    }
    
    [HttpPost]
    public IActionResult Edit(Category obj)
    {
        if (this.ModelState.IsValid)
        {
            _unitOfWork.Category.Update(obj);
            _unitOfWork.Save();
            this.TempData["Success"] = "Category updated successfully.";
            return this.RedirectToAction("Index");
        }
        return this.View();
    }

    public IActionResult Delete(int? id) // GET
    {
        if (id is null || id == 0) return this.NotFound();
        Category? categoryFromDb1 = _unitOfWork.Category.Get(u => u.Id == id); // .Find() only works on Primary Keys
        if (categoryFromDb1 is null) return this.NotFound();
        return this.View(categoryFromDb1);
    }

    [HttpPost, ActionName("Delete")] // POST
    public IActionResult DeletePOST(int? id)
    {
        Category obj = _unitOfWork.Category.Get(u => u.Id == id);
        if (obj is null) return this.NotFound();
        _unitOfWork.Category.Remove(obj);
        this.TempData["Success"] = "Category deleted successfully.";
        _unitOfWork.Save();
        return this.RedirectToAction("Index");
    }
}