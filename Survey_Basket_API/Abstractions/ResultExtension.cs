namespace Survey_Basket_API.Abstractions
{
    public static class ResultExtension
    {
        public static ObjectResult ToProblem(this Result result)
        {
            if (result.IsSuccess)
                throw new InvalidOperationException("Can not convert from success result to problem");
            var Problem = Results.Problem(statusCode: result.Error.StatusCode);
            var ProblemDetail = Problem.GetType().GetProperty(nameof(ProblemDetails))!.GetValue(Problem) as ProblemDetails;

            ProblemDetail!.Extensions = new Dictionary<string, object?>
            {

                {
                    "errors",new []{
                     result.Error.Code
                    ,result.Error.Description
                    }
                }
            };
            
            return new ObjectResult(ProblemDetail);
        }
    }
}
