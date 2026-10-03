using System;
using System.Collections.Generic;

namespace JobPortal.Domain.Models
{
    public class AdminSubRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        // When false, the sub-role only sees the forms listed in Forms.
        public bool AllFormsAccess { get; set; } = true;

        public List<AdminSubRolePermission> Permissions { get; set; } = new();
        public List<AdminSubRoleForm> Forms { get; set; } = new();
    }

    public class AdminSubRolePermission
    {
        public int Id { get; set; }
        public int SubRoleId { get; set; }
        public string Section { get; set; } = "";

        // 1 = view only, 2 = full access.
        public int AccessLevel { get; set; }

        public AdminSubRole? SubRole { get; set; }
    }

    public class AdminSubRoleForm
    {
        public int Id { get; set; }
        public int SubRoleId { get; set; }
        public int FormTypeCategoryId { get; set; }

        public AdminSubRole? SubRole { get; set; }
    }

    // An admin user with no row here is a Super Admin (full access).
    public class AdminUserSubRole
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public int SubRoleId { get; set; }

        // UTC. The assignment stops granting access at this moment.
        public DateTime ValidUntil { get; set; }

        // When false, this admin only sees submissions of the users listed in Users.
        public bool AllUsersAccess { get; set; } = true;
        public List<AdminUserSubRoleUser> Users { get; set; } = new();

        public AdminSubRole? SubRole { get; set; }
    }

    public class AdminUserSubRoleUser
    {
        public int Id { get; set; }
        public int AssignmentId { get; set; }
        public string TargetUserId { get; set; } = "";

        public AdminUserSubRole? Assignment { get; set; }
    }
}
