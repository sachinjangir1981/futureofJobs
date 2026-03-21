using System;

namespace JobPortal.Domain.Models;
public class EmployerProfile
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string WhatsappNo { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;

    public int RecType { get; set; }
    
}
