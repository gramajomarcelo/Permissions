using AutoMapper;
using Permissions.BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permissions.BL.DTOs
{
    public class MapperConfig
    {
        public static MapperConfiguration MapperConfiguration()
        {

            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Permission, PermissionDTO>();
                cfg.CreateMap<PermissionDTO, Permission>();

                cfg.CreateMap<PermissionType, PermissionTypeDTO>();
                cfg.CreateMap<PermissionTypeDTO, PermissionType>();
            });
        }
    }
}
