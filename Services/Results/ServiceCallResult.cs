namespace DocumentationApp.Services.Results
{
    public record ServiceCallResult<T>
    {
        public T? Object { get; set; }
        public bool Success { get; set; }
        public Errors? ErrorCode { get; set; }
        public string? Message { get; set; }
    }

    public record ServiceCallResult
    {
        public bool Success { get; set; }
        public Errors? ErrorCode { get; set; }
        public string? Message { get; set; }
    }

    public enum Errors
    {
        Success,
        NotFound,
        Unauthorized,
        InvaData,
        NameAlreadyUsed,
        Other
    }
}
