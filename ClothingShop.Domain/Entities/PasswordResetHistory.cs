using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ClothingShop.Domain.Enums;

namespace ClothingShop.Domain.Entities
{
    public class PasswordResetHistory
    {
        [Key]
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string Otp { get; set; } = null!;
        public DateTime OtpGeneratedAt { get; set; }
        public DateTime OtpExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime? UsedAt { get; set; }
        public bool IsExpired { get; set; } = false;
        public int AttemptCount { get; set; } = 0;
        public AttemptStatus Status { get; set; } = AttemptStatus.Pending;
    }
}
