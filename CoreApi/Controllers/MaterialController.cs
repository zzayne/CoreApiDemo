using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreApi.Services;
using CoreApi.Repositories;
using CoreApi.Dtos;

namespace CoreApi.Controllers
{
    [Route("api/product")]
    public class MaterialController:Controller
    {

        private readonly IProductRepository _productRepository;
        public MaterialController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }



        [HttpGet("{productId}/materials")]
        public IActionResult GetMaterials(int productId)
        {
            var product = _productRepository.ProductExist(productId);
            if (!product)
            {
                return NotFound();
            }

            var materials = _productRepository.GetMaterialsForProduct(productId);
            var results = materials.Select(material => new MaterialDto
            {
                Id = material.Id,
                Name = material.Name
            }).ToList();
            return Ok(results);
        }

        [HttpGet("{productId}/materials/{id}")]
        public IActionResult GetMaterial(int productId, int id)
        {
            var material = _productRepository.GetMaterialForProduct(productId, id);
            if (material == null)
            {
                return NotFound();
            }
            var result = new MaterialDto
            {
                Id = material.Id,
                Name = material.Name
            };
            return Ok(result);
        }
         
    }
}
   