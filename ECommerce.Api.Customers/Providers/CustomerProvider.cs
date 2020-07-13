using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ECommerce.Api.Customers.Db;
using ECommerce.Api.Customers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Customer = ECommerce.Api.Customers.Models.Customer;

namespace ECommerce.Api.Customers.Providers
{
    public class CustomerProvider: ICustomerProvider
    {
        private DbCustomerContext _dbContext { get; set; }
        private IMapper _mapper { get; set; }
        private ILogger<CustomerProvider> _logger { get; set; }

        public CustomerProvider(DbCustomerContext dbContext, IMapper mapper, ILogger<CustomerProvider> logger)
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
            this._logger = logger;

            SeedData();
        }

        private void SeedData()
        {
            if (!_dbContext.Customers.Any())
            {
                _dbContext.Customers.Add(new Db.Customer { Id = 1, Name = "Jhon",  Address = "Grodno, Space 20"});
                _dbContext.Customers.Add(new Db.Customer { Id = 2, Name = "Sarah", Address = "Minsk, Nezalejnasti 9"});
                _dbContext.Customers.Add(new Db.Customer { Id = 3, Name = "Kate", Address = "Brest, Sovetskai 20"});
                _dbContext.Customers.Add(new Db.Customer { Id = 4, Name = "Mike", Address = "NY, Tower 54"});
                _dbContext.SaveChanges();
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Customer> Customers, string ErrorMessage)> GetCustomersAsync()
        {
            try
            {
                var customers = await _dbContext.Customers.ToListAsync();
                if (customers == null || !customers.Any())
                {
                    return (false, null, "Not found");
                }

                var result = _mapper.Map<IEnumerable<Db.Customer>, IEnumerable<Models.Customer>>(customers);
                return (true, result, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, Customer Customer, string ErrorMessage)> GetCustomerAsync(int id)
        {
            try
            {
                var customer = await _dbContext.Customers.FirstOrDefaultAsync(p => p.Id == id);
                if (customer == null)
                {
                    return (false, null, "Not found");
                }

                var result = _mapper.Map<Db.Customer, Models.Customer>(customer);
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