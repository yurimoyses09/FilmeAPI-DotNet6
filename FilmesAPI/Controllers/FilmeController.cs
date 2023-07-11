using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
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
		var filmes = _context.Filmes.Skip(skip).Take(take);

		return Ok(new { data = filmes.ToList() });
	}

    [HttpGet("{id}")]
    public IActionResult RecuperaFilmePorId(int id)
    {
		var filme = _context.Filmes.Where(x => x.Id == id).FirstOrDefault();
		if (filme == null)
			return NotFound();

        return Ok(new { data = filme });
    }
}
