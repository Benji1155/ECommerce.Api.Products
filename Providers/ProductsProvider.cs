using ECommerce.Api.Products.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ECommerce.Api.Products.Models;
using AutoMapper;

namespace ECommerce.Api.Products.Providers
{
    public class ProductsProvider : IProductsProvider
    {
        private readonly Db.ProductsDbContext dbContext;
        private readonly ILogger<ProductsProvider> logger;
        private readonly IMapper mapper;

        public ProductsProvider(Db.ProductsDbContext dbContext, ILogger<ProductsProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;
            SeedData();
        }

        private void SeedData()
        {
            if (!dbContext.Products.Any())
            {
                dbContext.Products.Add(new Db.Product() { Id = 1, Name = "Steel Sword", Description = "A simpple sword made of steel, a good choice for a beginner swordsman.", Price = 100, Category = "Swords", Inventory = 4 });
                dbContext.Products.Add(new Db.Product() { Id = 2, Name = "Huntsman's Bow", Description = "Used primarily for hunting wildlife, this bow could be used against your enemies with good results.", Price = 75, Category = "Bows", Inventory = 2 });
                dbContext.Products.Add(new Db.Product() { Id = 3, Name = "Crafted Arrow", Description = "Flint head attached to a wooden body topped with a gray feather, nothing fancy but gets the job done.", Price = 2, Category = "Arrows", Inventory = 54 });
                dbContext.Products.Add(new Db.Product() { Id = 4, Name = "Health Potion", Description = "Makes you feel young again. I hope it has no side effects", Price = 25, Category = "Potions", Inventory = 7 });
                dbContext.SaveChanges();
            }
        }

        public async Task<(bool IsSuccess, Product Product, string ErrorMessage)> GetProductAsync(int id)
        {
            try
            {
                logger?.LogInformation($"Querying products with id: {id}");
                var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product != null)
                {
                    logger?.LogInformation("Product found");
                    var result = mapper.Map<Product>(product);
                    return (true, result, null);
                }
                //gives error message
                return (false, null, "[error] This id doesn't match a valid entry, try again with a different id.");
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Product> Products, string ErrorMessage)> GetProductsAsync()
        {
            try
            {
                logger?.LogInformation("Querying products");
                var products = await dbContext.Products.ToListAsync();
                if (products!=null && products.Any())
                {
                    logger?.LogInformation($"{products.Count} product(s) found");
                    var result = mapper.Map<IEnumerable<Product>>(products);
                    return (true, result, null);
                }
                return (false, null, "Not found");  
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }
    }
}