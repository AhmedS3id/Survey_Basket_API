using System.ComponentModel.DataAnnotations;

namespace Survey_Basket_API.Contract.Rquest
{
    public record PollRequest
    (
        [Required(ErrorMessage ="كتف امك اكتب التايتل")]
        string Title,
         string Description
    );
}
