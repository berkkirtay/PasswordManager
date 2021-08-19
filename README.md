I developed a simple password manager that holds encrypted credential data and serves it on request. 

## Procedure
- The client encrypts passwords and sends it to the server. Whenever we call our credentials back, the server will check for authorization and will serve the data if it is authorized.

- When we retrieve our credentials, the client will decrypt them with the same public-private pair and will serve to the user.

- This project can be useful when we run our own servers and want to use a container to deposit our passwords. 

- Password Manager Server can be used either as a windows service or as a command line program. 

- Authorization can be improved with using certificates and to have a stronger security, a couple of layers of encrytion can be added.
