namespace ClothingShop.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }

        // Marketing Info
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; } // Male, Female, Other

        // Trạng thái tài khoản
        public bool IsActive { get; set; } = true; // True: Hoạt động, False: Bị khóa
        public string? LockReason { get; set; }

        // Authentication
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        // FK
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public Cart? Cart { get; set; } // 1 User có 1 giỏ hàng
        public ICollection<PasswordResetHistory> PasswordResetHistories { get; set; } = new List<PasswordResetHistory>();

    }
}
