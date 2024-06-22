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

# Простой CRUD сайт для ввода/вывода клиентов и их заказов

## Модели (Entities):

### Клиент (Client) с полями:
- Id
- Name
- Email
- Birthdate (DateTime)
- Gender (Enum со значениями Male, Female)
- Orders - коллекция заказов этого клиента

### Товар (Product) с полями:
- Id
- Code
- Title
- Price (decimal)

### Заказ (Order) с полями:
- Id
- ClientId
- Client - ссылка на клиента
- ProductId
- Product - ссылка на продукт
- Quantity
- Status (Enum со значениями Created, Paid, Delivered)

## Требования:

1. Список клиентов с возможностью удалить клиента (вместе со всеми его заказами). Среди прочих колонок в списке обязательно должны быть колонки "Количество заказов" и "Средняя сумма заказа".

   Данные для списка должны выбираться только одним LINQ запросом при помощи проекции.

2. Формы создания клиента и редактирования клиента. Пол (Gender) должен выбираться при помощи radio button или dropdown (`<select />`).

3. Список товаров с возможностью удалить товар.

4. Формы создания товара и редактирования товара.

5. Список заказов с возможностью удалить заказ. Среди прочих колонок в списке должны быть колонки "Название товара", "Количество" (Quantity), "Сумма заказа" (количество умноженное на стоимость товара), "Статус заказа".

   Данные для списка должны выбираться только одним LINQ запросом при помощи проекции.

6. Формы создания заказа и редактирование заказа. Клиент и товар должны выбираться при помощи dropdown (`<select />`).

### Примечания:

- Во всех формах обязательно должна быть валидация (если поле емейл, то валидатор на емейл, если поле обязательное, то required-валидатор и т.д). Не должно возникать никаких Exceptions, если поля не заполнены или заполнены неправильно. Иными словами, главное все сделать аккуратно и без багов.
- Client.Gender и Order.Status должны быть обязательно сделаны при помощи Enum.
- На Front-End не требуется никаких фреймворков.
```