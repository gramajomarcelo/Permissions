using AutoMapper;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Permissions.BL.DTOs;
using Permissions.BL.Models;
using Permissions.BL.Services;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Permissions.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestPermissionController : ControllerBase
    {
        private IMapper _mapper;
        private readonly IRequestPermissionService _requestPermissionService;
        private readonly IProducer<string, string> _kafkaProducer;

        public RequestPermissionController(IRequestPermissionService requestPermissionService, IMapper mapper, IProducer<string, string> kafkaProducer)
        {
            _requestPermissionService = requestPermissionService;
            _mapper = mapper;
            _kafkaProducer = kafkaProducer;
        }

        /// <summary>
        /// Crea un nuevo permiso.
        /// </summary>
        /// <param name="permission">Datos del permiso a crear.</param>
        /// <returns>El permiso creado.</returns>
        /// <response code="201">El permiso fue creado exitosamente.</response>
        /// <response code="400">Los datos del permiso son inválidos.</response>
        /// <response code="500">Se produjo un error interno en el servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(Permission), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreatePermission([FromBody] PermissionDTO permissionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Mapea el PermissionDto a una entidad Permission
                var permission = _mapper.Map<Permission>(permissionDto);

                var newPermission = await _requestPermissionService.RequestPermission(permission);
                var newpermissionDto = _mapper.Map<PermissionDTO>(permission);

                var kafkaMessage = new KafkaMessageDTO
                {
                    Id = new Guid(newpermissionDto.Id.ToString()),
                    OperationName = "request"
                };

                var message = JsonSerializer.Serialize(kafkaMessage);
                await _kafkaProducer.ProduceAsync("permissions_topic", new Message<string, string> { Value = message });


                return StatusCode(201, newpermissionDto);
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                // Registra la excepción
                // Envia notificación por correo electrónico
                // Loggea la excepción

                return StatusCode(500, "Se produjo un error interno en el servidor.");
            }
        }
    }
}
