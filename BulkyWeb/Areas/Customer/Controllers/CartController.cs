using System.Security.Claims;
using Bulk.DataAccess.Repository;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    [BindProperty]
    public ShoppingCartVM ShoppingCartVM { get; set; }

    public CartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    // GET
    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVM = new()
        {
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperties:"Product"),
            OrderHeader = new ()
        };

        foreach (var product in ShoppingCartVM.ShoppingCartList)
        {
            product.Price = GetPriceBasedOnQuantity(product);
            ShoppingCartVM.OrderHeader.OrderTotal += product.Price * product.Count;
        }
        
        return View(ShoppingCartVM);
    }

    public IActionResult Summary()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVM = new()
        {
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperties:"Product"),
            OrderHeader = new ()
        };

        ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

        ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
        ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
        ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
        ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
        ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
        ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
        ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
        
        foreach (var product in ShoppingCartVM.ShoppingCartList)
        {
            product.Price = GetPriceBasedOnQuantity(product);
            ShoppingCartVM.OrderHeader.OrderTotal += product.Price * product.Count;
        }
        return View(ShoppingCartVM);
    }
    [HttpPost]
    [ActionName("Summary")]
    public IActionResult SummaryPOST()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVM.ShoppingCartList =
            _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product");
        ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
        ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
        
        ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
        
        foreach (var product in ShoppingCartVM.ShoppingCartList)
        {
            product.Price = GetPriceBasedOnQuantity(product);
            ShoppingCartVM.OrderHeader.OrderTotal += product.Price * product.Count;
        }
        
        // check if it's a company account
        if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        {
            // regular customer accoujnt and we need to capture payment
            ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusPending;
            
        }
        else
        {
            // company user
            ShoppingCartVM.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
            ShoppingCartVM.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
        }
        _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
        _unitOfWork.Save();

        foreach (var item in ShoppingCartVM.ShoppingCartList)
        {
            OrderDetail orderDetail = new()
            {
                ProductId = item.ProductId,
                OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                Price = item.Price,
                Count = item.Count
            };
            _unitOfWork.OrderDetail.Add(orderDetail);
            _unitOfWork.Save();
        }

        if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        {
            // it is a regular customer accounta nd we need to capture payment
            // stripe logic
        }
        
        return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
    }

    public IActionResult OrderConfirmation(int id)
    {
        return View(id);
    }
    public IActionResult Plus(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId);
        cartFromDb.Count += 1;
        _unitOfWork.ShoppingCart.Update(cartFromDb);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }
    
    public IActionResult Minus(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId);
        if (cartFromDb.Count <= 1)
        {
            // remove from cart
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
        }
        else
        {
            cartFromDb.Count -= 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
        }
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }
    
    public IActionResult Remove(int cartId)
    {
        var cartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId);
        _unitOfWork.ShoppingCart.Remove(cartFromDb);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }

    private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
    {
        if (shoppingCart.Count <= 50)
        {
            return shoppingCart.Product.Price;
        }
        else
        {
            if (shoppingCart.Count <= 100)
            {
                return shoppingCart.Product.Price50;
            }
            else
            {
                return shoppingCart.Product.Price100;
            }
        }
    }
}