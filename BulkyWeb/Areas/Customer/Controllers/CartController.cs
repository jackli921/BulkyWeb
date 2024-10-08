using System.Security.Claims;
using Bulk.DataAccess.Repository;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

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
            // it is a regular customer account, so we need to capture payment
            // stripe logic
            var domain = $"{Request.Scheme}://{Request.Host.Value}/"; // capture domain for redirect

            var options = new Stripe.Checkout.SessionCreateOptions 
            {
                SuccessUrl = domain + $"customer/cart/OrderConfirmation/{ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + "customer/cart/index",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(), // holds list of items
                Mode = "payment",
            };

            foreach (var item in ShoppingCartVM.ShoppingCartList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price) * 100,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }
            
            var service = new Stripe.Checkout.SessionService(); 
            Session session = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId); // paymentIntendId is null here and is only created only payment is successful
            _unitOfWork.Save();
            
            Response.Headers.Add("Location", session.Url); 
            return new StatusCodeResult(303);
        }
        
        return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
    }

    public IActionResult OrderConfirmation(int id)
    {
        OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
        if (orderHeader.PaymentStatus != StaticDetails.PaymentStatusDelayedPayment)
        {
            // this is an order by customer
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(id, StaticDetails.StatusApproved, StaticDetails.PaymentStatusApproved);
                _unitOfWork.Save();
            }
            HttpContext.Session.Clear();
        }

        List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
            .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
        _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
        _unitOfWork.Save();
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
        var cartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId, tracked:true);
        if (cartFromDb.Count <= 1)
        {
            // remove from cart
            HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == cartFromDb.ApplicationUserId).Count() );
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
        var cartFromDb = _unitOfWork.ShoppingCart.Get(x => x.Id == cartId, tracked:true);
        HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == cartFromDb.ApplicationUserId).Count() );
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