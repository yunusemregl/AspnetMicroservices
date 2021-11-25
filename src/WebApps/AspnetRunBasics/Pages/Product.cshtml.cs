using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspnetRunBasics.Models;
using AspnetRunBasics.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspnetRunBasics
{
    public class ProductModel : PageModel
    {

        private readonly ICatalogService _catalogService;
        private readonly IBasketService _basketService;
        public ProductModel(ICatalogService catalogService, IBasketService basketService)
        {
            _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
            _basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
        }

        public IEnumerable<string> CategoryList { get; set; } = new List<string>();
        public IEnumerable<CatalogModel> ProductList { get; set; } = new List<CatalogModel>();


        [BindProperty(SupportsGet = true)]
        public string SelectedCategory { get; set; }

        public async Task<IActionResult> OnGetAsync(string categoryName)
        {
            var productList = await _catalogService.GetCatalog();

            CategoryList = productList.Select(c => c.Category).Distinct();

            if (!string.IsNullOrEmpty(categoryName))
            {
                ProductList = productList.Where(c => c.Category == categoryName).ToList();
                SelectedCategory = categoryName;
            }
            else
            {
                ProductList = productList;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync(string productId)
        {
            var product = await _catalogService.GetCatalog(productId);

            string username = "swn";
            var basket = await _basketService.GetBasket(username);

            basket.Items.Add(new BasketItemModel()
            {
                Color = "Black",
                Price = product.Price,
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = 1
            });

            await _basketService.UpdateBasket(basket);
            return RedirectToPage("Cart");
        }
    }
}