using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QulixTest.Core.Domain;
using QulixTest.Core.IRepositories;
using QulixTest.Core.Model;

namespace QulixTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TextController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TextController> _logger;
        private readonly IMapper _mapper;

        public TextController(IUnitOfWork unitOfWork, ILogger<TextController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ResponseCache(CacheProfileName = "SecondsDuration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllText([FromQuery] RequestParams requestParams)
        {
            var texts = await _unitOfWork.Texts.GetAllPaged(requestParams);
            var results = _mapper.Map<IList<TextDTO>>(texts);
            return Ok(results);
        }

        [HttpGet("{id:long}", Name = "GetCollection")]
        /// declaring caching annotation
        [ResponseCache(CacheProfileName = "SecondsDuration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetText(long id)
        {
            var text = await _unitOfWork.Texts.Get(q => q.Id == id, null);
            var result = _mapper.Map<TextDTO>(text);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateText([FromBody] CreateTextDTO textnDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid Post attempt in {nameof(CreateText)}");
                return BadRequest(ModelState);
            }

            var text = _mapper.Map<Text>(textnDTO);

            await _unitOfWork.Texts.Insert(text);

            await _unitOfWork.SaveAsync();

            return CreatedAtRoute("GetCollection", new { id = text.Id }, text);
        }

        [Authorize]
        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateText(long id, [FromBody] UpdateTextDTO updateText)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid Update attemp in {nameof(UpdateText)}");
                return BadRequest(ModelState);
            }

            var text = await _unitOfWork.Texts.Get(option => option.Id == id, null);
            if (text == null)
            {
                _logger.LogError($"Invalid Update attemp in {nameof(UpdateText)}");
                return BadRequest("Submitted data is invalid");
            }

            _mapper.Map(updateText, text);
            _unitOfWork.Texts.Update(text);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }

        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteText(long id)
        {
            if (id < 1)
            {
                _logger.LogError($"Invalid ID attemp in {nameof(DeleteText)}");
                return BadRequest(ModelState);
            }
            var text = await _unitOfWork.Texts.Get(q => q.Id == id, new List<string> { "Files" });
            if (text == null)
            {
                _logger.LogError($"Invalid Delete attemp in {nameof(DeleteText)}");
                return BadRequest("Submitted data is invalid");
            }

            await _unitOfWork.Texts.Delete(text.Id);
            await _unitOfWork.SaveAsync();

            return NoContent();
        }
    }
}
