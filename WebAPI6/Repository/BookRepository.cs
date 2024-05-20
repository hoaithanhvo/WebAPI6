using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAPI6.Data;
using WebAPI6.Models;

namespace WebAPI6.Repository
    {
    public class BookRepository : IBookRepository
        {
        private readonly NIDEC_IOTContext _context;
        private readonly IMapper _mapper;

        public BookRepository(NIDEC_IOTContext context, IMapper mapper)
            {

            _context = context;
            _mapper = mapper;
            }
        public async Task<int> CreateBookAsync(BookModel model)
            {
            var newBook = _mapper.Map<Book>(model);
            await _context.AddAsync(newBook);
            await _context.SaveChangesAsync();
            return newBook.Id;

            }
        public async Task DeleteBookAsync(int id)
            {
            var checkBook = _context.Books.SingleOrDefault(book => book.Id == id);
            if (checkBook != null)
                {
                _context.Books.Remove(checkBook);
                await _context.SaveChangesAsync();
                }
            }

        public async Task<List<BookModel>> GetAllBookAsync()
            {
            var books = await _context.Books.ToListAsync();
            return _mapper.Map<List<BookModel>>(books);
            }

        public async Task<BookModel> GetBookAsync(int id)
            {
            var book = await _context.Books.FindAsync(id);
            return _mapper.Map<BookModel>(book);
            }

        public async Task<BookModel> UpdateBookAsync(int id, BookModel model)
            {
            //var check = _context.Books.SingleOrDefault(li => li.Id == id);
            //if(check!=null)
            //    {
            //    check.Description = model.Description;
            //    check.Quantity = model.Quantity;
            //    check.Price = model.Price;
            //    check.Title = model.Title;
            //    }
            //await _context.SaveChangesAsync();
            //return model;

            var existingBook = await _context.Books.SingleOrDefaultAsync(b => b.Id == id);

            if (existingBook != null)
                {
                // Ánh xạ các thuộc tính từ model sang existingBook (trừ Id)
                _mapper.Map(model, existingBook);

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();
                }

            return model;
            }
        }
    }
