# Test Certificates

This folder contains test certificates. These were generated as follows.

## `cert.pem` and `key.pem`

    openssl req -x509 -newkey rsa:4096 -days 365 -nodes -subj "/C=US/ST=Oregon/L=Portland/O=Company Name/OU=Org/CN=www.example.com" -sha256 -keyout key.pem -out cert.pem

## `cert.pfx`

    openssl pkcs12 -inkey key.pem -in cert.pem -export -out cert.pfx
