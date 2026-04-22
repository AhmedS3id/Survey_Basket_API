namespace Survey_Basket_API.Services
{
    public interface INotificationService
    {
        Task SendNewPollsNotification(int? pollId = null);
    }
}
