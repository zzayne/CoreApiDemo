using CoreApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Repositories
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProducts();

        bool ProductExist(int productId);
        Product GetProduct(int productId, bool includeMaterials);
        IEnumerable<Material> GetMaterialsForProduct(int productId);

        Material GetMaterialForProduct(int productId, int materialId);
    }
}
