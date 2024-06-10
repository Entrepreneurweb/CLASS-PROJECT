using DbRepository.Data;
using DbRepository.Models.DTOs;
using DbRepository.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using static System.Reflection.Metadata.BlobBuilder;

namespace CLASS_PROJECT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminControler : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IWebHostEnvironment  _webHostEnvironment;
        public AdminControler(ApplicationDbContext context, UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpPost("/AddBook")]
        [Authorize("RequireAdminRole")]
        public async Task<IActionResult> AddBook([FromForm] BookDTO bookDTO, IFormFile image)
        {
            if (bookDTO == null || image == null || image.Length == 0)
            {
                return BadRequest(new { Success = false, Message = "Invalid book data or no image uploaded." });
            }

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            // Vérifiez si le dossier existe déjà
            if (!Directory.Exists(uploadsFolder))
            {
                // Si le dossier n'existe pas, créez-le
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            try
            {
                // Copiez le fichier de l'image dans le dossier sur le serveur
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                // Créez l'objet Book en utilisant le chemin relatif de l'image
                Book book = new()
                {
                    Title = bookDTO.Title,
                    Author = bookDTO.Author,
                    ISBN = bookDTO.ISBN,
                    Price = bookDTO.Price,
                    Description = bookDTO.Description,
                    Genre = bookDTO.Genre,
                    Stock = bookDTO.Stock,
                    PublicationDate= DateTime.Now.ToString("G"),
                    ImagePath = Path.Combine("images", uniqueFileName) // Enregistrez le chemin relatif du fichier dans la base de données
                };

                // Enregistrez le livre dans la base de données
                await _context.Books.AddAsync(book);
                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Book uploaded successfully." });
            }
            catch (Exception ex)
            {
                // En cas d'erreur, supprimez le fichier d'image si elle a été enregistrée
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                return BadRequest(ex.Message);
            }
        }





        //

        [HttpGet("/GetBookStockAdmin")]
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
        
        [HttpPost("/IncrementBookStockAdmin")]
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
        [HttpGet("/GetBook")]

        public async Task<IActionResult> GetBook(int bookId )
        {

            try
            {
                // 
                var book = await _context.Books.FindAsync(bookId);
                if (book == null)
                {
                    return NotFound("Livre non trouvé");
                }

                 

                return Ok( book);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors de l'incrémentation de la valeur de Stock : {ex.Message}");
            }
        }

        [HttpGet("/GetAllBooks")]

        public async Task<IActionResult> GetAllBooks()
        {

            try
            {
                // 
                var book = await _context.Books.ToListAsync();  
                if (book == null)
                {
                    return NotFound("Livre non trouvé");
                }



                 return Ok(book);
              
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors de l'incrémentation de la valeur de Stock : {ex.Message}");
            }
        }


        [HttpPost("/SendNotification")]
        [Authorize("RequireAdminRole")]
        public async Task<IActionResult> SendNotification( [FromBody]  NotificationDTO model )
        {
            if( ModelState.IsValid )
            {
                Notification notification = new()
                {
                    Content = model.Content,
                    Title = model.Title,
                    IssuedDate = DateTime.Now.ToString("G"),
                };
                try
                {
                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();
                }
                catch ( Exception ex )
                {
                    return BadRequest( ex.Message );
                }


                return Ok( "succes" );

            }
            else
            {
                return BadRequest(" something went wrong ");
            }

        }

        //

        [HttpPost("/DeleteNotification")]
        [Authorize("RequireAdminRole")]
        public async Task<IActionResult> DeleteNotification([FromBody] DeleteNotificationRequest request)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (notification == null)
            {
                return NotFound("Notification not found");
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return Ok("Deleted");
        }

        public class DeleteNotificationRequest
        {
            public int Id { get; set; }
        }


        //

        [HttpGet("/GetNotification")]
        
        public async Task<List<Notification >> GetNotification( )
        {
            var notifications =  await _context.Notifications.ToListAsync();
                
            return notifications;


        }

        [HttpPost("/LogOut")]

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Ok("LogOut Succes");

        }

      /*  [HttpGet("/GetBookImage/{id}")]
        public async Task<IActionResult> GetBookImage(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null || book.Image == null)
                {
                    return NotFound("Livre non trouvé ou image non disponible");
                }

                return File(book.Image, "image/jpeg"); // Changer le type de fichier si nécessaire
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors de la récupération de l'image du livre : {ex.Message}");
            }
        }*/

    }
}
