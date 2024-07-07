using BulkyWeb.Controllers.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers;

public class CategoryController : Controller
{
    private readonly ApplicationDbContext _db;
    public CategoryController(ApplicationDbContext db) => _db = db;

    // GET
    public IActionResult Index()
    {
        List<Category> objCategoryList = _db.Categories.ToList();
        return View(objCategoryList);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public IActionResult Create(Category obj)
    {
        if (obj.Name == obj.DisplayOrder.ToString())
        {
            ModelState.AddModelError("Name", "The display order cannot exactly match the name");
        }
        
        if (ModelState.IsValid)
        {
            _db.Categories.Add(obj);
            _db.SaveChanges();
            TempData["Success"] = "Category created successfully.";
            return this.RedirectToAction("Index");
        }
        return View();
    }

    public IActionResult Edit(int? id)
    {
        if (id is null || id == 0) return this.NotFound();
        Category? categoryFromDb1 = _db.Categories.Find(id); // .Find() only works on Primary Keys
        if (categoryFromDb1 is null) return this.NotFound();
        return View(categoryFromDb1);
    }
    
    [HttpPost]
    public IActionResult Edit(Category obj)
    {
        if (ModelState.IsValid)
        {
            _db.Categories.Update(obj);
            _db.SaveChanges();
            TempData["Success"] = "Category updated successfully.";
            return this.RedirectToAction("Index");
        }
        return View();
    }

    public IActionResult Delete(int? id) // GET
    {
        if (id is null || id == 0) return this.NotFound();
        Category? categoryFromDb1 = _db.Categories.Find(id); // .Find() only works on Primary Keys
        if (categoryFromDb1 is null) return this.NotFound();
        return View(categoryFromDb1);
    }

    [HttpPost, ActionName("Delete")] // POST
    public IActionResult DeletePOST(int? id)
    {
        Category obj = _db.Categories.Find(id);
        if (obj is null) return this.NotFound();
        _db.Categories.Remove(obj);
        TempData["Success"] = "Category deleted successfully.";
        _db.SaveChanges();
        return this.RedirectToAction("Index");
    }
}