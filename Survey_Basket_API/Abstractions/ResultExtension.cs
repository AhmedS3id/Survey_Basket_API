namespace Survey_Basket_API.Abstractions
{
    public static class ResultExtension
    {
        public static ObjectResult ToProblem(this Result result, int statusCode)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("cannot convert from success result to problem");
            var Problem = Results.Problem(statusCode: statusCode);
            var ProblemDetail = Problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(Problem) as ProblemDetails;

            ProblemDetail!.Extensions = new Dictionary<string, object?>
            {

                {
                    "errors",new []{result.Error }
                }
            };
            
            return new ObjectResult(ProblemDetail);
        }
    }
}
