# AuctionApi

AuctionApi is a server-side web application for online auctions implemented on ASP.NET Core. The project includes functionality for managing lots, categories, bids, as well as authentication and authorization system using JWT tokens.

## Project Overview

This project is an auction API built using C# ASP.NET Core 6, with the following technologies and architectural patterns:

- **CQRS**: separation of command and request responsibilities.
- **MediatR**: for processing commands and requests.
- **Entity Framework Core**: for interacting with databases.
- **FluentValidation**: for query validation.
- **IdentityServer4**: for authentication and authorization.
- **JWT (JSON Web Tokens)**: for securing API endpoints.
- **Swagger**: for API documentation.
- **Principles of Clean Architecture**.
- **Angular**: for frontend.

The API allows users to register, log in, create and manage auction lots, bid and process payments. The frontend is developed using Angular, integrating various libraries such as RxJS and others.

### Features

### User Management

- **Register**: users can register by providing their email address and password.
- **Login**: users can log in and receive a JWT token.
- **Roles**: Users can be automatically assigned roles during registration (e.g. administrator role).
- **Authentication**: performed by JWT, with applications ensuring that users have the necessary permissions.

### Auction Management

- **Lots**: users can create auction lots, set the starting price and determine the end time.
- **Bids**: users can bid on available lots.
- **Categories**: lots can be categorized and users can filter them by category.

## Installation and startup

1. Clone the repository:
    ```bash
    git clone https://github.com/Eloqmens/Auction-Client
    cd AuctionApi
    ```

2. Configure the database connection string in ``appsettings.json``:
    ```json
    "ConnectionStrings": {
       "SQLserverAuction": "Server=(localdb)\\mssqllocaldb;Database=AuctionDb;Trusted_Connection=True;",
       "SQLserverIdentity": "Server=(localdb)\\mssqllocaldb;Database=AuctionIdentity;Trusted_Connection=True;"
    }
    ```

3. On startup to be created Admin which will be written to the database:
- adminEmail = "admin@example.com";
- adminPassword = "Admin@1234";


4. Run the application:
    ```bash
    dotnet run
    ```

5. The API will be available at ``https://localhost:7130/api``.

## Swagger

Swagger documentation for your API is available at `https://localhost:7130/swagger`. Here you can view all available endpoints and test them.

## Frontend

The frontend for this project is developed using Angular. The source code and instructions for installing the frontend part are in a separate repository:

- **Auction-Client Repository**: [https://github.com/Eloqmens/Auction-Client](https://github.com/Eloqmens/Auction-Client)

## Development plans
### Payment system

Future plans are to integrate a payment system that will allow users to pay for lots won in an auction. Basic steps:

- **Select a payment provider**: Selecting a payment processor such as Stripe, PayPal or other to process transactions.
- **Integration with API**: Adding endpoints for payment processing and transaction success verification.
- **UI/UX**: Updating the user interface to support the payment process.
- **Transaction Management**: Development of a system to log and track transactions.

This functionality will provide the full cycle of handling auction items, from creating and finalizing the auction, to payment and confirmation of purchase.
