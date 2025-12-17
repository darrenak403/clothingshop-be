using ClothingShop.Domain.Enums;

namespace ClothingShop.Domain.Entities
{
    public class PasswordResetHistory
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string Otp { get; set; } = null!;
        public DateTime OtpGeneratedAt { get; set; }
        public DateTime OtpExpriesdAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime UsedAt { get; set; }
        public DateTime IsExpried { get; set; }
        public int AttemptCount { get; set; } = 0;
        public AttemptStatus Status { get; set; } = AttemptStatus.Pending;
    }
}
