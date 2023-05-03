using AutoMapper;
using Business.Interfaces;
using Business.Models;
using Business.Validation;
using Data.Entities;
using Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
	public class ProductService : IProductService
	{
		public ProductService(IUnitOfWork @object, IMapper mapper)
		{
			Object = @object;
			Mapper = mapper;
		}

		public IUnitOfWork Object { get; }
		public IMapper Mapper { get; }

		public async Task AddAsync(ProductModel model)
		{
			CheckPropertyOfProductModel(model);

			Product product = Mapper.Map<Product>(model);

			await Object.ProductRepository.AddAsync(product);

			await Object.SaveAsync();
		}

		public async Task AddCategoryAsync(ProductCategoryModel categoryModel)
		{
			CheckPropertyOfProductCategoryModel(categoryModel);

			ProductCategory category = Mapper.Map<ProductCategory>(categoryModel);

			await Object.ProductCategoryRepository.AddAsync(category);

			await Object.SaveAsync();
		}

		public async Task DeleteAsync(int modelId)
		{
			await Object.ProductRepository.DeleteByIdAsync(modelId);

			await Object.SaveAsync();
		}

		public async Task<IEnumerable<ProductModel>> GetAllAsync()
		{
			IEnumerable<Product> products = await Object.ProductRepository.GetAllWithDetailsAsync();

			var productsModel = Mapper.Map<IEnumerable<ProductModel>>(products);

			return productsModel;
		}

		public async Task<IEnumerable<ProductCategoryModel>> GetAllProductCategoriesAsync()
		{
			var categories = await Object.ProductCategoryRepository.GetAllAsync();

			var categoriesModel = Mapper.Map<IEnumerable<ProductCategoryModel>>(categories);

			return categoriesModel;
		}

		public async Task<IEnumerable<ProductModel>> GetByFilterAsync(FilterSearchModel filterSearch)
		{
			CheckPropertyOfFilterSearchModel(filterSearch);

			var products = await Object.ProductRepository.GetAllWithDetailsAsync();

			var productsModel = Mapper.Map<IEnumerable<ProductModel>>(products
					.Where(p => (filterSearch.CategoryId is null || p.ProductCategoryId == filterSearch.CategoryId) && 
					p.Price >= filterSearch.MinPrice &&
					p.Price <= filterSearch.MaxPrice));

			return productsModel;
		}

		public async Task<ProductModel> GetByIdAsync(int id)
		{
			var product = await Object.ProductRepository.GetByIdWithDetailsAsync(id);

			var productModel = Mapper.Map<ProductModel>(product);

			return productModel;
		}

		public async Task RemoveCategoryAsync(int categoryId)
		{
			await Object.ProductCategoryRepository.DeleteByIdAsync(categoryId);

			await Object.SaveAsync();
		}

		public async Task UpdateAsync(ProductModel model)
		{
			CheckPropertyOfProductModel(model);

			Product product = Mapper.Map<Product>(model);

			Object.ProductRepository.Update(product);

			await Object.SaveAsync();
		}

		public async Task UpdateCategoryAsync(ProductCategoryModel categoryModel)
		{
			CheckPropertyOfProductCategoryModel(categoryModel);

			ProductCategory category = Mapper.Map<ProductCategory>(categoryModel);

			Object.ProductCategoryRepository.Update(category);

			await Object.SaveAsync();
		}

		private void CheckPropertyOfProductModel(ProductModel model)
		{
			if (model == null) throw new MarketException("ProductModel cannot be null");

			if (string.IsNullOrEmpty(model.ProductName)) throw new MarketException("ProductName cannot be null or empty");

			if (model.Price <= 0) throw new MarketException();
		}

		private void CheckPropertyOfProductCategoryModel(ProductCategoryModel model)
		{
			if (model == null) throw new MarketException("ProductModel cannot be null");

			if (string.IsNullOrEmpty(model.CategoryName)) throw new MarketException("CategoryName cannot be null or empty");
		}

		private void CheckPropertyOfFilterSearchModel(FilterSearchModel model)
		{
			if (model == null) throw new MarketException("ProductModel cannot be null");

			if (model.MinPrice <= 0) throw new MarketException("MinPrice cannot be less than 0");

			if (model.MaxPrice <= 0) throw new MarketException("MaxPrice cannot be less than 0");

			if (model.MaxPrice < model.MinPrice) throw new MarketException("MaxPrice cannot be less than MinPrice");

			if (model.MinPrice is null) model.MinPrice = 0;

			if (model.MaxPrice is null) model.MaxPrice = int.MaxValue;
		}
	}
}
