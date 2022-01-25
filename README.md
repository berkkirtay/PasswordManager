I developed a simple password manager that stores encrypted credential data in a database via meditor pattern.

## Procedure
- The client encrypts passwords and sends it to the server. Whenever we try to get our credentials back, the server will check for authorization and will serve the data if it is authorized.

- When we attempt to retrieve our credentials, the client will decrypt them with the same public-private pair and will serve to the user.

- Each client can generate its own token and use it to create unique password containers for its user. So, a client can only reach its own container on the server. 

- This project can be useful where we run our own servers and want to use a password container server to store our passwords. 

- The server can be used either as a windows service or as a command line program. 

- I used mysql database and you may need to change the connection parameters in CredentialsDB.cs file for your own database.

## Usage

- After we start the server, we can either use the client or use Postman to test the server.

- You can change the encryption algorithm on client or can use another client.

- The server has 5 http request handlers:

   - /addcredential
   - /getallcredentials
   - /deletecredential
   - /updatecredential
   - /reset
           
   With the given handlers, we can import a container that consists several passwords or a signle credential. We can also fetch or reset or update password data that corresponds to our user token on the server. 
   
- Server saves all the data fields in a database, I hardcoded schemas in CredentialsDB.cs file, so even if the tables don't exist, server will generate them.
- Additionally, server saves the credential data in a json file, so you may want to check it out in Password Manager Server folder. 
- If you use Postman api, your credentials won't be encrypted since server only stores the data it receives.

### Example usages:
- With Postman api: 
  
![Enc1](https://raw.githubusercontent.com/berkkirtay/PasswordManager/main/examples/Capture1.PNG)

- With Password Manager Client: 
  
![Enc1](https://raw.githubusercontent.com/berkkirtay/PasswordManager/main/examples/Capture2.PNG)

- Encrypted credential data in database:

![Enc1](https://raw.githubusercontent.com/berkkirtay/PasswordManager/main/examples/Capture3.PNG)