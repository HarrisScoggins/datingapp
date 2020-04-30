using System.Net;
using System.Threading.Tasks;
using datingapp.api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using datingapp.api.Models;
using datingapp.api.Dtos;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace datingapp.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly  IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            //validate request 
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            if(await _repo.UserExists(userForRegisterDto.Username))
            {
                 return BadRequest("Username already exists");                   
            }
            var userToCreate = new User {
                Username = userForRegisterDto.Username 
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);
            
           // return CreatedAtRoute()
           return StatusCode(201);
            
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            throw new Exception("nooooo");
        
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);
            if(userFromRepo == null)
                return Unauthorized();

            var claims = new []
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
   
            var tokenHandler = new JwtSecurityTokenHandler();  
            var token = tokenHandler.CreateToken(tokenDescriptor);
     
            return Ok(new  
            {
                token = tokenHandler.WriteToken(token)
            });
                     

            // var tokenHandler = new JsonWebTokenHandler(); 
            // var token = tokenHandler.CreateToken(tokenDescriptor);

            // return Ok(new {
            //      token = tokenHandler.ReadJsonWebToken(token) 
            // });

        } 
        
    }
}