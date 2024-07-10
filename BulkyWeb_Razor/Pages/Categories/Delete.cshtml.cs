using BulkyWeb_Razor.Data;
using BulkyWeb_Razor.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWeb_Razor.Pages.Categories;
[BindProperties]
public class Delete : PageModel
{
    private readonly ApplicationDbContext _db;
    public Category Category { get; set; }
    public Delete(ApplicationDbContext db) => _db = db;
    public void OnGet(int id)
    {
        if (id != null && id != 0)
        {
            Category = _db.Categories.Find(id);
        }
    }

    public IActionResult OnPost()
    {
        Category? obj = _db.Categories.Find(Category.Id);
        if (obj == null) return this.NotFound();
        
        _db.Categories.Remove(obj);
        _db.SaveChanges();
        TempData["Success"] = "Category deleted successfully.";
        return RedirectToPage("Index");
    }
}