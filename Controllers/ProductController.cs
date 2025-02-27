using Ecommercebegin.Products.WebaAPI.Context;
using Ecommercebegin.Products.WebaAPI.DTOs;
using Ecommercebegin.Products.WebaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommercebegin.Products.WebaAPI.Controllers
{
    [ApiController]
    [Route("api/products/")]
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
        public IActionResult GetAllProducts()
        {
            List<Product> data = _context.Products.ToList();

            if (data.Count == 0)
            {
                return NotFound(new
                {
                    status = "not found",
                    data = new { }
                });
            }

            var result = data.Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            });

            return Ok(result);
        }

        [HttpGet]
        [Route("{id:int}")]
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
        [Route("{id:int}")]
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
        [Route("{id:int}")]
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
        [Route("faker-maker")]
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

        [HttpGet]
        [Route("delete-all-data")]
        public IActionResult DeleteAllData()
        {
            if (_context.Products.Count() == 0)
            {
                return NotFound(new
                {
                    status = "not found",
                    message = "No data found"
                });
            }

            _context.Products.RemoveRange(_context.Products);

            _context.SaveChanges();

            return Ok(new
            {
                status = "error",
                message = "All data deleted"
            });
        }


    }
}
