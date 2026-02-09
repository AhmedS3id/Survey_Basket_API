using Microsoft.AspNetCore.Http.HttpResults;
using Survey_Basket_API.Contract.Questions;
using Survey_Basket_API.Entities;
using Survey_Basket_API.Persistence;
using System.Linq;

namespace Survey_Basket_API.Services
{
    public class QuestionService(AppDbContext context) : IQuestionServices
    {
        private readonly AppDbContext _context = context;
        public async Task<Result<QuestionResponse>> GetAsync(int PollId, int id, CancellationToken cancellationToken = default)
        {
            var question = await _context.Questions
               .Where(x => x.PollId == PollId && x.Id == id)
               .Include(x => x.Answers)
               .ProjectToType<QuestionResponse>()
               .AsNoTracking()
               .SingleOrDefaultAsync(cancellationToken);

            if (question == null)
                return Result.Failure<QuestionResponse>(PollsErrors.InvalidPolls);

            return Result.success(question);
        }
        public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int PollId, CancellationToken cancellationToken = default)
        {
            var IsPollExist = await _context.Polls.AnyAsync(x => x.Id == PollId, cancellationToken: cancellationToken);
            if (!IsPollExist)
                return Result.Failure<IEnumerable<QuestionResponse>>(PollsErrors.InvalidPolls);
            var question = await _context.Questions
                .Where(x => x.PollId == PollId)
                .Include(x => x.Answers)
                //.Select(q => new QuestionResponse(
                //    q.Id,
                //    q.Content,
                //    q.Answers.Select(a => new Contract.Answers.AnswerResponse(a.Id, a.Content))
                //    ))
                .ProjectToType<QuestionResponse>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return Result.success<IEnumerable<QuestionResponse>>(question);
      
                
        }

        public async Task<Result<QuestionResponse>> AddAsync(int PollId, QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var IsPollExist = await _context.Polls.AnyAsync(x => x.Id == PollId, cancellationToken: cancellationToken);
            if (!IsPollExist)
                return Result.Failure<QuestionResponse>(PollsErrors.InvalidPolls);

            var QuestionIsExist = await _context.Questions.AnyAsync(x => x.Content == request.Content, cancellationToken: cancellationToken);
            if (QuestionIsExist)
                return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent);

            var question = request.Adapt<Question>();
            question.PollId = PollId;

           // request.Answers.ForEach(answer => question.Answers.Add(new Answer { Content = answer }));

           await _context.Questions.AddAsync(question, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.success(question.Adapt<QuestionResponse>());

        }

        public async Task<Result> UpdateAsync(int PollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var questionIsExist = await _context.Questions.AnyAsync(x =>
            x.PollId == PollId &&
            x.Id != id &&
            x.Content == request.Content
            , cancellationToken);

            if (questionIsExist)
                return Result.Failure(QuestionErrors.DuplicatedQuestionContent);

            var question = await _context.Questions
                .Include(x=>x.Answers)
                .FirstOrDefaultAsync(x => x.PollId == PollId && x.Id == id, cancellationToken);

            if (question is null)
                return Result.Failure(QuestionErrors.QuestionNotFound);

            question.Content = request.Content;

            //current Answer
            var CurrentAnswer = question.Answers.Select(x=>x.Content).ToList();
            //new answer 
            var NewAnswer = request.Answers.Except(CurrentAnswer).ToList();

            NewAnswer.ForEach(answer => question.Answers.Add(new Answer { Content = answer }));

            foreach (var answer in question.Answers)
            {
                answer.IsActive= request.Answers.Contains(answer.Content);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.success();

        }

        public async Task<Result> ToggleStatusAsync(int PollId, int id, CancellationToken cancellationToken = default)
        {
            var question = await _context.Questions.FirstOrDefaultAsync(x => x.PollId == PollId && x.Id == id, cancellationToken);
            if (question is null)
                return Result.Failure(QuestionErrors.QuestionNotFound);
            question.IsActive = !question.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
            return Result.success();
        }

        
    }
}
