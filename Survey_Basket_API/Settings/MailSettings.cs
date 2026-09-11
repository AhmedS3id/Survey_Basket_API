using System.ComponentModel.DataAnnotations;

namespace Survey_Basket_API.Settings
{
    public class MailSettings
    {
        [Required, EmailAddress]
        public String Mail { get; set; } = string.Empty;
        [Required]
        public String DisplayName { get; set; } = string.Empty;
        [Required]
        public String Password { get; set; } = string.Empty;
        [Required]
        public String Host { get; set; } = string.Empty;
        [Required]
        public int Port { get; set; }
    }
}
