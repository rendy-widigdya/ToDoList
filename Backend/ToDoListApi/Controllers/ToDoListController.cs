using Microsoft.AspNetCore.Mvc;
using ToDoListApi.Domain.Interfaces;
using ToDoListApi.Models;
using ToDoListApi.Mappers;

namespace ToDoListApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToDoListController : ControllerBase
    {
        private readonly IToDoListService _service;
        private readonly ILogger<ToDoListController> _logger;

        public ToDoListController(IToDoListService service, ILogger<ToDoListController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Get all todo items
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ToDoItemResponse>), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            _logger.LogInformation("Getting all todo items");
            var todos = _service.GetAll()
                .Select(ToDoItemMapper.ToResponse);
            return Ok(todos);
        }

        /// <summary>
        /// Get a todo item by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ToDoItemResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            _logger.LogInformation("Getting todo item with ID: {Id}", id);
            var todo = _service.GetById(id);
            if (todo == null)
            {
                return NotFound();
            }
            return Ok(ToDoItemMapper.ToResponse(todo));
        }

        /// <summary>
        /// Create a new todo item
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ToDoItemResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] ToDoItemRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Creating todo item with title: {Title}", request.Title);
                var created = _service.Add(request.Title);
                var response = ToDoItemMapper.ToResponse(created);
                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Failed to create todo item");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing todo item
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Update(Guid id, [FromBody] ToDoItemRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating todo item with ID: {Id}", id);
            
            // Get existing item to preserve CreatedAt and other properties
            var existing = _service.GetById(id);
            if (existing == null)
            {
                return NotFound();
            }

            // Update only the properties from the request
            existing.Title = request.Title;
            existing.IsDone = request.IsDone;

            var updated = _service.Update(existing);
            return updated ? NoContent() : NotFound();
        }

        /// <summary>
        /// Delete a todo item
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            _logger.LogInformation("Deleting todo item with ID: {Id}", id);
            var deleted = _service.Delete(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
