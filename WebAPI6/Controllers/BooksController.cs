using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI6.Data;
using WebAPI6.Models;

namespace WebAPI6.Controllers
    {
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
        {
        private readonly NIDEC_IOTContext _context;
        private readonly IMapper _mapper;

        public BooksController(NIDEC_IOTContext context, IMapper mapper)
            {
            _context = context;
            _mapper = mapper;
            }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookModel>>> GetBooks()
            {
            if (_context.Books == null)
                {
                return NotFound();
                }
            var convert = await _context.Books.ToListAsync();
            return Ok(_mapper.Map<List<BookModel>>(convert));
            }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
            {
            if (_context.Books == null)
                {
                return NotFound();
                }
            var book = await _context.Books.FindAsync(id);

            if (book == null)
                {
                return NotFound();
                }

            return book;
            }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, BookModel book)
            {
            var check = await _context.Books.SingleOrDefaultAsync(x => x.Id == id);
            var item = _mapper.Map<Book>(book);
            if (check == null)
                {
                return BadRequest();
                }

            _context.Books.Update(check);
            try
                {
                await _context.SaveChangesAsync();
                }
            catch (DbUpdateConcurrencyException)
                {
                if (!BookExists(id))
                    {
                    return NotFound();
                    }
                else
                    {
                    throw;
                    }
                }

            return Ok(book);
            }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostBook(BookModel book)
            {
            var nbook = _mapper.Map<Book>(book);
            if (_context.Books == null)
                {
                return Problem("Entity set 'NIDEC_IOTContext.Books'  is null.");
                }
            _context.Books.Add(nbook);
            try
                {
                await _context.SaveChangesAsync();
                }
            catch (DbUpdateException)
                {
                }

            return Ok(nbook); // hoặc return Ok();
            }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
            {
            if (_context.Books == null)
                {
                return NotFound();
                }
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                {
                return NotFound();
                }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
            }

        private bool BookExists(int id)
            {
            return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
            }
        }
    }
