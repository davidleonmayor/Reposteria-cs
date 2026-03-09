# AppDbContext

Path: `Api/Data/AppDbContext.cs`

## DbSets
- People: `Person`, `PersonType`
- Catalog: `Category`, `Product`
- Sales: `Sale`, `SaleDetail`, `SaleParticipant`

## Relationship rules
- `Product -> Category`: restrict delete (`DeleteBehavior.Restrict`).
- `Sale -> Details`: cascade delete.
- `SaleDetail -> Product`: restrict delete.
- `Sale -> Participants`: cascade delete.
- `SaleParticipant -> Person`: restrict delete.
- Unique index: `(SaleId, PersonId, Role)`.
