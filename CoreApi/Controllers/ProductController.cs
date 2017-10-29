using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using CoreApi.Services;
using CoreApi.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;
using CoreApi.Repositories;
using AutoMapper;

namespace CoreApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {

        private readonly ILogger<ProductController> _logger;

        private readonly IMailService _mailService;

        private readonly IProductRepository _productRepository;

        public ProductController(
            ILogger<ProductController> logger,
            IMailService mailService,
            IProductRepository productRepository
            )
        {
            _logger = logger;
            _mailService = mailService;
           // mailService.Send("Product Deleted", "邮件服务启动完毕");
            _productRepository = productRepository;
        }



        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _productRepository.GetProducts();
            var results = Mapper.Map<IEnumerable<ProductWithoutMaterialDto>>(products);
           
            return Ok(results);
        }

        [Route("{id}", Name = "GetProduct")]
      
        public IActionResult GetProduct(int id, bool includeMaterial = false) {

            var product = _productRepository.GetProduct(id, includeMaterial);
            if (product == null)
            {
                return NotFound();
            }
            if (includeMaterial)
            {
                var productWithMaterialResult = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description
                };
                foreach (var material in product.Materials)
                {
                    productWithMaterialResult.Materials.Add(new MaterialDto
                    {
                        Id = material.Id,
                        Name = material.Name
                    });
                }
                return Ok(productWithMaterialResult);
            }

            var onlyProductResult = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description
            };
            return Ok(onlyProductResult);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProductCreation product)
        {
            if (product == null) {
                return BadRequest();
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var maxId = ProductService.Current.Products.Max(x => x.Id);
            var newProduct = new ProductDto
            {
                Id = ++maxId,
                Name = product.Name,
                Price = product.Price
            };
            ProductService.Current.Products.Add(newProduct);

            return CreatedAtRoute("GetProduct", new { id = newProduct.Id }, newProduct);
        }

        [HttpPut]
        public IActionResult Put(int id, [FromBody] ProductModification product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            if (product.Name == "产品")
            {
                ModelState.AddModelError("Name", "产品的名称不可以是'产品'二字");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = ProductService.Current.Products.SingleOrDefault(x => x.Id == id);
            if (model == null)
            {
                return NotFound();
            }
            model.Name = product.Name;
            model.Price = product.Price;
            model.Description = product.Description;

            // return Ok(model);
            return NoContent();

        }
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] JsonPatchDocument<ProductModification> patchDoc)
        {
            if (patchDoc == null) {

                return BadRequest();
            }
            var model = ProductService.Current.Products.SingleOrDefault(x => x.Id == id);

            if(model == null){
                return NotFound();
            }

            var toPatch = new ProductModification
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price
            };

            patchDoc.ApplyTo(toPatch, ModelState);

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }



            if (toPatch.Name == "产品")
            {
                ModelState.AddModelError("Name", "产品的名称不可以是'产品'二字");
            }
            TryValidateModel(toPatch);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }



            model.Name = toPatch.Name;
            model.Description = toPatch.Description;
            model.Price = toPatch.Price;



            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var model = ProductService.Current.Products.SingleOrDefault(x => x.Id == id);
            if (model == null)
            {
                return NotFound();
            }

            _mailService.Send("Product Deleted", $"Id为{id}的产品被删除了");


            ProductService.Current.Products.Remove(model);
            return NoContent();
        }


    }
}
