﻿using Domain.Enums;

namespace Domain.Models;

public class SupplierOrderDomain
{
    public int OrderId { get; set; }

    public DateTime DateOfOrder { get; set; }

    public float OrderCost { get; set; }

    public float OrderDiscount { get; set; }

    public DateTime DueDateForPayment { get; set; }

    public int SupplierId { get; set; }
    public int ManagerId { get; set; }
    public int ReceiverId { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public OrderStatus OrderStatus { get; set; }
}