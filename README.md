## Spaceship Weather API
This project is a RESTful API that reads weather information from a web service, stores it in a SQL Server database, and sends the information to client. The program is designed to be deployed on a spaceship and sent to space, where it will operate for at least 20 years without any maintenance or updates.

### Getting Started
To get started with this project, follow these steps:

- Clone the repository to your local machine.
- Use .NET 8 as the runtime and framework for building the API.
- Update the appsettings.json file with the appropriate connection string for your SQL Server database.
- Run the application.
The API will start listening on https://localhost:7002. This will open up Swagger in your browser, from where you can send your request. Alternatively, you can use a tool like Postman to send requests to the API.

### API Endpoints
- GET /weather: Returns the current weather information.
- 
### Technologies
- .NET 8: The runtime and framework for building the API.
- ASP.NET Core WebAPI(Controller)
- Dapper
- Microsoft SQL Server
