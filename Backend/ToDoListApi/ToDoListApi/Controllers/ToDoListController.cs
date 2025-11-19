using Microsoft.AspNetCore.Mvc;
using ToDoListApi.Domain.Interfaces;
using ToDoListApi.Domain.Models;

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
            var todos = _service.GetAll();
            return Ok(todos);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ToDoItem todo)
        {
            try
            {
                var created = _service.Add(todo.Title);
                return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] ToDoItem todo)
        {
            todo.Id = id;
            var updated = _service.Update(todo);
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
