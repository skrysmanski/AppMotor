# Secrets API

*Note:* The secrets API was partially developed but ultimately discarded because it got too complicated to use (and it took way too long to develop it). This document exists to explain the idea behind the secrets API and to explain why it was discarded.

## The problem

Most applications need to work with secrets - passwords, TLS certificates, ...

Usually, one would use either `byte[]` or `string` to store these secrets. However, the disadvantage of using these types is that they can live in memory for a long time - even after they have been used for the last time. Instances of these types will only be deleted when the Garbage Collector decides to delete them - and this may only happen if memory runs out, which may not happen for a long time.

This becomes a problem when a developer creates a memory dump of such a process - thereby accidentally leaking secrets, even if they haven't been in use for some time.

Also, one would consider it to be good "security hygiene" to keep secrets in memory only as longer as necessary.

## The basic idea

The basic idea behind the secrets API was to **limit the lifetime of secrets**. To do this, there would be dedicated types for storing secrets (namely `SecretBytes` and `SecretString`). These types would implement `IDisposable` and when disposing them, the secrets would be removed from memory:

```c#
using (SecretBytes secret = ...)
{
    // Use secret here
}

// Here, the secret is no longer available
```

Internally, on dispose, the memory would simply be cleared. (The secrets would also be pinned to prevent the Garbage Collector from moving them around in memory thereby accidentally creating copies of the secrets.)

If this all sounds familiar, it's basically the same implementation as .NET's own `SecureString` - only also extended to bytes.

## Why was this API discarded?

While all of this moght sound like a good idea, the API was ultimately discard. The primary reason: it was **too complex to use**.

In the experiment we did (which took multiple weeks), simple APIs would get very complicated/complex to use when the secrets API was used. On the other hand, the benefit of using the secrets API was just too small to justify a more complex API. (Also, the implementation toke too long without seeing an actual benefit.)

It seems that even Microsoft found that this way is too complicated. The [documentation of `SecureString`](https://docs.microsoft.com/en-us/dotnet/api/system.security.securestring) contains this note:

> We don't recommend that you use the `SecureString` class for new development. For more information, see [SecureString shouldn't be used](https://github.com/dotnet/platform-compat/blob/master/docs/DE0001.md) on GitHub.

With the conclusion:

> The general approach of dealing with credentials is to avoid them and instead rely on other means to authenticate, such as certificates or Windows authentication.

The following sections will explain some of the problems we encountered while developing the secrets API.

### No support from .NET

At the time of writing (.NET 5), there was no support for (the concept of) such a secrets API in .NET itself. This becomes a problem at the "boundaries" - i.e. the various ways of how the secrets would enter or leave the process.

All of .NET's I/O classes (e.g. `File`, `Stream`, `HttpClient`) use various methods of buffering or even string building when used. All of these may leak (parts of) the secrets they read or write - especially if there is an exception (since none of the implementations clears their buffers when exceptions occur).

For most of these problems, we found ways to reduce the chance of leaking. But all of these ways relied on knowing exactly how the various methods are implemented. And relying on implementation details for security is very bad - especially since most of these implementation details could *not* be validated via automatic tests.

Also, this whole problem would get even more trickier when RPC/REST frameworks like ASP\.NET Core or gRPC.

### Ownership and lifetime problems

While the ownership and lifetime of a single secret may be clear, things get complicated if the secrets needs to be transformed within the API that uses it.

The primary example in our experiment was conversion between `SecretBytes` and `SecretString`. Consider the following example (constructor of the class `PemCertificateSource`):

```c#
public PemCertificateSource(SecretString pemEncodedCertificate, SecretString? separatePemEncodedPrivateKey)
{
    this._pemEncodedCertificate = pemEncodedCertificate.ToAsciiString();
    this._separatePemEncodedPrivateKey = separatePemEncodedPrivateKey;
}

public PemCertificateSource(SecretBytes pemEncodedCertificate, SecretBytes? separatePemEncodedPrivateKey)
{
    this._pemEncodedCertificate = pemEncodedCertificate;
    this._separatePemEncodedPrivateKey = separatePemEncodedPrivateKey?.ToAsciiString()
}
```

In the first constructor, the newly created instance owns `_pemEncodedCertificate` but not `_separatePemEncodedPrivateKey`. In  the second constructor, it's the other way around.

One idea to solve this problem was to always create copies of secrets. While this would solve *this* problem (and other problems like it), it's very easy to forget creating copies when implementing your own API (an example of why using the secrets API is complicated).

Also, either way, the class `PemCertificateSource` would need to implement `IDisposable` - which makes using it more complicated (think of the "ripple effect" of introducing `IDisposable`).

Another idea of how to solve this problem was to introduce a `SecretsScope` but this also did not pan out; see below for more details.

### SecretsScope

To solve the problems with the complex ownerships of secrets, one idea was to always store them encrypted in memory - with an overall scope limiting the lifetime of all secrets created with it:

```c#
SecretBytes mySecret;

using (var secretsScope = new SecretsScope())
{
    // Use secrets here
    mySecret = ...
}

// mySecret can't be used here anymore
```

The idea behind this was to encrypt all secrets with a key stored in the `SecretsScope` and when the `SecretsScope` was disposed, the key would simply be "thrown away" (thus secrets could not be decrypted anymore even if they still existed in memory).

The biggest problem with this approach is: The user of the APIs that use the secret APIs needs to know how long to keep the scope open.

This would either require experimentation (i.e. you only know at runtime if you closed the scope too early) or complicated re-encryption of secrets with owners of different lifetimes.

In the end, this approach was also deemed too complicated (for the little benefit it would bring).

## Conclusion

The effort of implementing and (safely) using the secrets API was too big for the little benefit it brought.

In future, we may revive this API if we find a simpler way to use and to implement it.
