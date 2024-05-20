using Microsoft.Build.Framework;
using WebAPI6.Models;

namespace WebAPI6.Repository
    {
    public interface IBookRepository
        {
        public Task<List<BookModel>> GetAllBookAsync();
        public Task<BookModel> GetBookAsync(int id);
        public Task<BookModel> UpdateBookAsync(int id, BookModel model);
        public Task DeleteBookAsync(int id);
        public Task <int> CreateBookAsync(BookModel model);
        }
    }
