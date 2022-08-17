using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QulixTest.Core.Domain;
using QulixTest.Core.IRepositories;
using QulixTest.Core.Model;
using QulixTest.Persistence.AuthServive;

namespace QulixTest.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<Author> _userManager;
    private readonly ILogger<AccountController> _logger;
    private readonly IMapper _mapper;
    private readonly IAuthManager _authManager;
    private readonly IUnitOfWork _unitOfWork;

    public AccountController(UserManager<Author> userManager,
        ILogger<AccountController> logger, IMapper mapper,
        IAuthManager auth, IUnitOfWork unitOfWork
        )
    {
        _userManager = userManager;
        _logger = logger;
        _mapper = mapper;
        _authManager = auth;
        _unitOfWork = unitOfWork;
    }


    [HttpPost]
    [Route("register")]

    public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogInformation($"Invalid Registration Attempt for {userDTO.Email}");
            return BadRequest(ModelState);
        }

        var userExist = await _userManager.FindByNameAsync(userDTO.Email);
        if (userExist != null)
        {
            return BadRequest(userExist + " Already exist user");
        }
        var user = _mapper.Map<Author>(userDTO);
        user.UserName = userDTO.Email;
        user.SecurityStamp = Guid.NewGuid().ToString();

        var result = await _userManager.CreateAsync(user, userDTO.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return BadRequest("User Registration Attemp Failed");
        }

        return Accepted("Accepted success");
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)
    {
        _logger.LogInformation($"Login Attempt for {userDTO.Email}");
        if (!ModelState.IsValid)
        {
            _logger.LogInformation($"Invalid Login Attempt for {userDTO.Email}");
            return BadRequest(ModelState);
        }

        if (!await _authManager.ValidateUser(userDTO))
        {
            return Unauthorized();
        }

        return Accepted(new { Token = await _authManager.CreateToken() });
    }

    //[Authorize(Roles = "SuperAdmin")]
    //[Authorize(Roles = "Admin")]
    [HttpGet("api/account/users")]
    [ResponseCache(CacheProfileName = "SecondsDuration")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllUsers([FromQuery] RequestParams requestParams)
    {
        var users = await _unitOfWork.Authors.GetAllPaged(requestParams);
        var results = _mapper.Map<IList<UserDTO>>(users);
        return Ok(results);
    }

    [HttpGet("{id:long}")]
    [ResponseCache(CacheProfileName = "SecondsDuration")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAuthor(string id)
    {
        var author = await _unitOfWork.Authors.GetAllAsync(q => q.Id == id);
        if (author == null)
        {
            _logger.LogError($"Invalid User attemp in {nameof(GetAuthor)}");
            return BadRequest("Submitted data is invalid");
        }
        return Ok(author);

    }

}
