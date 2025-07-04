using System.Collections.Generic;

namespace ObligatorioProgramacion3_Francisco_Luis.Models
{
    public class PermissionCheckbox
    {
        public int PermissionID { get; set; }
        public string PermissionName { get; set; }
        public bool IsAssigned { get; set; }
    }

    public class EditRoleViewModel
    {
        public int ID { get; set; }
        public string RoleName { get; set; }
        public List<PermissionCheckbox> Permissions { get; set; }
    }
}
