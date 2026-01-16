using System.ComponentModel.DataAnnotations;

namespace Survey_Basket_API.Contract.Rquest
{
    public record PollRequest
    (
        
       int Id,
       [Required(ErrorMessage ="كتف امك اكتب التايتل")]
        string Title,
        string Summary,
        bool IsPublished,
        DateOnly StartsAt,
        DateOnly EndsAt
    );
}
