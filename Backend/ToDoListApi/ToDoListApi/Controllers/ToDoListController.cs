using Microsoft.AspNetCore.Mvc;
using ToDoListApi.Domain.Interfaces;
using ToDoListApi.Models;
using ToDoListApi.Domain.Mappers;

namespace ToDoListApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToDoListController : ControllerBase
    {
        private readonly IToDoListService _service;

        public ToDoListController(IToDoListService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var todos = _service.GetAll()
                .Select(ToDoItemMapper.ToResponse);
            return Ok(todos);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ToDoItemRequest request)
        {
            try
            {
                var created = _service.Add(request.Title);
                var response = ToDoItemMapper.ToResponse(created);
                return CreatedAtAction(nameof(GetAll), new { id = response.Id }, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] ToDoItemRequest request)
        {
            var domainItem = ToDoItemMapper.ToDomain(request);
            domainItem.Id = id;
            var updated = _service.Update(domainItem);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var deleted = _service.Delete(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
