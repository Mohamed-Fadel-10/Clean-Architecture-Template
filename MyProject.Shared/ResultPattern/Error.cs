using Domain.Enums;
using System.Text.Json.Serialization;

namespace Domain.ResultPattern
{

    public class ValidationError(string propertyName, string errorMessage)
    {
        public string PropertyName { get; } = propertyName;
        public string ErrorMessage { get; } = errorMessage;
    }

    public class Error(string message = "",
                IEnumerable<ValidationError> validationErrors = null,
                ErrorType type = ErrorType.General,
                Exception exception = null)
    {
        public string Message { get; } = message;
        public IEnumerable<ValidationError> ValidationErrors { get; } = validationErrors;
        public ErrorType Type { get; } = type;
        [JsonIgnore]
        public Exception Exception { get; } = exception;

        // Predefined errors
        public static readonly Error None = new(string.Empty, null, ErrorType.None);
        public static readonly Error NotFound = new("Resource not found", null, ErrorType.NotFound);
        public static readonly Error Unauthorized = new("Unauthorized access", null, ErrorType.Unauthorized);
        public static readonly Error Forbidden = new("Access forbidden", null, ErrorType.Forbidden);

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (Error)obj;
            return Message == other.Message && Type == other.Type;
        }

        public override int GetHashCode() => HashCode.Combine(Message, Type);

        public static Error FromException(Exception ex) =>
            new(ex.Message, null, ErrorType.Exception, ex);
    }
}
