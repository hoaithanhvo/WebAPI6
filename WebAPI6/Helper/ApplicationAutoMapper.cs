using AutoMapper;
using WebAPI6.Data;
using WebAPI6.Models;

namespace WebAPI6.Helper
    {
    public class ApplicationAutoMapper:Profile
        {
        public ApplicationAutoMapper()
            {
            CreateMap<TIotMoldMaster, TIotMoldMasterModel>().ReverseMap();
            CreateMap<Book, BookModel>().ReverseMap();
            CreateMap<BookModel, Book>().ForMember(dest => dest.Id, opt => opt.Ignore());
            }
        }
    }
