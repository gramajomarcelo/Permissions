using AutoMapper;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Permissions.BL.DTOs;
using Permissions.BL.Models;
using Permissions.BL.Services;
using System;
using System.Security;
using System.Text.Json;
using System.Threading.Tasks;

namespace Permissions.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModifyPermissionController : ControllerBase
    {
        private IMapper _mapper;
        private readonly IModifyPermissionService _modifyPermissionService;
        private readonly IProducer<string, string> _kafkaProducer;

        public ModifyPermissionController(IModifyPermissionService modifyPermissionService, IMapper mapper, IProducer<string, string> kafkaProducer)
        {
            _modifyPermissionService = modifyPermissionService ?? throw new ArgumentNullException(nameof(modifyPermissionService));
            _mapper = mapper;
            _kafkaProducer = kafkaProducer;
        }

        /// <summary>
        /// Modifica un permiso existente por su ID.
        /// </summary>
        /// <param name="id">El ID del permiso a modificar.</param>
        /// <param name="modifiedPermission">La información actualizada del permiso en un DTO.</param>
        /// <returns>El permiso modificado en un DTO.</returns>
        /// <response code="200">Devuelve el permiso modificado.</response>
        /// <response code="400">La solicitud no es válida o el ID no coincide con el permiso proporcionado.</response>
        /// <response code="404">El permiso no fue encontrado.</response>
        /// <response code="500">Se produjo un error interno en el servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PermissionDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePermission(int id, [FromBody] PermissionDTO modifiedPermission)
        {
            try
            {
                // Mapea el PermissionDto a una entidad Permission
                var permission = _mapper.Map<Permission>(modifiedPermission);

                var updatedPermission = await _modifyPermissionService.ModifyPermission(id, permission);
                var updatedPermissionDto = _mapper.Map<PermissionDTO>(updatedPermission);

                var kafkaMessage = new KafkaMessageDTO
                {
                    Id = new Guid(updatedPermissionDto.Id.ToString()),
                    OperationName = "modify"
                };

                var message = JsonSerializer.Serialize(kafkaMessage);
                await _kafkaProducer.ProduceAsync("permissions_topic", new Message<string, string> { Value = message });


                return Ok(updatedPermissionDto);
            }
            catch (SecurityException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Se produjo un error interno en el servidor.");
            }
        }
    }
}
