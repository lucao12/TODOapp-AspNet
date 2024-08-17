using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Teste.Data;
using Teste.Interfaces;
using Teste.Models;
using Teste.Services;
using Teste.ViewModel;

namespace Teste.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHashServices _hashServices;
        private readonly ITokenServices _tokenServices;
        public UserController(AppDbContext context, IHashServices hashServices, ITokenServices tokenServices)
        {
            _context = context;
            _hashServices = hashServices;
            _tokenServices = tokenServices;
        }

        [HttpPost]
        [Route(template: "login")]
        public async Task<IActionResult> LoginAsync(
            [FromBody] UserLoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userExist = await _context.Users.FirstOrDefaultAsync(x =>
                x.Email == model.Email.ToLower());

                if (userExist == null)
                {
                    return BadRequest(new
                    {
                        error = "Email e/ou senha incorretos!"
                    });
                }

                var user = await _context.Users.FirstOrDefaultAsync(x =>
                x.Email == model.Email.ToLower() &&
                x.Password == _hashServices.GenerateHash(model.Password, Convert.FromBase64String(userExist.Salt)));

                if (user == null)
                {
                    return BadRequest(new
                    {
                        error = "Email e/ou senha incorretos!"
                    });
                }

                var token = _tokenServices.GenerateToken(user);
                return Ok(new
                {
                    user = user,
                    token = token
                });
            }
            catch
            {
                return StatusCode(500, new
                {
                    error = "Erro interno do servidor!"
                });
            }
        }
        [HttpPost]
        [Route(template: "signup")]
        public async Task<IActionResult> SignUpAsync(
            [FromBody] UserSignUpViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == model.Email.ToLower());

                if (user != null)
                {
                    return BadRequest(new { error = "Usuário já cadastrado!" });
                }

                var salt = _hashServices.GenerateSalt();

                var hashedPassword = _hashServices.GenerateHash(model.Password, salt);

                var newUser = new User
                {
                    Name = model.Name,
                    Email = model.Email.ToLower(),
                    Password = hashedPassword,
                    Role = "User",
                    Salt = Convert.ToBase64String(salt)  
                };

                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();
                return Created($"{newUser.Id}", newUser);
            }
            catch
            {
                return StatusCode(500, new { error = "Erro interno do servidor!" });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route(template: "users")]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var users = await _context.Users.OrderBy(x => x.Role).ThenBy(x => x.Name).ToListAsync();

                return Ok(users);
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
        [Route(template: "user/yourself")]
        public async Task<IActionResult> GetYourselfAsync()
        {
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var userFind = await _context.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
                var id = userFind.Id;
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        error = "Usuário não encontrado!"
                    });
                }

                return Ok(user);
            }
            catch
            {
                return StatusCode(500, new
                {
                    error = "Erro interno do servidor!"
                });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route(template: "user/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] int id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        error = "Usuário não encontrado!"
                    });
                }

                return Ok(user);
            }
            catch
            {
                return StatusCode(500, new
                {
                    error = "Erro interno do servidor!"
                });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route(template: "delete/user/{id:int}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        error = "Usuário não encontrado!"
                    });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok("Usuário deletado com sucesso");
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
        [Route(template: "update/user/yourself")]
        public async Task<IActionResult> UpdateAsync(
            [FromBody] UserUpdateViewModel model)
        {
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var userFind = await _context.Users.FirstOrDefaultAsync(x => x.Email == userEmail);
                var id = userFind.Id;
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        error = "Usuário não encontrado!"
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (model.Email != null)
                {
                    var verificaEmail = await _context.Users.FirstOrDefaultAsync(x => x.Email == model.Email.ToLower());
                    if (verificaEmail != null && verificaEmail != user)
                    {
                        return BadRequest(new
                        {
                            error = "Email já cadastrado!"
                        });
                    }
                }

                if(_hashServices.GenerateHash(model.OldPassword, Convert.FromBase64String(user.Salt)) != user.Password)
                {
                    return Unauthorized(new
                    {
                        error = "Senha inválida!"
                    });
                }

                user.Name = (model.Name != null && model.Name != user.Name) ? model.Name : user.Name;
                user.Email = (model.Email != null && model.Email != user.Email) ? model.Email : user.Email;
                user.Password = (model.NewPassword != null && _hashServices.GenerateHash(model.NewPassword, Convert.FromBase64String(user.Salt)) != user.Password) ? _hashServices.GenerateHash(model.NewPassword, Convert.FromBase64String(user.Salt)) : user.Password;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Ok("Usuário atualizado com sucesso");
            }
            catch
            {
                return StatusCode(500, new
                {
                    error = "Erro interno do servidor!"
                });
            }
        }
        [Authorize(Roles ="Admin")]
        [HttpPut]
        [Route(template: "update/user/{id:int}")]
        public async Task<IActionResult> UpdateAdminAsync(
            [FromRoute] int id,
            [FromBody] UserUpdateAdminViewModel model)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                {
                    return BadRequest(new
                    {
                        error = "Usuário não encontrado!"
                    });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (model.Email != null)
                {
                    var verificaEmail = await _context.Users.FirstOrDefaultAsync(x => x.Email == model.Email.ToLower());
                    if (verificaEmail != null && verificaEmail != user)
                    {
                        return BadRequest(new
                        {
                            error = "Email já cadastrado!"
                        });
                    }
                }

                user.Name = (model.Name != null && model.Name != user.Name) ? model.Name : user.Name;
                user.Email = (model.Email != null && model.Email != user.Email) ? model.Email : user.Email;
                user.Password = (model.Password != null && _hashServices.GenerateHash(model.Password, Convert.FromBase64String(user.Salt)) != user.Password) ? _hashServices.GenerateHash(model.Password, Convert.FromBase64String(user.Salt)) : user.Password;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Ok("Usuário atualizado com sucesso");
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
