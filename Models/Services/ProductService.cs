using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StoreMongoApp.Models;

namespace StoreMongoApp.Services
{
    public class ProductService
    {
        private readonly IMongoCollection<Product> _productsCollection;

        public ProductService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);

            _productsCollection = database.GetCollection<Product>(
                mongoDBSettings.Value.ProductsCollectionName
            );
        }

        public async Task<List<Product>> GetAsync()
        {
            return await _productsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Product?> GetAsync(string id)
        {
            return await _productsCollection.Find(product => product.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Product product)
        {
            await _productsCollection.InsertOneAsync(product);
        }

        public async Task UpdateAsync(string id, Product updatedProduct)
        {
            await _productsCollection.ReplaceOneAsync(product => product.Id == id, updatedProduct);
        }

        public async Task RemoveAsync(string id)
        {
            await _productsCollection.DeleteOneAsync(product => product.Id == id);
        }
    }
}