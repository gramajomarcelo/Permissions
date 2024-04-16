using AutoMapper;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Permissions.BL.DTOs;
using Permissions.BL.Models;
using Permissions.BL.Services;
using Permissions.BL.Services.Implements;
using System.Text.Json;

namespace Permissions.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GetPermissionController : ControllerBase
    {
        private IMapper _mapper;
        private readonly IGetPermissionsService _getPermissionsService;
        private readonly IProducer<string, string> _kafkaProducer;

        public GetPermissionController(IGetPermissionsService getPermissionsService, IMapper mapper, IProducer<string, string> kafkaProducer)
        {
            _getPermissionsService = getPermissionsService;
            _mapper = mapper;
            _kafkaProducer = kafkaProducer;
        }

        /// <summary>
        /// Obtiene todos los permisos disponibles.
        /// </summary>
        /// <remarks>
        /// Devuelve una lista de permisos en formato DTO.
        /// </remarks>
        /// <returns>Una lista de permisos en formato DTO.</returns>
        /// <response code="200">Devuelve la lista de permisos.</response>
        /// <response code="500">Se produjo un error interno en el servidor.</response>
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<PermissionDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPermissions()
        {
            try
            {
                var permissions = await _getPermissionsService.GetPermissions();
                // Mapea las entidades a DTOs para enviar solo la información necesaria
                var permissionDTOs = permissions.Select(x => _mapper.Map<PermissionDTO>(x));

                foreach (var permission in permissions)
                {
                    var kafkaMessage = new KafkaMessageDTO
                    {
                        Id = new Guid(permission.Id.ToString()),
                        OperationName = "get"
                    };

                    var message = JsonSerializer.Serialize(kafkaMessage);
                    await _kafkaProducer.ProduceAsync("permissions_topic", new Message<string, string> { Value = message });
                }

                return Ok(permissionDTOs);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ocurrió un error al obtener todos los permisos.");
                return StatusCode(500, "Se produjo un error interno en el servidor.");
            }
        }

        /// <summary>
        /// Obtiene un permiso por su ID.
        /// </summary>
        /// <param name="id">El ID del permiso.</param>
        /// <returns>El permiso encontrado.</returns>
        /// <response code="200">Se encontró el permiso.</response>
        /// <response code="404">El permiso no fue encontrado.</response>
        [HttpGet("byId/{id}")]
        [ProducesResponseType(typeof(PermissionDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPermission(int id)
        {
            try
            {
                var permission = await _getPermissionsService.GetPermissionById(id);
                if (permission == null)
                {
                    return NotFound();
                }

                // Mapea la entidad a un DTO para enviar solo la información necesaria
                var permissionDTO = _mapper.Map<PermissionDTO>(permission);

                var kafkaMessage = new KafkaMessageDTO
                {
                    Id = new Guid(permission.Id.ToString()),
                    OperationName = "get"
                };

                var message = JsonSerializer.Serialize(kafkaMessage);
                await _kafkaProducer.ProduceAsync("permissions_topic", new Message<string, string> { Value = message });


                return Ok(permissionDTO);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Ocurrió un error al obtener el permiso con ID {ID}.", id);
                return StatusCode(500, "Se produjo un error interno en el servidor.");
            }
        }

    }
}
