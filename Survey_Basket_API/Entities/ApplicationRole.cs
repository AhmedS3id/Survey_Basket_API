using Microsoft.AspNetCore.Identity;

namespace Survey_Basket_API.Entities;

    public class ApplicationRole :IdentityRole
    {
        public bool IsDefault { get; set; }

        public bool IsDeleted { get; set; }
    }

