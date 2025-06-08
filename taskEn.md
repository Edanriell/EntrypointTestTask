# Simple CRUD Site for Clients and Their Orders

## Models (Entities):

### Client with fields:
- Id
- Name
- Email
- Birthdate (DateTime)
- Gender (Enum with values Male, Female)
- Orders - collection of the client's orders

### Product with fields:
- Id
- Code
- Title
- Price (decimal)

### Order with fields:
- Id
- ClientId
- Client - reference to the client
- ProductId
- Product - reference to the product
- Quantity
- Status (Enum with values Created, Paid, Delivered)

## Requirements:

1. A list of clients with the ability to delete a client (along with all their orders). Among other columns in the list, there must be columns "Number of Orders" and "Average Order Amount".

   The data for the list should be selected with a single LINQ query using projection.

2. Forms for creating and editing a client. Gender should be selectable using radio buttons or a dropdown (`<select />`).

3. A list of products with the ability to delete a product.

4. Forms for creating and editing a product.

5. A list of orders with the ability to delete an order. Among other columns in the list, there must be columns "Product Name", "Quantity", "Order Amount" (quantity multiplied by the price of the product), "Order Status".

   The data for the list should be selected with a single LINQ query using projection.

6. Forms for creating and editing an order. Client and product should be selectable using dropdowns (`<select />`).

### Notes:

- All forms must have validation (e.g., if the field is email, then an email validator; if the field is required, then a required validator, etc.). No exceptions should occur if fields are not filled in or filled in incorrectly. In other words, everything should be done carefully and without bugs.
- Client.Gender and Order.Status must be implemented using Enum.
- No front-end frameworks are required.