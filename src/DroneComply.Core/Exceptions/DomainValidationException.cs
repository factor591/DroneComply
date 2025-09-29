using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DroneComply.Core.Exceptions;

public sealed class DomainValidationException : Exception
{
    private static readonly IReadOnlyDictionary<string, string[]> EmptyErrors =
        new ReadOnlyDictionary<string, string[]>(new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase));

    public DomainValidationException(string message)
        : this(message, null)
    {
    }

    public DomainValidationException(string message, IDictionary<string, string[]>? errors)
        : base(message)
    {
        Errors = errors is null
            ? EmptyErrors
            : new ReadOnlyDictionary<string, string[]>(new Dictionary<string, string[]>(errors, StringComparer.OrdinalIgnoreCase));
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public override string ToString()
    {
        if (Errors.Count == 0)
        {
            return base.ToString();
        }

        var builder = new StringBuilder(base.ToString());
        builder.AppendLine();

        foreach (var error in Errors)
        {
            builder.Append(error.Key);
            builder.Append(':');
            builder.Append(' ');
            builder.AppendLine(string.Join("; ", error.Value));
        }

        return builder.ToString();
    }
}
