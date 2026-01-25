using System.ComponentModel.DataAnnotations;

namespace Survey_Basket_API.Authentication
{
    public class JwtOptions
    {
        [Required]
        public string Key {  get; init; } =string.Empty;
        [Required]

        public string Issuer { get; init; } = string.Empty;
        [Required]

        public string Audience { get; init; } = string.Empty;
        [Range(1,int.MaxValue)]
        public int ExpireMinutes { get; init; } 
    }
}
