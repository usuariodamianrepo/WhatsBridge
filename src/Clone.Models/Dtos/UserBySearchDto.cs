namespace Clone.Models.Dtos
{
    public sealed class UserBySearchDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public string? Roles { get; set; }
        public bool IsLocked => LockoutEnd > DateTimeOffset.UtcNow;
    }
}

