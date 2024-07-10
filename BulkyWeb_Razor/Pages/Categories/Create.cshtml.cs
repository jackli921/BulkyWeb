using BulkyWeb_Razor.Data;
using BulkyWeb_Razor.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWeb_Razor.Pages.Categories;

public class Create : PageModel
{
    private readonly ApplicationDbContext _db;
    [BindProperty]
    public Category Category { get; set; }
    public Create(ApplicationDbContext db) => _db = db;

    public void OnGet()
    {
        
    }

    public IActionResult OnPost()
    {
        _db.Categories.Add(Category);
        _db.SaveChanges();
        TempData["Success"] = "Category added successfully.";
        return this.RedirectToPage("Index");
    }
}