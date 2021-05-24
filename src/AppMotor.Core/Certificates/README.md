# Certificates API

The certificates API exists to make it easier to use/manage TLS certificates. It's a thin wrapper around .NET's own `X509Certificate2` API.

## Certificates Mini Primer

Certificates are a complicated topic but the day-to-day use is not that complicated. This section give a quick introduction into (TLS) certificates.

First off, TLS certificates provide the following functions:

1. Confidentiality: the transmitted data is encrypted and can't be read by man-in-the-middle attackers
1. Integrity: the transmitted data can't be tampered with by man-in-the-middle attackers
1. Authentication: the sender can be sure to talk to the correct receiver

Functions 1 and 2 are provided by certificates themselves.

Function 3 is a little bit more complicated. The most common way is for a CA (certificate authority) to "sign" the certificate. This signature can be validate. Each operating system has a list of CAs that are trusted.

Certificates consist of a public key and a private key. The public key is known to everyone, the private key is only known to the server itself. A sender can encrypt data with a public key and only the owner of the private key can then decrypt the data.

Public keys (and thereby also the private keys) are created using a key algorithm. The most commonly used is RSA, but ECDSA is also gaining support. For compatibility reason, it's still recommended to create RSA keys.

For storing certificates on disk, there are basically three formats: pfx, pem, and der. Files in pfx format usually store public and private key in a single (password encrypted) file while files in pem format store those keys in separate files. Files in der format seem to be very uncommon.

## Loading a certificate

To load a certificate with this API, first create an instance of `TlsCertificateSource`. For example, if the certificate is on disk, simply use:

```c#
var source = TlsCertificateSource.FromFile("/path/to/certs/cert.pfx");
```

The certificate may or may not contain a private key. The certificates API does not care.

Next, to create an actual certificate from this source, simply pass it to one of the constructors of `TlsCertificate`:

```c#
var cert = new TlsCertificate(source);
```

If the certificate is password protected, also pass the password:

```c#
var cert = new TlsCertificate(source, "P@ssw0rd");
```

To use this certificate, pass it to any API that takes a `X509Certificate2` parameter.

## Creating a self-signed certificate

To create a self-signed certificate for `example.com` that's valid for 20 days, use:

```c#
var cert = TlsCertificate.CreateSelfSigned("example.com", TimeSpan.FromDays(20));
```

Or:

```c#
using var certificateRequest = new TlsCertificateRequest("example.com"));

var cert = certificateRequest.CreateSelfSignedCertificate(TimeSpan.FromDays(20));
```

## Exporting a certificate

To export a certificate, you need to decide whether to:

* export public key and private key - or just the public key
* export as pfx or pem

For example, to export the public key as pem into a byte array, use:

```c#
var exportedBytes = cert.ExportPublicKey().AsPem().ToBytes();
```

To export the public and the private key as pfx into a file, use:

```c#
cert.ExportPublicAndPrivateKey().AsPfx().ToFile("/path/to/export/cert.pfx");
```

*Note:* The private key must be exportable for this to work. This flag is set during the creation of the `TlsCertificate` instance.
