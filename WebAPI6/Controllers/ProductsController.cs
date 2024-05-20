using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI6.Models;
using WebAPI6.Repository;

namespace WebAPI6.Controllers
    {
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
        {
        private readonly IBookRepository _repo;

        public ProductsController(IBookRepository repo)
            {
            _repo = repo;
            }
        [HttpGet]
        public async Task<IActionResult> GetAll()
            {
            try
                {
                return Ok(await _repo.GetAllBookAsync());
                }
            catch (Exception ex)
                {
                return BadRequest(ex.Message);
                }
            }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBookAsync(int id)
            {
            //var book = await _repo.GetBookAsync(id);
            return Ok(await _repo.GetBookAsync(id));
            //return book == null ? NotFound() : Ok(book);
            }

    [HttpDelete("{id}")]
        public  async Task<IActionResult> DeleteBook(int id)
            {
            try
                {
                await _repo.DeleteBookAsync(id);
                return Ok();
                }
            catch(Exception ex)
                {
                return BadRequest(ex.Message);
                }
            }
        [HttpPost]
        public async Task<IActionResult> CreateBook(BookModel model)
            {
            return Ok(await _repo.CreateBookAsync(model));

            }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(BookModel model, int id)
            {
            return Ok(await _repo.UpdateBookAsync(id, model));
            }
        }
    }
