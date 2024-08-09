﻿using Bulk.DataAccess.Data;
using Bulky.Models;
using BulkyWeb.Models;
using System.Linq.Expressions;

namespace Bulk.DataAccess.Repository;

public class OrderHeaderRepository: Repository<OrderHeader>, IOrderHeaderRepository
{
    private ApplicationDbContext _db;
    public OrderHeaderRepository(ApplicationDbContext db) : base(db) => _db = db;

    public void Update(OrderHeader obj) => _db.Update(obj);
    public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
    {
        var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
        if (orderFromDb != null)
        {
            orderFromDb.OrderStatus = orderStatus;
            if (!string.IsNullOrEmpty(paymentStatus))
            {
                orderFromDb.PaymentStatus = paymentStatus;
            }
        }
    }

    public void UpdateSTripePaymentId(int id, string sessionId, string paymentIntentId)
    {
        var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
        if (!string.IsNullOrEmpty(sessionId))
        {
            orderFromDb.SessionId = sessionId;
        }

        if (!string.IsNullOrEmpty(paymentIntentId))
        {
            orderFromDb.PaymentIntentId = paymentIntentId;
            orderFromDb.PaymentDate = DateTime.Now;
        }
    }
}