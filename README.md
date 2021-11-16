# [RedWallet](https://redwalletdeployment.azurewebsites.net/) 
# A Bitcoin Wallet
### Dean Collier, Work In Progress, Winter 2021
 
## About
This is a free web application for create and using Bitcoin wallets.
ASP.NET Web MVC, N-tier architecture, C# was the main language used to create the models, logic, and MVC along with some SQL for database querying and Javascript. 

## NOTES
# Currently, functions that need to connect to the Bitcoin network are not complete. This currently includes:
1. Getting and returning the balance of wallets. - Currently every wallet in the app has 100BTC balance(hardcoded)
2. Constructing transactions and sedding to the network. - Currently just returning a hash of a string for the transaction hash.

# Functions working:
1. Creating real mainnet Bitcoin private keys and mnemonic seed.
2. Encrypting private keys with passphrase for storage.
4. Creating new Bitcoin public addresses based on Xpub
4. CRUD methods for all data models
5. MVC web app

## Technologies
1. NBitcoin 
2. Bootstrap
3. QRCoder
4. Entity Framework
5. .NET Framework with Owin for ASP.NET

## Table of Contents
1. General Project Layout
2. Database Entity Detail
3. Git Pull Documentation
 
### 1. General Project Layout
1. Data
2. Models
3. Services
4. WebMVC 
 
### 2. Database Entity Detail
- Wallet
  - Description: Contains information for wallets created
  - Id: primary key for wallet
  - UserId: foreign key of user
  - User: virtual user based on foreign key
  - EncryptedSecret: Bitcoin private key ecrypted with wallet passphrase, passphrase is not stored in database
  - Xpub: extended public key of wallet
  - XpubIteration: iterating number to produce new public addresses
 
- Send
  - Description: Contains details on transactions sent from wallets
  - Id: primary key for send
  - UserId: foreign key of user
  - User: virtual user based on foreign key
  - TransactionHash: Bitcoin transaction hash from the Bitcoin network
  - Created: date created
 
- Request
  - Description: Contains details of public addresses created for wallets
  - Id: primary key for send
  - UserId: foreign key of user
  - User: virtual user based on foreign key
  - RequestAddress: Bitcoin public address created
  - Created: date created
 
  ### 3. Git Pull Documentation
  Clone the repository to your local machine with this git command:
  ```
  git clone https://github.com/DeanCollier/RedWallet.git
  ```
  You did it!
 