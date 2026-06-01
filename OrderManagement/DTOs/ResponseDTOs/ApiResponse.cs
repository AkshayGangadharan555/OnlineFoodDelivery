namespace Orders.DTOs.ResponseDTOs
{
    /// <summary>
    /// Standard API response wrapper for all endpoints.
    /// Provides consistent response structure including status code, message, and data.
    /// </summary>
    /// <typeparam name="T">The type of data being returned in the response</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// HTTP status code indicating the result of the operation
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Message describing the result of the operation
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The actual data returned by the operation
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the ApiResponse class.
        /// </summary>
        /// <param name="status">HTTP status code</param>
        /// <param name="message">Response message</param>
        /// <param name="data">Response data</param>
        public ApiResponse(int status, string message, T data)
        {
            Status = status;
            Message = message;
            Data = data;
        }
    }
}
