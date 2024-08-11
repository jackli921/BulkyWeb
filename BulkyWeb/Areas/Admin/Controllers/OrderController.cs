using Bulk.DataAccess.Repository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers;
[Area("Admin")]
public class OrderController : Controller
{
    private IUnitOfWork _unitOfWork;
    public OrderController(IUnitOfWork unitofWork)
    {
        _unitOfWork = unitofWork;
    }
    // GET
    public IActionResult Index()
    {
        return View();
    }
    
    #region API Endpoints

    [HttpGet]
    public IActionResult GetAll()
    {
        List<OrderHeader> objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties:"ApplicationUser").ToList();
        return Json(new { data = objOrderHeaders });
    }
    
    #endregion
}