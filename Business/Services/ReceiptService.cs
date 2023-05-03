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
	public class ReceiptService : IReceiptService
	{
		public ReceiptService(IUnitOfWork @object, IMapper mapper)
		{
			Object = @object;
			Mapper = mapper;
		}

		public IUnitOfWork Object { get; }
		public IMapper Mapper { get; }

		public async Task AddAsync(ReceiptModel model)
		{
			CheckPropertyOfReceiptModel(model);

			Receipt receipt = Mapper.Map<Receipt>(model);

			await Object.ReceiptRepository.AddAsync(receipt);

			await Object.SaveAsync();
		}

		public async Task AddProductAsync(int productId, int receiptId, int quantity)
		{
			Receipt receipt = await Object.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);

			if (receipt is null)
			{
				throw new MarketException();
			}

			var receiptDetailWithExceptProduct = receipt.ReceiptDetails?.FirstOrDefault(rd => rd.ProductId == productId);

			if (receiptDetailWithExceptProduct != null)
			{
				receiptDetailWithExceptProduct.Quantity += quantity;

				await Object.SaveAsync();

				return;
			}

			Product product = await Object.ProductRepository.GetByIdAsync(productId);

			if (product is null)
			{
				throw new MarketException();
			}

			ReceiptDetail receiptDetail = new ReceiptDetail()
			{
				ProductId = productId,
				Product = product,
				ReceiptId = receiptId,
				Quantity = quantity,
				UnitPrice = product.Price,
				Receipt = receipt,
				DiscountUnitPrice = product.Price - (receipt.Customer.DiscountValue * product.Price / 100m)
			};

			if (receipt.ReceiptDetails is null)
			{
				receipt.ReceiptDetails = new List<ReceiptDetail>() { receiptDetail };
			}
			else
			{
				receipt.ReceiptDetails.Add(receiptDetail);
			}

			await Object.ReceiptDetailRepository.AddAsync(receiptDetail);

			await Object.SaveAsync();
		}

		public async Task CheckOutAsync(int receiptId)
		{
			Receipt receipt = await Object.ReceiptRepository.GetByIdAsync(receiptId);

			if (receipt is null)
			{
				throw new MarketException();
			}

			receipt.IsCheckedOut = true;

			await Object.SaveAsync();
		}

		public async Task DeleteAsync(int modelId)
		{
			Receipt receipt = await Object.ReceiptRepository.GetByIdWithDetailsAsync(modelId);

			if (receipt is null)
			{
				throw new MarketException();
			}

			foreach (var rd in receipt.ReceiptDetails.ToArray())
			{
				Object.ReceiptDetailRepository.Delete(rd);
			}

			await Object.ReceiptRepository.DeleteByIdAsync(modelId);

			await Object.SaveAsync();
		}

		public async Task<IEnumerable<ReceiptModel>> GetAllAsync()
		{
			IEnumerable<Receipt> receipts = await Object.ReceiptRepository.GetAllWithDetailsAsync();

			var receiptsModel = Mapper.Map<IEnumerable<ReceiptModel>>(receipts);

			return receiptsModel;
		}

		public async Task<ReceiptModel> GetByIdAsync(int id)
		{
			var receipt = await Object.ReceiptRepository.GetByIdWithDetailsAsync(id);

			var receiptModel = Mapper.Map<ReceiptModel>(receipt);

			return receiptModel;
		}

		public async Task<IEnumerable<ReceiptDetailModel>> GetReceiptDetailsAsync(int receiptId)
		{
			var receipt = await Object.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);

			if (receipt is null)
			{
				throw new MarketException();
			}

			var receiptDetailsModel = Mapper.Map<IEnumerable<ReceiptDetailModel>>(receipt.ReceiptDetails);

			return receiptDetailsModel;
		}

		public async Task<IEnumerable<ReceiptModel>> GetReceiptsByPeriodAsync(DateTime startDate, DateTime endDate)
		{
			IEnumerable<Receipt> receipts = await Object.ReceiptRepository.GetAllWithDetailsAsync();

			var receiptsModel = Mapper.Map<IEnumerable<ReceiptModel>>(receipts.Where(r => r.OperationDate >= startDate && r.OperationDate <= endDate));

			return receiptsModel;
		}

		public async Task RemoveProductAsync(int productId, int receiptId, int quantity)
		{
			Receipt receipt = await Object.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);

			if (receipt is null)
			{
				throw new MarketException("Receipt is null");
			}

			var receiptDetail = receipt.ReceiptDetails.FirstOrDefault(rd => rd.ProductId == productId);

			if (receiptDetail is null)
			{
				throw new MarketException();
			}

			receiptDetail.Quantity -= quantity;

			if (receiptDetail.Quantity <= 0)
			{
				Object.ReceiptDetailRepository.Delete(receiptDetail);
			}

			await Object.SaveAsync();
		}

		public async Task<decimal> ToPayAsync(int receiptId)
		{
			Receipt receipt = await Object.ReceiptRepository.GetByIdWithDetailsAsync(receiptId);

			if (receipt is null)
			{
				throw new MarketException();
			}

			return receipt.ReceiptDetails.Sum(rd => rd.DiscountUnitPrice * rd.Quantity);
		}

		public async Task UpdateAsync(ReceiptModel model)
		{
			CheckPropertyOfReceiptModel(model);

			Receipt receipt = Mapper.Map<Receipt>(model);

			Object.ReceiptRepository.Update(receipt);

			await Object.SaveAsync();
		}

		private void CheckPropertyOfReceiptModel(ReceiptModel model)
		{
			if (model == null) throw new MarketException("ReceiptModel cannot be null");
		}
	}
}
