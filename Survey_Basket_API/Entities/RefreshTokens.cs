namespace Survey_Basket_API.Entities
{
    [Owned]
    public class RefreshTokens
    {
        public string Token { get; set; } =string.Empty;
        public DateTime ExpireOn { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedOn { get; set; }
        public bool Is_Expired=>DateTime.UtcNow>=ExpireOn;
        public bool Is_Active => RevokedOn is null && !Is_Expired;

    }
}
