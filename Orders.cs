using cart;
using System;

namespace orders;

class Order
{
      public List<CartItem> items = new List<CartItem>(); 
      public DateTime checkOutDate;
      public double total = 0;
      public Guid orderId;
}