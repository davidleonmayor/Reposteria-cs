# ProductController

Path: `Api/Controllers/ProductController.cs`

## Routes
- `GET /api/Product` (anonymous)
- `GET /api/Product/{id}` (anonymous)
- `POST /api/Product` (`CatalogWrite`)
- `PUT /api/Product/{id}` (`CatalogWrite`)
- `DELETE /api/Product/{id}` (`CatalogWrite`)

## DTOs
- Create: `ProductCreateDto`
- Update: `ProductUpdateDto`

## Notes
- On create, `CategoryId = 0` is treated as "Uncategorized" (auto-created if missing).
