using Microsoft.AspNetCore.Mvc;
using Singula.Core.Core.Entities;
using Singula.Core.Repositories;

namespace API.backend.singula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoAlertaCatalogoController : BaseCrudController<EstadoAlertaCatalogo>
    {
        public EstadoAlertaCatalogoController(IRepository<EstadoAlertaCatalogo> repository) : base(repository)
        {
        }
    }
}
