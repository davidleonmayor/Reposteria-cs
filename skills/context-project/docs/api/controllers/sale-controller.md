# SaleController

Path: `Api/Controllers/SaleController.cs`

## Routes
- `GET /api/Sale` (anonymous)
- `GET /api/Sale/{id}` (anonymous)
- `POST /api/Sale` (`SalesWrite`)
- `PUT /api/Sale/{id}` (`SalesWrite`)
- `DELETE /api/Sale/{id}` (`SalesWrite`)

## DTOs
- Create: `SaleCreateDto` (participants + details required)
- Update: `SaleUpdateDto`

## Behavior notes
- On create:
  - Validates referenced `PersonId` and `ProductId` exist.
  - Validates stock; decrements `Product.Stock`.
  - Computes `Subtotal`/`Total` from details.
- On delete: restores stock for included details (optional behavior implemented).
