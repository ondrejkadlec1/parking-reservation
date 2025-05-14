namespace ParkingReservation.Services.Results
{
    public record ServiceCallResult<T> : ServiceCallResult
    {
        public T? Object { get; set; }
    }

    public record ServiceCallResult
    {
        public bool Success { get; set; } = false;
        public Errors? ErrorCode { get; set; }
        public string? Message { get; set; }
    }
}
