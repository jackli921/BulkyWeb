using System.Security.Claims;
using Bulk.DataAccess.Repository;
using Bulky.Models;
using Bulky.Models.ViewModel;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace BulkyWeb.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize]
public class OrderController : Controller
{
    private IUnitOfWork _unitOfWork;
    [BindProperty]
    public OrderVM OrderVM { get; set; }
    public OrderController(IUnitOfWork unitofWork)
    {
        _unitOfWork = unitofWork;
    }
    // GET
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult Details(int orderId)
    {
        OrderVM = new()
        {
            OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
            OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
        };
        return View(OrderVM);
    }
    [Authorize(Roles = StaticDetails.Role_Admin+ "," + StaticDetails.Role_Employee)]
    [HttpPost]
    public IActionResult UpdateOrderDetail(int orderId)
    {
        var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
        orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
        orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
        orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
        orderHeaderFromDb.City = OrderVM.OrderHeader.City;
        orderHeaderFromDb.State = OrderVM.OrderHeader.State;
        orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;

        if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
        {
            orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
        }
        if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
        {
            orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
        }
        _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
        _unitOfWork.Save();

        TempData["Success"] = "Order Details Updated Successfully";
        return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id} );
    }
    
    [HttpPost]
    [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
    public IActionResult StartProcessing()
    {
        _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, StaticDetails.StatusInProcess);
        _unitOfWork.Save();
        TempData["Success"] = "Order Details Updated Successfully";
        return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id} );
    }
    [HttpPost]
    [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
    public IActionResult ShipOrder()
    {
        var orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVM.OrderHeader.Id);
        orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
        orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
        orderHeader.OrderStatus = StaticDetails.StatusShipped;
        orderHeader.ShippingDate = DateTime.Now;
        if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
        {
            orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
        }
        
        _unitOfWork.OrderHeader.Update(orderHeader);
        _unitOfWork.Save();
        TempData["Success"] = "Order Shipped Successfully";
        return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id} );
    }

    [HttpPost]
    [Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
    public IActionResult CancelOrder()
    {
        var orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == OrderVM.OrderHeader.Id);

        if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusApproved)
        {
            var options = new RefundCreateOptions
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = orderHeader.PaymentIntentId
            };

            var service = new RefundService();
            Refund refund = service.Create(options);
            
            _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, StaticDetails.StatusCancelled, StaticDetails.StatusRefunded);
        }
        else
        {
            _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, StaticDetails.StatusCancelled, StaticDetails.StatusCancelled);
        }
        _unitOfWork.Save();
        TempData["Success"] = "Order Cancelled Successfully";
        return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id} );
    }

    [ActionName("Details")]
    [HttpPost]
    public IActionResult Details_PAY_NOW()
    {
        OrderVM.OrderHeader =
            _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
        OrderVM.OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id,
            includeProperties: "Product");
        // stripe logic
        var domain = $"{Request.Scheme}://{Request.Host.Value}/"; // capture domain for redirect
        var options = new Stripe.Checkout.SessionCreateOptions 
        {
            SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.Id}",
            CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.Id}",
            LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(), // holds list of items
            Mode = "payment",
        };

        foreach (var item in OrderVM.OrderDetails)
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
        _unitOfWork.OrderHeader.UpdateStripePaymentId(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId); // paymentIntendId is null here and is only created only payment is successful
        _unitOfWork.Save();
        Response.Headers.Add("Location", session.Url); 
        return new StatusCodeResult(303);
    }
    
    public IActionResult PaymentConfirmation(int orderHeaderId)
    {
        OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId);
        if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
        {
            // this is an order by a company
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, StaticDetails.PaymentStatusApproved);
                _unitOfWork.Save();
            }
        }
        
        return View(orderHeaderId);
    }
    
    #region API Endpoints
    
    [HttpGet]
    public IActionResult GetAll(string status)
    {
        IEnumerable<OrderHeader> objOrderHeaders;

        if (User.IsInRole(StaticDetails.Role_Admin) || User.IsInRole(StaticDetails.Role_Employee))
        {
           objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
        }
        else
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            objOrderHeaders = _unitOfWork.OrderHeader.GetAll(x => x.ApplicationUserId == userId,
                includeProperties: "ApplicationUser");
        }
        
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