
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Survey_Basket_API.Helpers;
using Survey_Basket_API.Persistence;

namespace Survey_Basket_API.Services
{
    public class NotificationService(AppDbContext context,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        IEmailSender emailSender) : INotificationService
    {
        private readonly AppDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IEmailSender _emailSender = emailSender;

        public async Task SendNewPollsNotification(int? pollId = null)
        {
         IEnumerable<Poll> polls = [];
            if(pollId.HasValue)
            {
                var poll = await _context.Polls.FirstOrDefaultAsync(x => x.Id == pollId && x.IsPublished);
                polls = [poll!];
            }
            else
            {
                polls = await _context.Polls
                     .Where(x => x.IsPublished && x.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                     .AsNoTracking()
                     .ToListAsync();   
            }
            var users = await _userManager.Users.ToListAsync();

            var Origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            foreach (var poll in polls)
            {
                foreach (var user in users)
                {
                    var PlaceHolders = new Dictionary<string, string>
                    {
                        {"{name}",user.FirsName },
                        {"{pollTill}",poll.Title },
                        {"{endDate}",poll.EndsAt.ToString() },
                        {"{url}",$"{Origin}/polls/start/{poll.Id}" }
                    };
                    var generateEmailBody = EmailBodyBuilder.GenerateEmailBody("PollNotification", PlaceHolders);
                    var SendEmail = _emailSender.SendEmailAsync(user.Email!, $"Survey Basket : New Poll-{poll.Id}", generateEmailBody);
                }
            }
        }
    }
}
