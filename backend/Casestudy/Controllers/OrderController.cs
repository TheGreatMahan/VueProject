using System;
using Casestudy.DAL;
using Casestudy.DAL.DAO;
using Casestudy.DAL.DomainClasses;
using Casestudy.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Casestudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        AppDbContext _ctx;
        public OrderController(AppDbContext context) // injected here
        {
            _ctx = context;
        }
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<string>> Index(OrderHelper helper)
        {
            string retVal = "";
            try
            {
                CustomerDAO uDao = new CustomerDAO(_ctx);
                Customer orderOwner = await uDao.GetByEmail(helper.email);
                OrderDAO tDao = new OrderDAO(_ctx);
                int orderId = await tDao.AddOrder(orderOwner.Id, helper.selections);
                if (orderId > 0)
                {
                    retVal = "Order " + orderId + " saved!";
                }
                else
                {
                    retVal = "Order not saved1";
                }
            }
            catch (Exception ex)
            {
                retVal = "Order not savedx " + ex.Message;
            }
            return retVal;
        }


        [Route("{email}")]
        public async Task<ActionResult<List<Order>>> List(string email)
        {
            List<Order> trays = new List<Order>();
            CustomerDAO uDao = new CustomerDAO(_ctx);
            Customer trayOwner = await uDao.GetByEmail(email);
            OrderDAO tDao = new OrderDAO(_ctx);
            trays = await tDao.GetAll(trayOwner.Id);
            return trays;
        }


        [Route("{trayid}/{email}")]
        public async Task<ActionResult<List<OrderDetailsHelper>>> GetTrayDetails(int trayid, string email)
        {
            OrderDAO dao = new OrderDAO(_ctx);
            return await dao.GetTrayDetails(trayid, email);
        }

    }
}