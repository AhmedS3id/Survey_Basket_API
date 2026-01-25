namespace Survey_Basket_API.Contract.Poll
{
    public record PollResponse
    (
        int Id,
        string Title,
        string Summary ,
        DateOnly StartsAt ,
        DateOnly EndsAt

    );
        
    
}
