# Test Certificates

This folder contains test certificates. These were generated as follows.

## `cert.pem` and `key.pem`

    openssl req -x509 -newkey rsa:4096 -days 365 -nodes -subj "/C=US/ST=Oregon/L=Portland/O=Company Name/OU=Org/CN=www.example.com" -sha256 -keyout key.pem -out cert.pem

## `key_encrypted.pem`

    openssl rsa -aes256 -in key.pem -out key_encrypted.pem

Use `P@ssw0rd` as password.

## `key_encrypted_pkcs1.pem`

    openssl rsa -aes256 -in key.pem -out key_encrypted_pkcs1.pem

Use `P@ssw0rd` as password.

## `cert.pfx`

    openssl pkcs12 -inkey key.pem -in cert.pem -export -out cert.pfx

When asked for a password, just hit Enter (without entering any).

## `cert_encrypted.pfx`

    openssl pkcs12 -inkey key.pem -in cert.pem -export -out cert_encrypted.pfx

Use `P@ssw0rd` as password.
