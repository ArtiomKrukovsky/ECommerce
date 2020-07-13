using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Api.Orders.Db;
using ECommerce.Api.Orders.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Order = ECommerce.Api.Orders.Models.Order;

namespace ECommerce.Api.Orders.Providers
{
    public class OrderProvider : IOrderProvider
    {
        private readonly ILogger<OrderProvider> _logger;
        private OrderDbContext _dbContext { get; set; }
        private IMapper _mapper { get; set; }

        public OrderProvider(OrderDbContext dbContext, IMapper mapper, ILogger<OrderProvider> logger)
        {
            this._logger = logger;
            this._dbContext = dbContext;
            this._mapper = mapper;

            SeedData();
        }

        private void SeedData()
        {
            if (!_dbContext.OrderItems.Any())
            {
                _dbContext.OrderItems.Add(new Db.OrderItem(){Id = 1, OrderId = 2, ProductId = 4, Quantity = 3, UnitPrice = 10});
                _dbContext.OrderItems.Add(new Db.OrderItem(){Id = 2, OrderId = 1, ProductId = 1, Quantity = 10, UnitPrice = 45});
                _dbContext.OrderItems.Add(new Db.OrderItem(){Id = 3, OrderId = 3, ProductId = 1, Quantity = 4, UnitPrice = 50});
                _dbContext.OrderItems.Add(new Db.OrderItem(){Id = 4, OrderId = 4, ProductId = 2, Quantity = 23, UnitPrice = 20});
                _dbContext.SaveChanges();
            }

            if (!_dbContext.Orders.Any())
            {
                _dbContext.Orders.Add(new Db.Order { Id = 1, CustomerId = 2, Total = 4, OrderDate = DateTime.Now, Items = _dbContext.OrderItems.Where(order => order.OrderId == 1).ToList()});
                _dbContext.Orders.Add(new Db.Order { Id = 2, CustomerId = 1, Total = 20, OrderDate = DateTime.Now, Items = _dbContext.OrderItems.Where(order => order.OrderId == 2).ToList() });
                _dbContext.Orders.Add(new Db.Order { Id = 3, CustomerId = 4, Total = 12, OrderDate = DateTime.Now, Items = _dbContext.OrderItems.Where(order => order.OrderId == 3).ToList() });
                _dbContext.Orders.Add(new Db.Order { Id = 4, CustomerId = 3, Total = 44, OrderDate = DateTime.Now, Items = _dbContext.OrderItems.Where(order => order.OrderId == 4).ToList() });
                _dbContext.SaveChanges();
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Order> Orders, string ErrorMessage)> GetOrdersAsync(int customerId)
        {
            try
            {
                var orders = await _dbContext.Orders.Where(order => order.CustomerId == customerId).ToListAsync();
                if (orders == null || !orders.Any())
                {
                    return (false, null, "Not found");
                }

                var result = _mapper.Map<IEnumerable<Db.Order>, IEnumerable<Models.Order>>(orders);
                return (true, result, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }
    }
}