// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

namespace AppMotor.Core.Exceptions;

/// <summary>
/// Thrown on the attempt to modify a read-only collection.
/// </summary>
/// <remarks>
/// It's not very intuitive whether modifying a read-only collection should result
/// in a <see cref="InvalidOperationException"/> or a <see cref="NotSupportedException"/>.
/// The .NET framework itself uses <see cref="NotSupportedException"/> while I personally find
/// <see cref="InvalidOperationException"/> more fitting. This why this exception
/// exists: so that developer don't need to care.
///
/// <para>Note: The base exception is <see cref="NotSupportedException"/> so that this
/// exception can honor the contract of <see cref="ICollection{T}.Add"/> and the like
/// which require a <see cref="NotSupportedException"/> in case of read-only collections.</para>
/// </remarks>
public class CollectionIsReadOnlyException : NotSupportedException, ICollectionIsReadOnlyException
{
    /// <summary>
    /// The message used as text for this exception - and its "cousins" <see cref="CollectionIsReadOnlyArgumentException"/>
    /// and <see cref="CollectionIsReadOnlyValueException"/>.
    /// </summary>
    public const string DEFAULT_MESSAGE = "This collection is read-only.";

    /// <summary>
    /// Constructor.
    /// </summary>
    public CollectionIsReadOnlyException() : base(DEFAULT_MESSAGE)
    {
    }
}