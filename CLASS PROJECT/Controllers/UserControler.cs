using DbRepository.Data;
using DbRepository.Models.DTOs;
using DbRepository.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;
using System.Security.Claims;

namespace CLASS_PROJECT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserControler : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserControler(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager )
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        [HttpPost("/registerTest")]
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
                Email = modelDto.Email
            };

            // Créez l'utilisateur dans la base de données
            var result = await _userManager.CreateAsync(newUser, modelDto.Password);

            if (result.Succeeded)
            {
                // Utilisateur enregistré avec succès
                UserLoginDto userLoginDto = new()
                {
                    Email = modelDto.Email,
                    Password = modelDto.Password
                };
                var user = await _userManager.FindByEmailAsync(modelDto.Email);

                // Appel de Fonction1Async
                await Fonction1Async(modelDto);

                // Vérification du rôle et appel de Fonction2Async si nécessaire
                if (await _userManager.IsInRoleAsync(user, "User"))
                {
                    await Fonction2Async();
                }

                return Ok("Succes");
            }
            else
            {
                // Échec de l'enregistrement de l'utilisateur
                return BadRequest(result.Errors);
            }
        }

        //
        private async Task Fonction1Async(UserSignInDto model)
        {
            UserLoginDto loginModel = new()
            {
                Email = model.Email,
                Password = model.Password
            };

            await Login(loginModel);
            await Task.Delay(2000); // Par exemple, attendre 2 secondes

            MesssageDTO message = new()
            {
                MessageText = "",
                ReceiverId = "admin@example.com"
            };
            await SendMessage(message);
        }
        //

        private async Task Fonction2Async()
        {

            MesssageDTO message = new()
            {
                MessageText = "",
                ReceiverId = "admin@example.com"
            };
            await SendMessage(message);
            await Task.Delay(1000); // Par exemple, attendre 1 seconde
        }


        [HttpPost("/loginTest")]
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

            UserLoginDto userLoginDto = new()
            {
                Email=  user.UserName            

            };
            if (result.Succeeded)
            {
                // Connexion réussie



                return Ok(userLoginDto);
            }
            else
            {
                // Échec de la connexion, renvoyer une erreur
                return BadRequest("Invalid email or password.");
            }
        }
        /*[HttpPost("/AddBookToOrder")]
        public async Task<IActionResult> AddBookToOrder(int orderId, int bookId)
        {
            try
            {
                // Rechercher la commande par son ID
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return NotFound("Commande non trouvée");
                }

                // Rechercher le livre par son ID
                var book = await _context.Books.FindAsync(bookId);
                if (book == null)
                {
                    return NotFound("Livre non trouvé");
                }

                // Créer une nouvelle instance de BookOrder pour associer le livre à la commande
                var bookOrder = new BookOrder
                {
                    Order = order,
                    Book = book
                };

                // Ajouter le BookOrder à la base de données
                _context.BookOrders.Add(bookOrder);
                await _context.SaveChangesAsync();

                return Ok("Livre ajouté à la commande avec succès");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors de l'ajout du livre à la commande : {ex.Message}");
            }
        }


        */



        [HttpPost("/NewOrder")]
        public async Task<IActionResult> New0rder( OrderDTO orderdto)
        {
            if( ModelState.IsValid )
            {
                try
                {
                    var identity = User.Identity as ClaimsIdentity;

                    // var UserId = identity.FindFirst(ClaimTypes.NameIdentifier);
                     var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                    string userId = userIdClaim != null ? userIdClaim.Value : "No ID";

                    //string UserIdString = UserId!.Value;

                    Order newOrder = new Order()
                    {
                        UserID = userId,
                       // OrderDate = DateTime.Now ,
                        OrderStatus = orderdto.OrderStatus,
                        Quantity=orderdto.Quantity,
                        Price=orderdto.Price,
                        BookID=orderdto.BookID,
                        
                    };
                    var result = await _context.Orders.AddAsync(newOrder);
                    await _context.SaveChangesAsync();
                    return Ok("Succes");
                }catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
            else
            {
                return BadRequest(" Something went wrong ");
            }
        }

        [HttpGet("/GetBookStock")]
        public async Task<int> GetBookStock(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book != null)
            {
                return book.Stock;
            }
            else
            {

                return -1;
            }
        }


        [HttpPost("/IncrementBookStock")]
        public async Task<IActionResult> IncrementBookStock(int bookId, int addVal)
        {
            try
            {
                // 
                var book = await _context.Books.FindAsync(bookId);
                if (book == null)
                {
                    return NotFound("Livre non trouvé");
                }

                //  
                book.Stock += addVal;

                // 
                _context.Books.Update(book);
                await _context.SaveChangesAsync();

                return Ok("La valeur de Stock a été incrémentée avec succès");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors de l'incrémentation de la valeur de Stock : {ex.Message}");
            }
        }


        //

        [HttpPost("/IncrementOderPrice")]
        public async Task<IActionResult> IncrementOderPrice(int orderId, float addVal)
        {
            try
            {
                // 
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return NotFound("Livre non trouvé");
                }

                //  
                order.Price += addVal;

                // 
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();

                return Ok("La valeur de Stock a été incrémentée avec succès");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors de l'incrémentation de la valeur du prix : {ex.Message}");
            }
        }
      


        [Authorize("RequireAdminRole")]
        [HttpGet("/Handshake")]
        public string Handshake()
        {
            return "HANDSHAKE ";
        }

        [HttpPost("/SendMessage")]
        [Authorize]
        public async Task<IActionResult> SendMessage(MesssageDTO messagedto)
        {

            if (ModelState.IsValid) 
            {
                try
                {
                    var identity = User.Identity as ClaimsIdentity;



                    //  reecupere les donnée de mon claim
                    var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
                    //var userEmailClaim = identity.FindFirst(ClaimTypes.Email );
                    string UsedId = userIdClaim.Value;


                    // le receiver Id fonctionne plutot avec le email et non l'Id
                    /*if( await _userManager.IsInRoleAsync("User"))
                    {

                    }*/
                    var user = await _userManager.GetUserAsync(User);


                    bool isUserRole = await _userManager.IsInRoleAsync(user, "User");

                    // Définir le ReceiverId en fonction du rôle de l'utilisateur
                    string receiverId = isUserRole ? "admin@example.com" : messagedto.ReceiverId!;

                    // Créer le message
                    Message newMessage = new Message
                    {
                        MessageText = messagedto.MessageText,
                        UserId = UsedId,
                        ReceiverId = receiverId
                    };


                    await _context.Messages.AddAsync(newMessage);
                    await _context.SaveChangesAsync();

                    return Ok(" SUCCES");

                }catch (Exception ex)
                {
                    return  BadRequest(ex.Message);
                }

            }
            else
            {
                return BadRequest("SOMETHING WENT WRONG");
            }
        }

        [Authorize]

        [HttpPost("/UserTest")]
        public string UserTest()
        {
            var identity = User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                return "Error";
            }

            
            var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            var userNameClaim = identity.FindFirst(ClaimTypes.Name);
            var userNameClaime = identity.FindFirst(ClaimTypes.Email );


            string userId = userIdClaim != null ? userIdClaim.Value : "No ID";
            string userName = userNameClaim != null ? userNameClaim.Value : "No Name";

            if (User.IsInRole("Admin"))
            {
                return $"Admin User: {userName} (ID: {userId})";
            }
            else if (User.IsInRole("User"))
            {
                return $"Regular User: {userName} (ID: {userId})";
            }
            else
            {
                return "Error";
            }
        }

        // [HttpPost("/CheckLog")]
        [HttpGet("/CheckLog")]

        public async Task<bool>   CheckLog()
        {
            var user = await _userManager.GetUserAsync(User);

            //await _userManager.IsInRoleAsync(user, "Admin")

            if ((await _userManager.IsInRoleAsync(user, "Admin")) || (await _userManager.IsInRoleAsync(user, "User")))
            {
                
                return true ;
            }
            else
            {
                return false;
            }
        }

        [HttpGet("/GetUser")]
        [Authorize]
        public async Task<UserSignInDto> GetUser()
        {
            var identity = User.Identity as ClaimsIdentity;

             

        
            var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            var userName = identity.FindFirst(ClaimTypes.Name);
            var userEmail = identity.FindFirst(ClaimTypes.Email);
            string email = userEmail.Value;
            string name = userName.Value;
          //  int customId = userIdClaim.Value:

            UserSignInDto newUser = new()
            {
                Email= email,
                Name=name
            };
            return newUser;
        }


        [HttpGet("/GetMyMessages")]
        [Authorize]
        public async Task<IActionResult> GetMyMessages()
        {
            try
            {
                var identity = User.Identity as ClaimsIdentity;
                var userEmailClaim = identity?.FindFirst(ClaimTypes.Email);
                if (userEmailClaim == null)
                {
                    return BadRequest("Unable to identify the user.");
                }

                var userEmail = userEmailClaim.Value;

                
                var receivedMessages = await _context.Messages
                    .Where(m => m.ReceiverId == userEmail)
                    .ToListAsync();

                // Fetch the user's ID
                var userIdClaim = identity?.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return BadRequest("Unable to identify the user.");
                }

                var userId = userIdClaim.Value;

               
                var sentMessages = await _context.Messages
                    .Where(m => m.UserId == userId)
                    .ToListAsync();

                // 
                if (!receivedMessages.Any() && !sentMessages.Any())
                {
                    return Ok(new { receivedMessages = new List<object>(), sentMessages = new List<object>() });
                }

                // 
                var senderIds = receivedMessages.Concat(sentMessages).Select(m => m.UserId).Distinct();
                var senders = await _userManager.Users
                    .Where(u => senderIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => u.Email);

                var groupedReceivedMessages = receivedMessages.GroupBy(m => senders[m.UserId])
                    .Select(group => new
                    {
                        SenderEmail = group.Key,
                        Messages = group.Select(m => new
                        {
                            m.MessageId,
                            m.MessageText,
                            m.UserId,
                            m.ReceiverId
                        }).ToList()
                    })
                    .ToList();

                var groupedSentMessages = sentMessages.GroupBy(m => senders[m.UserId])
                    .Select(group => new
                    {
                        SenderEmail = group.Key,
                        Messages = group.Select(m => new
                        {
                            m.MessageId,
                            m.MessageText,
                            m.UserId,
                            m.ReceiverId
                        }).ToList()
                    })
                    .ToList();

                return Ok(new { receivedMessages = groupedReceivedMessages, sentMessages = groupedSentMessages });
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while fetching the messages: {ex.Message}");
            }
        }




        [HttpGet("/IslogAdminTest")]
        
        public async Task<bool> IslogAdminTest()
        {
            var user = await _userManager.GetUserAsync(User);
            var result = false;
            result = await _userManager.IsInRoleAsync( user, "Admin");
            if ( result == true )
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        // [Htt("/DeleteBook")]
        [HttpDelete("/DeleteBook/{bookId}")]
        [Authorize("RequireAdminRole")]
        public async Task<IActionResult> DeleteBook(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
            {
                return NotFound("Book not found");
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return Ok("Success");
        }
        [HttpGet("/GetMyUsersEmail")]
        //[Authorize("RequireAdminRole")]
        public async Task<IActionResult > GetMyUsersEmail()
        {
            // var result = await  _context.Users.Select(a => a.Email).Where( b => b.Ema )
            var result = await _context.Users.Where(a => a.Email != "admin@example.com").Select(b => b.Email).ToListAsync();
            return Ok(result);
        }

        [HttpPost("/EditUserProfile")]
        public async Task<IActionResult> EditUserProfile(EditProfileDto  model )
        {
            var monuser = await _userManager.FindByIdAsync(model.UserId);
            if (monuser == null)
            {
                return NotFound(" User Not Found");
            }

            monuser.UserName = model.UserName;
            monuser.UserEmail = model.UserEmail;
            // Sauvegarder les modifications dans la base de données
            var result = await _userManager.UpdateAsync(monuser);
            if (result.Succeeded)
            {
                return Ok("User profile updated successfully");
            }
            else
            {
                return BadRequest("Failed to update user profile");
            }


        }
        public class EditProfileDto
        {
            public string UserId { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;

           // public string? Password { get; set; } = string.Empty;
            public string? UserEmail { get; set; } = string.Empty;



        }
    }
}