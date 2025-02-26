using Ecommercebegin.Products.WebaAPI.Context;
using Ecommercebegin.Products.WebaAPI.DTOs;
using Ecommercebegin.Products.WebaAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommercebegin.Products.WebaAPI.Controllers
{
    [ApiController]
    [Route("api/")]
    public class ProductController : Controller
    {
        private readonly Bogus.Randomizer _randomizer;

        private readonly MyDbContext _context;

        public ProductController(MyDbContext context, Bogus.Randomizer randomizer)
        {
            _context = context;
            _randomizer = randomizer;
        }

        [HttpGet]
        [Route("products")]
        public IActionResult GetAllProducts()
        {
            List<Product> data = _context.Products.ToList();

            var result = data.Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            });

            return Ok(result);
        }

        [HttpGet]
        [Route("products/{id:int}")]
        public IActionResult GetProductById(int id)
        {
            Product data = _context.Products.FirstOrDefault(x => x.Id == id);

            if (data == null)
            {
                return NotFound(new
                {
                    status = "not found",
                    data = new { }
                });
            }


            var result = new
            {
                Id = data.Id,
                Name = data.Name,
                Description = data.Description
            };

            return Ok(new
            {
                status = "success",
                data = result
            });
        }

        [HttpPost]
        [Route("products")]
        public IActionResult CreateProduct([FromBody] CreateProductDto product)
        {

            if(product == null)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Invalid payload"
                });
            }

            Product equalData = _context.Products.FirstOrDefault(x => x.Name == product.Name);

            if (equalData != null)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Product already exists"
                });
            }

            Product newProduct = new Product
            {
                Name = product.Name,
                Description = product.Description
            };

            if(newProduct.Name == null || newProduct.Description == null)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Name and Description are required"
                });
            }

            _context.Products.Add(newProduct);
            
            _context.SaveChanges();

            return Ok(new
            {
                status = "success",
                data = newProduct
            });
        }

        [HttpDelete]
        [Route("products/{id:int}")]
        public IActionResult DeleteProduct(int id)
        {
            Product data = _context.Products.FirstOrDefault(x => x.Id == id);

            if (data == null)
            {
                return NotFound(new
                {
                    status = "not found",
                    data = new { }
                });
            }

            _context.Products.Remove(data);
            _context.SaveChanges();

            return Ok(new
            {
                status = "success, product deleted",
            });
        }

        [HttpPut]
        [Route("products/{id:int}")]
        public IActionResult UpdateProduct(int id, [FromBody] CreateProductDto product)
        {
            Product data = _context.Products.FirstOrDefault(x => x.Id == id);

            if (data == null)
            {
                return NotFound(new
                {
                    status = "not found",
                    data = new { }
                });
            }

            if (product == null)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "Invalid payload"
                });
            }

            data.Name = product.Name;
            data.Description = product.Description;

            _context.SaveChanges();

            return Ok(new
            {
                status = "changed datas",
                data = data
            });
        }

        [HttpGet]
        [Route("products/faker-maker")]
        public IActionResult Faker()
        {
            for (int i = 0; i < 50; i++)
            {
                Product product = new Product
                {
                    Name = _randomizer.Word(),
                    Description = _randomizer.Word()
                };

                _context.Products.Add(product);
            }

            _context.SaveChanges();

            return Ok(new
            {
                status = "success",
                message = "50 Products Added to the end of the page",
                datas = _context.Products.ToList()
            });
        }
    }
}
