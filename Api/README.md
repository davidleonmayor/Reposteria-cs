```mermaid
erDiagram
    PersonType {
        int Id PK
        string Name
        string Description
    }

    Person {
        int Id PK
        int PersonTypeId FK
        string Name
        string LastName
        string Phone
        string Email
        string Address
        datetime RegisterDate
        bool Active
    }

    Category {
        int Id PK
        string Name
        string Description
    }

    Product {
        int Id PK
        string Name
        string Description
        decimal Price
        int Stock
        int CategoryId FK
        bool Active
    }

    Sale {
        int Id PK
        datetime SaleDate
        decimal Subtotal
        decimal Total
        string State
        string Observations
    }

    SaleParticipant {
        int Id PK
        int SaleId FK
        int PersonId FK
        string Role
    }

    SaleDetail {
        int Id PK
        int SaleId FK
        int ProductId FK
        int Quantity
        decimal UnitPrice
    }

    PersonType ||--o{ Person : "classifies"
    Category ||--o{ Product : "has"
    Sale ||--o{ SaleParticipant : "has participants"
    Person ||--o{ SaleParticipant : "participates"
    Sale ||--|{ SaleDetail : "contains"
    Product ||--o{ SaleDetail : "sold in"
```

## Seed de datos

El seed se ejecuta automáticamente al iniciar la API si la base de datos está vacía.

### Cómo resetear y re-ejecutar el seed

```bash
# 1. Desde la carpeta Api/ (IMPORTANTE: correr siempre desde aquí)
cd Api

# 2. Eliminar la base de datos actual
rm app.db

# 3. Iniciar la API — aplica migrations y seed automáticamente
dotnet run
```

### Datos generados

| Tabla | Registros |
|-------|-----------|
| PersonType | 4 (Administrador, Vendedor, Cliente, Proveedor) |
| Category | 6 (Tortas, Panadería, Postres, Bocados, Tartas, Galletas) |
| Person | 5 — contraseña de todos: `Password123!` |
| Product | 16 distribuidos entre las 6 categorías |
| Sale | 6 (3 Completadas, 2 Pendientes, 1 Cancelada) |

### Credenciales de prueba

| Email | Contraseña | Rol |
|-------|------------|-----|
| maria@reposteria.com | Password123! | Administrador |
| juan@reposteria.com | Password123! | Vendedor |
| sofia@gmail.com | Password123! | Cliente |
