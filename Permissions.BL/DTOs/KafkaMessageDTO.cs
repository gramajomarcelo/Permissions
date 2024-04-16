using System;

namespace Permissions.BL.DTOs
{
    public class KafkaMessageDTO
    {
        public Guid Id { get; set; }
        public string OperationName { get; set; }
        public string Message { get; set; }
    }
}
