# E-Commerce API

## Overview
This project is an e-commerce RESTful API built using ASP.NET Core (C#) and MongoDB. It provides a backend infrastructure for managing users, items, shopping carts, completed orders, and categories. The API includes authentication using JSON Web Tokens (JWT) for user registration, login, and secure access to user-specific resources.

## Features

### User Management
- User registration and login with JWT authentication.
- User profile management, including updating user details and retrieving user information.

### Item Management
- CRUD operations for managing items, including adding new items, updating item details, and retrieving item information.
- Users can add items to their shopping carts and remove them.

### Shopping Cart and Completed Orders
- Each user has a dedicated shopping cart and completed orders section.
- Users can view their shopping cart and completed orders.

### Category Management
- Items can be categorized for better organization.

## Project Structure

### Models
- `Users`: Represents user information, including address and references to shopping carts and completed orders.
- `Items`: Represents product details, including name, description, price, inventory, and more.
- `ShoppingCarts`: Stores user-specific shopping cart information, including cart items.
- `CompletedOrders`: Stores user-specific completed orders information, including bought items.

### Controllers
- `UsersController`: Handles user-related operations, including registration, login, profile management, and address updates.
- `ItemsController`: Manages item-related operations, such as adding, updating, and deleting items, as well as handling shopping cart operations.

### Services
- `JwtService`: Generates JWT tokens for user authentication.

### Data
- `ApiContext`: Manages the MongoDB connection and provides access to different collections.

## How to Use
1. Clone the repository: `git clone <repository-url>`
2. Set up your MongoDB connection in `appsettings.json`.
3. Run the API locally.

## Accessing Swagger UI (OpenAPI Documentation)

To explore the API endpoints and test them interactively, Swagger UI is provided. Follow these steps to access Swagger UI:

1. Ensure your application is running locally.

2. Open a web browser.

3. Navigate to the following URL:   http://localhost:5000/swagger/index.html

4. Once the page loads, you'll see the Swagger UI interface, which lists all available endpoints along with descriptions and parameters. You can use Swagger UI to make test requests to your API directly from the browser.

5. Explore the different endpoints, parameters, and responses to understand how your API works.

## Dependencies
- ASP.NET Core
- MongoDB
- JWT for authentication


Happy coding!
