using System;

namespace JobPortal.Domain.Models
{
    public class AdminShareLink
    {
        public int Id { get; set; }

        public string Token { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        // Comma-separated FormTypeCategory ids this link is scoped to; null/empty means all forms the user filled.
        public string? FormIdsCsv { get; set; }

        public string CreatedByUserId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        public DateTime? RevokedAt { get; set; }
    }
}
