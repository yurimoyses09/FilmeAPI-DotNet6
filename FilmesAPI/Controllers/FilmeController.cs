using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class FilmeController : ControllerBase
{
	private FilmeContext _context;
	private IMapper _mapper;

    public FilmeController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
	public IActionResult AdicionaFilme([FromBody] CreateFilmeDto filmeDto)
	{
		Filme filme = _mapper.Map<Filme>(filmeDto);

		_context.Filmes.Add(filme);
		_context.SaveChanges();

		return CreatedAtAction(nameof(RecuperaFilmePorId), new { id = filme.Id }, filme);
	}

	[HttpGet]
	public IActionResult RecuperaFilmes([FromQuery]int skip = 0, [FromQuery]int take = 50)
	{
		var filmes = _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take));

		return Ok(new { data = filmes.ToList() });
	}

    [HttpGet("{id}")]
    public IActionResult RecuperaFilmePorId(int id)
    {
		var filme = _context.Filmes.Where(x => x.Id == id).FirstOrDefault();
		if (filme == null)
			return NotFound();

        var filmedto = _mapper.Map<ReadFilmeDto>(filme);

        return Ok(new { data = filmedto });
    }

	[HttpPut("{id}")]
	public IActionResult AtualizaFilme(int id, [FromBody] UpdateFilmeDto FilmeDto)
	{
		try
		{
            var filme = _context.Filmes.FirstOrDefault(x => x.Id == id);

            if (filme == null) return NotFound();

            _mapper.Map(FilmeDto, filme);
            
			_context.SaveChanges();

            return NoContent();
        }
		catch (Exception ex)
		{
			return BadRequest(new { data = ex.Message });
		}
	}

	[HttpPatch("{id}")]
	public IActionResult AtualizaFilmeParcial(int id, JsonPatchDocument<UpdateFilmeDto> jsonPatch)
	{
        try
        {
            var filme = _context.Filmes.FirstOrDefault(x => x.Id == id);

            if (filme == null) return NotFound();

			var filmeParcial = _mapper.Map<UpdateFilmeDto>(filme);
			jsonPatch.ApplyTo(filmeParcial, ModelState);

			if (!TryValidateModel(filmeParcial)) return ValidationProblem(ModelState);

            _mapper.Map(filmeParcial, filme);
            _context.SaveChanges();

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { data = ex.Message });
        }
    }

	[HttpDelete("{id}")]
	public IActionResult DeletaFilme(int id)
	{
        try
        {
            var filme = _context.Filmes.FirstOrDefault(x => x.Id == id);

            if (filme == null) return NotFound();

            _context.Remove(filme);
            _context.SaveChanges();

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { data = ex.Message });
        }
    }
}
