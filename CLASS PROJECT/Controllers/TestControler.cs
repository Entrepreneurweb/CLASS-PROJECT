using DbRepository.Data;
using DbRepository.Models.DTOs;
using DbRepository.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CLASS_PROJECT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestControler : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public TestControler( ApplicationDbContext context , UserManager<User> userManager, SignInManager<User> signInManager    )
        {

            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /*
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserSignInDto modelDto)
        {
            // Vérifiez si l'email est déjà utilisé
            var existingUser = await _userManager.FindByEmailAsync(modelDto.Email);
            if (existingUser != null)
            {
                return BadRequest("Email is already registered.");
            }

            User newUser = new User
            {
                UserName = modelDto.Name,
                UserEmail = modelDto.Email,
                Email=modelDto.Email
            };

            // Créez l'utilisateur dans la base de données
            var result = await _userManager.CreateAsync(newUser, modelDto.Password);

            if (result.Succeeded)
            {
                // Utilisateur enregistré avec succès
                return Ok("User registered successfully.");
            }
            else
            {
                // Échec de l'enregistrement de l'utilisateur
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto modelDto)
        {
            // Vérifier si l'utilisateur existe dans la base de données
            var user = await _userManager.FindByEmailAsync(modelDto.Email);

            if (user == null)
            {
                // Utilisateur non trouvé, renvoyer une erreur
                return BadRequest("User with this email does not exist.");
            }

            // Tenter de connecter l'utilisateur avec le mot de passe fourni
            var result = await _signInManager.PasswordSignInAsync(user.UserName, modelDto.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Connexion réussie
                return Ok("Logged in successfully.");
            }
            else
            {
                // Échec de la connexion, renvoyer une erreur
                return BadRequest("Invalid email or password.");
            }
        }

        */

    }
}
