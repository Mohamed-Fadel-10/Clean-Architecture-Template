using Domain.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Enums.ResultPattern;

namespace Domain.ResultPattern
{
    /// <summary>
    /// Represents the result of an operation with a strongly typed value on success.
    /// </summary>
    /// <typeparam name="T">Type of the success value</typeparam>
    public class Result<T>
    {
        // Private constructors to enforce factory methods
        private Result(T data, Error error, ResultStatus status, string? msg = null)
        {
            Data = data;
            Error = error;
            Status = status;
            Message = msg;
            // Validate state
            if (data != null && error.Equals(Error.None) && status != ResultStatus.Success)
            {
                throw new InvalidOperationException("Cannot have both data and error in a result");
            }
        }

        // Core properties
        public bool IsSuccess => Status == ResultStatus.Success && Error.Equals(Error.None);
        [JsonIgnore]
        public bool IsFailure => !IsSuccess;
        public T Data { get; }
        public Error Error { get; }
        public ResultStatus Status { get; }
        public string StatusCode => Status.ToString();
        public bool HasValue => Data != null;
        public string? Message { get; }

        // Factory methods for success results
        public static Result<T> Success(T data) =>
            new(data, Error.None, ResultStatus.Success);

        public static Result<T> Success(T data , string message) =>
         new(data, Error.None, ResultStatus.Success, message);
        public static Result<T> Warning(string message) =>
         new(default, new Error(message), ResultStatus.Conflict);

        public static Result<T> Success() =>
            new(default, Error.None, ResultStatus.Success);
        // Factory methods for various failure states
        public static Result<T> Failure(Error error) =>
            new(default, error, ResultStatus.Error);

        public static Result<T> Failure(string message) =>
            new(default, new Error(message), ResultStatus.Error);

        public static Result<T> NotFound(string message = null) =>
            new(default, message != null ? new Error(message) : Error.NotFound, ResultStatus.NotFound);

        public static Result<T> Unauthorized(string message = null) =>
            new(default, message != null ? new Error(message) : Error.Unauthorized, ResultStatus.Unauthorized);

        public static Result<T> Forbidden(string forbiddenAction = null) =>
            new(default,
                          forbiddenAction != null ? new Error($"{forbiddenAction} is forbidden") : Error.Forbidden,
                          ResultStatus.Forbidden);

        public static Result<T> Conflict(string message = null) =>
            new(default,
                          message != null ? new Error(message) : new Error("A conflict occurred"),
                          ResultStatus.Conflict);

        public static Result<T> BadRequest(string message) =>
            new(default, new Error(message), ResultStatus.BadRequest);

        public static Result<T> ValidationError(IEnumerable<ValidationError> errors) =>
            new(default, new Error("Validation failed", errors), ResultStatus.BadRequest);

        // Extension methods
        public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
        {
            if (IsFailure)
                return Result<TNew>.Failure(Error);

            return Data != null ? Result<TNew>.Success(mapper(Data)) : Result<TNew>.Failure("Cannot map null result");
        }

        public async Task<Result<TNew>> MapAsync<TNew>(Func<T, Task<TNew>> mapper)
        {
            if (IsFailure)
                return Result<TNew>.Failure(Error);

            if (Data == null)
                return Result<TNew>.Failure("Cannot map null result");

            var mappedData = await mapper(Data);
            return Result<TNew>.Success(mappedData);
        }

        public Result<T> OnSuccess(Action<T> action)
        {
            if (IsSuccess && Data != null)
                action(Data);

            return this;
        }

        public Result<T> OnFailure(Action<Error> action)
        {
            if (IsFailure)
                action(Error);

            return this;
        }

        public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
        {
            return IsSuccess && Data != null ? onSuccess(Data) : onFailure(Error);
        }

        public async Task<TResult> MatchAsync<TResult>(
            Func<T, Task<TResult>> onSuccess,
            Func<Error, Task<TResult>> onFailure)
        {
            return IsSuccess && Data != null ? await onSuccess(Data) : await onFailure(Error);
        }

        // Ensure
        public Result<T> Ensure(Func<T, bool> predicate, string errorMessage)
        {
            if (IsFailure)
                return this;

            if (Data == null || !predicate(Data))
                return Failure(errorMessage);

            return this;
        }

        // Serialization
        public string Serialize() => JsonSerializer.Serialize(this);

        public static Result<T> Deserialize(string json) =>
            JsonSerializer.Deserialize<Result<T>>(json) ??
            Failure("Failed to deserialize result");

        // Implicit operators
        public static implicit operator T(Result<T> result) => result.Data;
        public static implicit operator Result<T>(T data) => Success(data);

        // For easier error handling in pattern matching
        public void Deconstruct(out bool isSuccess, out T data, out Error error)
        {
            isSuccess = IsSuccess;
            data = Data;
            error = Error;
        }
    }
    // Non-generic Result for operations that don't return a value
    public class Result
    {
        private Result() { }

        public static Result<T> Success<T>(T value) => Result<T>.Success(value);
        public static Result<Nothing> Success() => Result<Nothing>.Success(new Nothing());

        public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);
        public static Result<T> Failure<T>(string message) => Result<T>.Failure(message);
        public static Result<Nothing> Failure(string message) => Result<Nothing>.Failure(message);

        // Additional helper methods for common scenarios
        public static Result<Nothing> NotFound(string message = "Resource not found") =>
            Result<Nothing>.NotFound(message);

        public static Result<Nothing> Unauthorized(string message = "Unauthorized access") =>
            Result<Nothing>.Unauthorized(message);
    }

    // Represents a void result
    public sealed class Nothing
    {
        public static readonly Nothing Instance = new();
    }
  
}
