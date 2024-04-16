using System;
using System.Collections.Generic;

namespace Permissions.BL.Models;

public partial class Permission
{
    public int Id { get; set; }

    public string EmployeeForename { get; set; } = null!;

    public string EmployeeSurname { get; set; } = null!;

    public int PermissionType { get; set; }

    public DateTime PermissionDate { get; set; }

    public virtual PermissionType PermissionTypeNavigation { get; set; } = null!;
}
