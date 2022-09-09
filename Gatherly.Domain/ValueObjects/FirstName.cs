using Gatherly.Domain.Primitives;
using Gatherly.Domain.Shared;

namespace Gatherly.Domain.ValueObjects;

public sealed class FirstName : ValueObject
{
    public const int MaxLength = 50;

    public string Value { get; }

    private FirstName(string value)
    {
        //if (value.Length > MaxLength)
        //{
        //    throw new ArgumentException();
        //}
        Value = value;
    }

    public static Result<FirstName> Create(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result.Failure<FirstName>(new Error(
                "FirstName.empty"
                , "FirstName is empty"));
        }

        if (firstName.Length > MaxLength)
        {
            return Result.Failure<FirstName>(new Error(
                "FirstName.TooLong",
                "FirstName is too long."));
        }

        return new FirstName(firstName);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
