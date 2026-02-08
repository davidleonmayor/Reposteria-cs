erDiagram
TipoPersona {
int Id PK
string Nombre
string Descripcion
}

    Personas {
        int Id PK
        int TipoPersonaId FK
        string Nombre
        string Apellido
        string Telefono
        string Email
        string Direccion
        datetime FechaRegistro
        bool Activo
    }

    Categorias {
        int Id PK
        string Nombre
        string Descripcion
    }

    Productos {
        int Id PK
        string Nombre
        string Descripcion
        real Precio
        int Stock
        int CategoriaId FK
        bool Activo
    }

    Ventas {
        int Id PK
        int ClienteId FK
        int VendedorId FK
        datetime FechaVenta
        real Total
        string Estado
        string Observaciones
    }

        int Id PK
        int VentaId FK
    DetalleVentas {
        int ProductoId FK
        int Cantidad
        real PrecioUnitario
        real Subtotal
    }

    TipoPersona ||--o{ Personas : "clasifica"
    Personas ||--o{ Ventas : "compra como cliente"
    Personas ||--o{ Ventas : "gestiona como vendedor"
    Categorias ||--o{ Productos : "tiene"
    Ventas ||--|{ DetalleVentas : "contiene"
    Productos ||--o{ DetalleVentas : "incluido en"v
