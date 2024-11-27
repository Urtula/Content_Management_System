## Description

This Content Management System (CMS) implements the logic for interacting with Users Categories and Contents along with 2 unique variants for each Content.

The application provides a RESTful API to interact with categories, contents, and variants. It also utilizes redis caching to improve performance and reduce database load.

## Technologies Used

- C# -> Language used to develop the API.
- ASP.NET Core -> Framework for building the API.
- Entity Framework Core -> ORM (Object-Relational Mapping) for interacting with the SQL database.
- SQL Server -> Database used for storing users, content, categories, and variants.
- Mapster -> Object mapping tool used to map entities to DTOs(See the entities below).
- Redis (Optional) -> Used for caching. Could also use other caching methods with modifications.
- Docker(Optional) -> Used to host the redis server. Optional and another method could be used with modifications.
- Swagger(Optional) - Used during testing and documenting the API.

## Prerequisites

To run the project, ensure you have the following installed:

- .NET SDK
- SQL Server
- Redis and Docker (optional) - For caching (if using Redis in your application).

## Setting Up the Project

After cloning the repository and installing the required dependencies and other software make sure to make sure to setup the database and apply the migrations. 
To quickly install all dependencies in Visual Studio run 'dotnet restore' command in the project directory within powershell. You can use the existing migrations within 
the project and only use 'dotnet ef database update' within the startup project directory to update your database.

## Configuring Project
    You also need to configure the `appsettings.json` file to connect to your SQL Server instance and configure your Redis caching if you intend to use Redis.

    Example configuration for `appsettings.json`:

    {
        "Logging": {
            "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
            }
        },
        "AllowedHosts": "*",
        "ConnectionStrings": {
            "DefaultConnection": "Server=localhost;Database=CMS;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True",
            "Redis": "localhost:5002"
        }
    }

    For the Redis Key the value should be the adress of your Redis server. If you plan to use SQL Server authentication make sure to update the ConnectionString field accordingly

## Endpoints
The API allows endpoints to modify Categories, Contents and Users.
### Categories

1. **Get all categories**: 
    - **Endpoint**: `GET /categories/all`
    - **Description**: Retrieves all categories.
    - **Response**: A list of all categories.

2. **Get category by name**: 
    - **Endpoint**: `GET /categories/{categoryName}`
    - **Description**: Retrieves a category by its name.
    - **Response**: The category with the specified name.

3. **Delete category**: 
    - **Endpoint**: `DELETE /categories/{categoryName}`
    - **Description**: Deletes a category by its name.
    - **Response**: No content (204).

4. **Create category**: 
    - **Endpoint**: `POST /api/categories`
    - **Description**: Creates a new category.
    - **Request Body**: Content data including variants.
        {
            "CategoryName": "Example Category",
            "CategoryDesc": "Example Desc"
        }
    - **Response**: Returns the newly created category.

### Contents

1. **Get contents by category**: 
    - **Endpoint**: `GET /contents/category/{categoryName}`
    - **Description**: Retrieves all contents belonging to a specified category.
    - **Response**: A list of contents in the specified category.

2. **Add new content**:
    - **Endpoint**: `POST /contents`
    - **Description**: Adds a new content item.
    - **Request Body**: Content data including variants.
    - **Response**: The created content.
    
3. **Get Content by ID**:
    - **Endpoint**: `GET /api/Content/{id}`
    - **Description**: Fetches content based on the specified ID.
    - **Request Body**: id parameter.
    - **Response**: The requested content.
    
4. **Delete Content by ID**:
    - **Endpoint**: `DELETE /api/Content/{id}`
    - **Description**: Deletes content based on the specified ID.
    - **Request Body**: id parameter.
    - **Response**: None.

5. **Get Contents by Category**:
    - **Endpoint**: `GET /api/Content/ByCategory/{categoryName}`
    - **Description**: Fetches all the content within category.
    - **Request Body**: name of Category.
    - **Response**: All of the requested content.

### Users
1. **Get user by mail**: 
    - **Endpoint**: `GET /api/User/ByEmail/{email}`
    - **Description**: Retrieves user by specified mail.
    - **Request Body**: User's email.
    - **Response**: The requested user.

2. **Get All users**:
    - **Endpoint**: `GET /api/User`
    - **Description**: Gets all users.
    - **Response**: All users.
    
3. **Create User**:
    - **Endpoint**: `POST /api/User`
    - **Description**: Creates a user.
    - **Request Body**: mail and full name of user.
    - **Response**: None.

4. **Delete User**:
    - **Endpoint**: `DELETE /api/User`
    - **Description**: Deletes User.
    - **Request Body**: ID of user.
    - **Response**: None.

5. **Add Categories to User**:
    - **Endpoint**: `POST /api/User/{userId}/categories`
    - **Description**: Adds the specified category ids to users categories.
    - **Request Body**: Userid and id of categories to be added.
    - **Response**: None.

6. **Get Categories Associated with User**:
- **Endpoint**: `GET /api/User/{userId}/Categories`
- **Description**: Get the specified categories associated with user.
- **Request Body**: Userid.
- **Response**: Categories of user.