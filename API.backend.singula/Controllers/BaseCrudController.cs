using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Singula.Core.Repositories;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace API.backend.singula.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseCrudController<TEntity> : ControllerBase where TEntity : class
    {
        protected readonly IRepository<TEntity> _repository;

        public BaseCrudController(IRepository<TEntity> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAll()
        {
            var list = await _repository.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Get([FromRoute] object id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] TEntity entity)
        {
            var created = await _repository.CreateAsync(entity);
            return CreatedAtAction(nameof(Get), new { id = GetKeyValue(created) }, created);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update([FromRoute] object id, [FromBody] TEntity entity)
        {
            // try to set key if possible
            var keyProp = GetKeyProperty(entity);
            if (keyProp != null)
            {
                keyProp.SetValue(entity, ConvertIdType(keyProp.PropertyType, id));
            }
            var updated = await _repository.UpdateAsync(entity);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete([FromRoute] object id)
        {
            // If entity has property named "Estado" or "EsActivo" perform logical delete when possible
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var tipo = entity.GetType();
            var propEstado = tipo.GetProperty("Estado", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var propEsActivo = tipo.GetProperty("EsActivo", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (propEstado != null && propEstado.PropertyType == typeof(string))
            {
                propEstado.SetValue(entity, "deleted");
                var res = await _repository.UpdateAsync(entity);
                return res == null ? NotFound() : NoContent();
            }

            if (propEsActivo != null && (propEsActivo.PropertyType == typeof(bool) || propEsActivo.PropertyType == typeof(bool?)))
            {
                if (propEsActivo.PropertyType == typeof(bool)) propEsActivo.SetValue(entity, false);
                else propEsActivo.SetValue(entity, false);
                var res = await _repository.UpdateAsync(entity);
                return res == null ? NotFound() : NoContent();
            }

            var deleted = await _repository.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        private PropertyInfo? GetKeyProperty(object entity)
        {
            var tipo = entity.GetType();
            var key = tipo.GetProperties().FirstOrDefault(p => p.GetCustomAttributes().Any(a => a.GetType().Name == "KeyAttribute"));
            if (key != null) return key;
            // fallback to first property starting with Id
            key = tipo.GetProperty(tipo.Name.StartsWith("I") ? "Id" + tipo.Name.Substring(1) : "Id" + tipo.Name);
            if (key != null) return key;
            key = tipo.GetProperties().FirstOrDefault(p => p.Name.ToLower().StartsWith("id"));
            return key;
        }

        private object? GetKeyValue(object entity)
        {
            var prop = GetKeyProperty(entity);
            return prop?.GetValue(entity);
        }

        private object? ConvertIdType(Type t, object id)
        {
            try
            {
                if (t == typeof(int) || t == typeof(int?)) return Convert.ToInt32(id);
                if (t == typeof(long) || t == typeof(long?)) return Convert.ToInt64(id);
                if (t == typeof(string)) return id.ToString();
                return id;
            }
            catch
            {
                return id;
            }
        }
    }
}
