using BulkyWeb_Razor.Data;
using BulkyWeb_Razor.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWeb_Razor.Pages.Categories;

public class Index : PageModel
{
    private readonly ApplicationDbContext _db;
    public List<Category> CategoryList { get; set; }
    public Index(ApplicationDbContext db) => _db = db;

    public void OnGet() => CategoryList = _db.Categories.ToList();
}