
using Casestudy.Helpers;
using Casestudy.DAL.DomainClasses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Casestudy.DAL.DAO
{
    public class OrderDAO
    {
        private AppDbContext _db;
        public OrderDAO(AppDbContext ctx)
        {
            _db = ctx;
        }

        public async Task<List<Order>> GetAll(int id)
        {
            return await _db.Orders.Where(order => order.UserId == id).ToListAsync<Order>();
        }

        public async Task<List<OrderDetailsHelper>> GetTrayDetails(int tid, string email)
        {
            Customer user = _db.Customers.FirstOrDefault(user => user.Email == email);
            List<OrderDetailsHelper> allDetails = new List<OrderDetailsHelper>();
            // LINQ way of doing INNER JOINS
            var results = from t in _db.Orders
                          join ti in _db.OrderLineItems on t.Id equals ti.OrderId
                          join mi in _db.Products on ti.ProductId equals mi.Id
                          where (t.UserId == user.Id && t.Id == tid)
                          select new OrderDetailsHelper
                          {
                              OrderId = t.Id,
                              UserId = user.Id,
                              Name = mi.ProductName,
                              ProductId = ti.ProductId,
                              SellingPrice = ti.SellingPrice,
                              QtyS = ti.QtySold,
                              QtyB = ti.QtyBackOrdered,
                              QtyO = ti.QtyOrdered,
                              DateCreated = t.OrderDate.ToString("yyyy/MM/dd - hh:mm tt")
                          };
            allDetails = await results.ToListAsync<OrderDetailsHelper>();
            return allDetails;
        }
        public async Task<int> AddOrder(int customerid, OrderSelectionHelper[] selections)
        {
            int orderId = -1;
            using (_db)
            {
                // we need a transaction as multiple entities involved
                using (var _trans = await _db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        Order order = new Order();
                        order.UserId = customerid;
                        order.OrderDate = System.DateTime.Now;
                        order.OrderAmount = 0;

                        // calculate the totals and then add the order row to the table
                        foreach (OrderSelectionHelper selection in selections)
                        {
                            order.OrderAmount += selection.item.MSRP * selection.Qty;
                        }
                        await _db.Orders.AddAsync(order);
                        await _db.SaveChangesAsync();
                        // then add each item to the orderitems table
                        foreach (OrderSelectionHelper selection in selections)
                        {
                            OrderLineItem tItem = new OrderLineItem();
                            ProductDAO pDAO = new ProductDAO(_db);
                            Product product = await pDAO.GetProduct(selection.item.Id);
                            if (selection.Qty <= product.QtyOnHand)
                            {
                                product.QtyOnHand -= selection.Qty;
                                tItem.QtySold = selection.Qty;
                                tItem.QtyOrdered = selection.Qty;
                                tItem.QtyBackOrdered = 0;
                            }
                            else
                            {
                                product.QtyOnBackOrder = selection.Qty - product.QtyOnHand;
                                tItem.QtySold = product.QtyOnHand;
                                tItem.QtyOrdered = selection.Qty;
                                tItem.QtyBackOrdered = product.QtyOnBackOrder;
                                product.QtyOnHand = 0;
                            }
                            tItem.ProductId = selection.item.Id;
                            tItem.SellingPrice = selection.item.MSRP * selection.Qty;
                            tItem.OrderId = order.Id;
                            await _db.OrderLineItems.AddAsync(tItem);
                            await _db.SaveChangesAsync();
                        }
                        // test trans by uncommenting out these 3 lines
                        //int x = 1;
                        //int y = 0;
                        //x = x / y;
                        await _trans.CommitAsync();
                        orderId = order.Id;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        await _trans.RollbackAsync();
                    }
                }
            }
            return orderId;
        }
    }
}