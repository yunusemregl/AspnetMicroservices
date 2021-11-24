using Microsoft.AspNetCore.Mvc;
using Shopping.Aggregator.Models;
using Shopping.Aggregator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Shopping.Aggregator.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ShoppingController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;
        private readonly ICatalogService _catalogService;

        public ShoppingController(IBasketService basketService, IOrderService orderService, ICatalogService catalogService)
        {
            _basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
        }

        [HttpGet("{userName}", Name = "GetShopping")]
        [ProducesResponseType(typeof(ShoppingModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingModel>> GetShopping(string userName)
        {
            // get basket with username
            // iterate basket items and consume products with basket item productId member
            // map product related members into basket item dto with extended columns
            // consume ordering microservices in order to retrieve order list
            // return root ShoppingModel dto class which including all responses.

            BasketModel basket = await _basketService.GetBasket(userName);

            foreach (var item in basket.Items)
            {
                var product = await _catalogService.GetCatalog(item.ProductId);

                // set additional product fields onto basket item
                item.ProductName = product.Name;
                item.Category = product.Category;
                item.Summary = product.Summary;
                item.Description = product.Description;
                item.ImageFile = product.ImageFile;
            }

            var orders = await _orderService.GetOrdersByUserName(userName);

            ShoppingModel shoppingModel = new ShoppingModel()
            {
                BasketWithProducts = basket,
                Orders = orders,
                UserName = userName
            };

            return Ok(shoppingModel);

        }

    }
}
