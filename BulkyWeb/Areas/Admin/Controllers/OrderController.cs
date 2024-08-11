using Bulk.DataAccess.Repository;
using Bulky.Models;
using Bulky.Utility;
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
    public IActionResult GetAll(string status)
    {
        IEnumerable<OrderHeader> objOrderHeaders =
            _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
        switch (status) {
            case "pending":
                objOrderHeaders =
                    objOrderHeaders.Where(u => u.PaymentStatus == StaticDetails.StatusPending);
                break;
            case "inprocess":
                objOrderHeaders =
                    objOrderHeaders.Where(u => u.PaymentStatus == StaticDetails.StatusInProcess);
                break;
            case "approved":
                objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == StaticDetails.StatusApproved);
                break;
            case "completed":
                objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == StaticDetails.StatusShipped);
                break;
            default:
                break;
        };
        return Json(new { data = objOrderHeaders });
    }

    #endregion
}