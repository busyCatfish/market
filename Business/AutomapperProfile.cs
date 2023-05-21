using AutoMapper;
using Business.Models;
using Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Business
{
	public class AutomapperProfile : Profile
	{
		public AutomapperProfile()
		{
			CreateMap<Receipt, ReceiptModel>()
				.ForMember(rm => rm.ReceiptDetailsIds, r => r.MapFrom(x => x.ReceiptDetails.Select(rd => rd.Id)))
				.ReverseMap();

			CreateMap<Product, ProductModel>()
				.ForMember(pm => pm.ReceiptDetailIds, p => p.MapFrom(x => x.ReceiptDetails.Select(rd => rd.Id)))
				.ForMember(pm => pm.ProductCategoryId, p => p.MapFrom(x => x.ProductCategoryId))
				.AfterMap((p, pm) => pm.CategoryName = p.Category != null ? p.Category.CategoryName : string.Empty)
			.ReverseMap();

			CreateMap<ReceiptDetail, ReceiptDetailModel>()
				.ReverseMap();

			CreateMap<Customer, CustomerModel>()
				.ForMember(rm => rm.Id, r => r.MapFrom(x => x.Id))
				.ForMember(rm => rm.Id, r => r.MapFrom(x => x.Person.Id))
				.ForMember(rm => rm.Name, r => r.MapFrom(x => x.Person.Name))
				.ForMember(rm => rm.Surname, r => r.MapFrom(x => x.Person.Surname))
				.ForMember(rm => rm.BirthDate, r => r.MapFrom(x => x.Person.BirthDate))
				.ForMember(rm => rm.ReceiptsIds, r => r.MapFrom(x => x.Receipts.Select(rd => rd.Id)))
				.ReverseMap();

			CreateMap<ProductCategory, ProductCategoryModel>()
				.ForMember(rm => rm.ProductIds, r => r.MapFrom(x => x.Products.Select(rd => rd.Id)))
				.ReverseMap();
		}
	}
}