// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.Core.Exceptions;

/// <summary>
/// <see cref="ValueException"/> version of <see cref="CollectionIsReadOnlyException"/>.
/// </summary>
public class CollectionIsReadOnlyValueException : ValueException, ICollectionIsReadOnlyException
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="valueName">The name of the value this exception applies to.</param>
    public CollectionIsReadOnlyValueException(string valueName)
        : base(CollectionIsReadOnlyException.DEFAULT_MESSAGE, valueName: valueName)
    {
    }
}
