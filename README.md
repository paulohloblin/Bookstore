# Online Bookstore API

The Online Bookstore API is a RESTful web service built with ASP.NET Core and Entity Framework Core that allows users to perform CRUD operations on books, authors, and genres. Users can also search for books by title, author, or genre.

## Prerequisites

Before you can run the application, ensure that you have the following prerequisites installed on your system:

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Visual Studio Code](https://code.visualstudio.com/) or any code editor of your choice
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

## Installation

1. Clone this repository to your local machine:

   ```shell
   git clone https://github.com/yourusername/online-bookstore-api.git
   ```

2. Change to the project directory:

   ```shell
   cd online-bookstore-api
   ```

3. Restore the required NuGet packages:

   ```shell
   dotnet restore
   ```

4. Create the database (SQLite is used in this example):

   ```shell
   dotnet ef database update
   ```

## Running the Application

To run the Online Bookstore API, use the following command:

```shell
dotnet run
```

The API will be hosted at `http://localhost:5000` by default. You can access it using a web browser or a tool like [Postman](https://www.postman.com/) for testing the endpoints.

## API Endpoints

- GET: `/api/books` - Get a list of all books.
- GET: `/api/books/{id}` - Get a specific book by ID.
- POST: `/api/books` - Add a new book.
- PUT: `/api/books/{id}` - Update an existing book.
- DELETE: `/api/books/{id}` - Delete a book by ID.
- GET: `/api/books/search` - Search for books by title, author, or genre.

## Documentation

The API includes Swagger documentation. To access it, navigate to:

`http://localhost:5000/swagger`

## Unit Tests

To run the unit tests for the API, use the following command:

```shell
dotnet test
```

## Contributing

Feel free to contribute to this project by opening issues or submitting pull requests. Your contributions are highly appreciated.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.