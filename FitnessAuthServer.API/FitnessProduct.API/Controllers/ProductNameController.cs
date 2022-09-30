using FitnessAuthServer.Data;
using FitnessAuthSever.Core.Dtos;
using FitnessAuthSever.Core.Entity;
using FitnessAuthSever.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FitnessProduct.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductNameController : ControllerBase
    {
        //Ürün İsimlerini tutan bir servis.

        private readonly IGenericService<Product, ProductDto> _productService;

        public ProductNameController(IGenericService<Product, ProductDto> productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductName()
        {
            var userName = HttpContext.User.Identity.Name;
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            var products = await _productService.GetAllAsync();


            //Veritabanında userId ve username e göre veri çek.

        }

    }
}
