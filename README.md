I implemented a simple password manager that stores encrypted credential data in a database via meditor and command patterns.

## Procedure
- The client encrypts passwords and sends it to the server. Whenever we want to retrieve our credentials back, the server will check for authorization and will serve the data if it is authorized.

- When we attempt to retrieve our credentials, the client will decrypt them with the same public-private pair and will serve to the user. Client can use the same key pair or create a new one if it's required.

- Each client can generate its own token and use it to create unique password containers for its user. So, a client can only reach its own container on the server. 

- This project can be useful where we run our own servers and want to use a password container server to store our passwords. 

- The server can be used either as a windows service or as a command line program. 

- I used mysql database and you may need to change the connection parameters in CredentialsDB.cs file for your own database.

## Usage

- After we start the server, we can either use the client or use a api tester like postman or curl.

- You can also change the encryption algorithm on client or can use another client.

- The server has five endpoint:
   - /addcredential
   - /getallcredentials
   - /deletecredential
   - /updatecredential
   - /reset
           
   With the given handlers, we can send or fetch user credentials. We can also reset or update password data that corresponds to our user token on the server. 
   
- Server saves all the data fields in a database, I hardcoded table creation code in CredentialsDB.cs file, so even if the tables don't exist, server will generate them.
- Additionally, client can save the credential data in a json file, so you may want to check it out in examples folder.
- If you use only a api tester like curl, your credentials won't be encrypted since server only stores the data it receives.

### Example usages:
- With Curl (no encryption on credentials): 


![Enc1](https://raw.githubusercontent.com/berkkirtay/PasswordManager/main/examples/Capture4.PNG)

- With Postman (no encryption on credentials): 


![Enc1](https://raw.githubusercontent.com/berkkirtay/PasswordManager/main/examples/Capture1.PNG)

- With Password Manager Client: 


![Enc1](https://raw.githubusercontent.com/berkkirtay/PasswordManager/main/examples/Capture2.PNG)

- Encrypted credential data in database:


![Enc1](https://raw.githubusercontent.com/berkkirtay/PasswordManager/main/examples/Capture3.PNG)
