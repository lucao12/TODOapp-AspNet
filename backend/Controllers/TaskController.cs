using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using Teste.Data;
using Teste.Models;
using Teste.ViewModel;

namespace Teste.Controllers
{
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost]
        [Route(template: "add/task")]
        public async Task<IActionResult> PostTaskAsync(
            [FromBody] TaskCreateViewModel model)
        {
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        error = "Usuário não encontrado"
                    });
                }

                var userid = user.Id;

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var taskExist = await _context.Tasks.FirstOrDefaultAsync(x =>
                x.Title.ToLower() == model.Title.ToLower() &&
                x.UserId == userid);

                if (taskExist != null)
                {
                    return BadRequest(new
                    {
                        error = "Tarefa já existente!"
                    });
                }

                var newTask = new TaskItem
                {
                    Title = model.Title,
                    Description = model.Description,
                    UserId = userid,
                    isCompleted = model.isCompleted,
                    DateLimit = model.DateLimit,
                    Priority = model.Priority
                };

                await _context.Tasks.AddAsync(newTask);
                await _context.SaveChangesAsync();

                return Created($"{newTask.Id}", newTask);
            }
            catch
            {
                return StatusCode(500, new
                {
                    error = "Erro interno do servidor!"
                });
            }
        }
        [Authorize]
        [HttpGet]
        [Route(template: "tasks")]
        public async Task<IActionResult> GetAllTasksAsync()
        {
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        error = "Usuário não encontrado"
                    });
                }

                var userid = user.Id;

                var tasks = await _context.Tasks.Where(x =>
                x.UserId == userid).OrderBy(x => x.isCompleted).ThenBy(x => x.DateLimit).ThenBy(x => x.Title).ToListAsync();

                return Ok(tasks);
            }
            catch
            {
                return StatusCode(500, new
                {
                    error = "Erro interno do servidor!"
                });
            }
        }
        [Authorize]
        [HttpGet]
        [Route(template: "task/{id:int}")]
        public async Task<IActionResult> GetTaskByIdAsync(
            [FromRoute] int id)
        {
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        error = "Usuário não encontrado"
                    });
                }

                var userid = user.Id;

                var task = await _context.Tasks.FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.UserId == userid);

                if (task == null)
                {
                    return BadRequest(new
                    {
                        error = "Tarefa não encontrada"
                    });
                }

                return Ok(task);
            }
            catch
            {
                return StatusCode(500, new
                {
                    error = "Erro interno do servidor!"
                });
            }
        }
        [Authorize]
        [HttpDelete]
        [Route(template: "delete/task/{id:int}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id)
        {
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        error = "Usuário não encontrado"
                    });
                }

                var userid = user.Id;

                var task = await _context.Tasks.FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.UserId == userid);

                if (task == null)
                {
                    return BadRequest(new
                    {
                        error = "Tarefa não encontrada"
                    });
                }

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                return Ok("Tarefa deletada com sucesso");
            }
            catch
            {
                return StatusCode(500, new
                {
                    error = "Erro interno do servidor!"
                });
            }
        }
        [Authorize]
        [HttpPut]
        [Route(template: "update/task/{id:int}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] int id,
            [FromBody] TaskUpdateViewModel model)
        {
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userEmail);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        error = "Usuário não encontrado"
                    });
                }

                var userid = user.Id;

                var task = await _context.Tasks.FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.UserId == userid);

                if (task == null)
                {
                    return BadRequest(new
                    {
                        error = "Tarefa não encontrada"
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (model.Title != null)
                {
                    var verificaTitle = await _context.Tasks.FirstOrDefaultAsync(x =>
                    x.Title.ToLower() == model.Title.ToLower() &&
                    x.UserId == userid);

                    if (verificaTitle != null && verificaTitle != task)
                    {
                        return BadRequest(new
                        {
                            error = "Tarefa já cadastrada!"
                        });
                    }
                }

                task.Title = (model.Title != null && model.Title != task.Title) ? model.Title : task.Title;
                task.Description = (model.Description != null && model.Description != task.Description) ? model.Description : task.Description;
                task.Priority = (model.Priority != null && model.Priority != task.Priority) ? model.Priority : task.Priority;
                task.isCompleted = (model.isCompleted != null && model.isCompleted != task.isCompleted) ? (bool)model.isCompleted : task.isCompleted;
                task.DateLimit = (model.DateLimit != null && model.DateLimit != task.DateLimit) ? (DateTime)model.DateLimit : task.DateLimit;

                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();

                return Ok("Tarefa atualizada com sucesso");
            }
            catch
            {
                return StatusCode(500, new
                {
                    error = "Erro interno do servidor!"
                });
            }
        }
    }
}
