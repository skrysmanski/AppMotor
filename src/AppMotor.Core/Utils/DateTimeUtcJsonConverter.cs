// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppMotor.Core.Utils;

/// <summary>
/// JSON converter for <see cref="DateTimeUtc"/>.
/// </summary>
public class DateTimeUtcJsonConverter : JsonConverter<DateTimeUtc>
{
    /// <inheritdoc />
    public override DateTimeUtc Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new(reader.GetDateTimeOffset());
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTimeUtc value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToDateTime());
    }
}
