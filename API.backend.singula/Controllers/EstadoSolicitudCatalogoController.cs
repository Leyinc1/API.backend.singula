using Microsoft.AspNetCore.Mvc;
using Singula.Core.Core.Entities;
using Singula.Core.Repositories;

namespace API.backend.singula.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoSolicitudCatalogoController : BaseCrudController<EstadoSolicitudCatalogo>
    {
        public EstadoSolicitudCatalogoController(IRepository<EstadoSolicitudCatalogo> repository) : base(repository)
        {
        }
    }
}
