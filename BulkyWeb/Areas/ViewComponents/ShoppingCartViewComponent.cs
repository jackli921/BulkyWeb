using System.Security.Claims;
using Bulk.DataAccess.Repository;
using Bulky.Utility;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.ViewComponents;

public class ShoppingCartViewComponent: ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;
    public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        if (claim != null)
        {
            if (HttpContext.Session.GetInt32(StaticDetails.SessionCart) == null)
            {
                HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).Count());
            }

            return View(HttpContext.Session.GetInt32(StaticDetails.SessionCart));
        }
        else
        {
            HttpContext.Session.Clear();
            return View(0);
        }
    }
}