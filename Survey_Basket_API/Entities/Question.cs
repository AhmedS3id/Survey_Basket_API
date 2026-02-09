namespace Survey_Basket_API.Entities
{
    public sealed class Question:AuditTableEntity
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int PollId { get; set; }
        public bool IsActive { get; set; } = true;

        public Poll Poll { get; set; } = default!;
        public ICollection<Answer> Answers { get; set; } = [];

    }
}
