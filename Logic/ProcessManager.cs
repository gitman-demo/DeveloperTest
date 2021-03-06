﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTO.Entities;
using DTO.Context;

namespace Logic
{
	public class ProcessManager
	{     
        private DTOContext _context;

        public ProcessManager(DTOContext context) 
        { 
            _context = context; 
        }

        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns></returns>
        public List<Product> GetProducts()
        {
            return _context.Products.ToList();
        }

		/// <summary>
		/// Gets all customers.
		/// </summary>
		/// <returns></returns>
		public List<Customer> GetCustomers()
		{
            return _context.Customers.ToList();
		}

        /// <summary>
        /// Gets all sizes.
        /// </summary>
        /// <returns></returns>
        public List<Size> GetSizes()
        {
            return _context.Sizes.ToList();
        }

        /// <summary>
        /// Gets all Brands.
        /// </summary>
        /// <returns></returns>
        public List<Brand> GetBrands()
        {
            return _context.Brands.ToList();
        }

        /// <summary>
        /// Gets all Colours.
        /// </summary>
        /// <returns></returns>
        public List<Colour> GetColours()
        {
            return _context.Colours.ToList();
        }

		/// <summary>
		/// Performs a search for products using the supplied search parameters and returns a list of Product objects
		/// </summary>
		/// <param name="searchParameters">The product search parameters.</param>
		/// <returns></returns>
		public List<Product> ProductSearch(DTO.ProductSearchParameters searchParameters)
		{
            IQueryable<Product> Products;
            //Get all products first
            Products = _context.Products;

            //If brand ID's provided then add these to the where clause
            if (searchParameters.BrandIds.Count() > 0)
            {
                Products = Products.Where(x => searchParameters.BrandIds.Any(i => i == x.BrandId ));
            }

            //If size ID's provided then add these to the where clause
            if (searchParameters.SizeIds.Count() > 0)
            {
                Products = Products.Where(x => searchParameters.SizeIds.Any(i => i == x.SizeId));
            }

            //If colour ID's provided then add these to the where clause
            if (searchParameters.ColourIds.Count() > 0)
            {
                Products = Products.Where(x => searchParameters.ColourIds.Any(i => i == x.ColourId));
            }

            if(!String.IsNullOrEmpty(searchParameters.SearchString))
            {
                Products = SearchForProductsViaString(searchParameters.SearchString, Products);
            }
            
            return Products.ToList();
		}

		/// <summary>
		/// Performs a search for products using the supplied search parameters and returns a list of Product objects
		/// The DiscountedSellPrice for the products is calculated based on the DiscountGroup associated with the
		/// specified customer
		/// </summary>
		/// <param name="customerId">The customer id.</param>
		/// <param name="searchParameters">The product search parameters.</param>
		/// <returns></returns>
		public List<Product> CustomerProductSearch(int customerId, DTO.ProductSearchParameters searchParameters)
		{
            List<Product> Products;
            //Perform search on products
            Products = ProductSearch(searchParameters);
            //Lookup customer that has been provided
            Customer _Customer = _context.Customers.Include("DiscountGroup").Where(c => c.CustomerId == customerId).FirstOrDefault();
            if (_Customer != null)
            {
                //We have found a customer so lets update the discount price
                foreach (Product p in Products)
                {
                    p.DiscountedSellPrice = CalculateDiscountedPrice((decimal)_Customer.DiscountGroup.DiscountPercentage, p.SellPrice, p.CostPrice);
                }
            }

            return Products;
		}

        //Calculate discount amount for product based on a percentage
        public decimal CalculateDiscountedPrice(decimal discountPercentage, decimal sellPrice, decimal costPrice)
        {
            decimal discountAmount = ((discountPercentage / 100) * sellPrice);
            return ((sellPrice - discountAmount) < costPrice ? costPrice : (sellPrice - discountAmount));
        }

        //Search for products via search string
        public IQueryable<Product> SearchForProductsViaString(string searchString, IQueryable<Product> products)
        {
            //Split search words into array
            string[] searchTerms = searchString.Split(null);

            //Loop through each keyword and look for a match
            foreach (string item in searchTerms)
			{
                products = products.Where(p => p.ProductName.ToLower().Contains(item.ToLower()) 
                                          || p.Brand.BrandName.ToLower().Contains(item.ToLower())
                                          || p.Colour.ColourName.ToLower().Contains(item.ToLower()));
            }

            return products;                                    
        }
	}
}
