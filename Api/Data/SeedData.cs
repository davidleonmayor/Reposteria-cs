using Api.Core.Modules.Auth.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

public static class SeedData
{
    /// <summary>
    /// Populates the database with mock data if it is empty.
    /// Call this at startup after applying migrations.
    /// Idempotent — does nothing if PersonType rows already exist.
    /// </summary>
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        if (await context.PersonType.AnyAsync()) return;

        const string defaultPassword = "Password123!";
        var hash = passwordHasher.Hash(defaultPassword);

        // ── 1. PersonTypes ────────────────────────────────────────────────────
        var ptCliente      = new PersonType { Name = "Cliente",       Description = "Clientes de la repostería" };
        var ptVendedor     = new PersonType { Name = "Vendedor",      Description = "Personal de ventas" };
        var ptProveedor    = new PersonType { Name = "Proveedor",     Description = "Proveedores de insumos" };
        var ptAdmin        = new PersonType { Name = "Administrador", Description = "Administración del negocio" };

        context.PersonType.AddRange(ptCliente, ptVendedor, ptProveedor, ptAdmin);
        await context.SaveChangesAsync();

        // ── 2. Categories ─────────────────────────────────────────────────────
        var catTortas    = new Category { Name = "Tortas",    Description = "Tortas artesanales para toda ocasión" };
        var catPanaderia = new Category { Name = "Panadería", Description = "Pan artesanal y productos de panadería" };
        var catPostres   = new Category { Name = "Postres",   Description = "Postres finos y tradicionales" };
        var catBocados   = new Category { Name = "Bocados",   Description = "Pequeños bocados dulces" };
        var catTartas    = new Category { Name = "Tartas",    Description = "Tartas dulces y saladas" };
        var catGalletas  = new Category { Name = "Galletas",  Description = "Galletas artesanales variadas" };

        context.Category.AddRange(catTortas, catPanaderia, catPostres, catBocados, catTartas, catGalletas);
        await context.SaveChangesAsync();

        // ── 3. Persons ────────────────────────────────────────────────────────
        var p1 = new Person
        {
            PersonTypeId = ptAdmin.Id,
            Name = "María", LastName = "González",
            Phone = "3001234567", Email = "maria@reposteria.com",
            Address = "Calle 10 # 5-20, Pasto",
            RegisterDate = DateTime.UtcNow.AddMonths(-12), Active = true, PasswordHash = hash
        };
        var p2 = new Person
        {
            PersonTypeId = ptVendedor.Id,
            Name = "Juan", LastName = "Pérez",
            Phone = "3109876543", Email = "juan@reposteria.com",
            Address = "Carrera 8 # 12-45, Pasto",
            RegisterDate = DateTime.UtcNow.AddMonths(-8), Active = true, PasswordHash = hash
        };
        var p3 = new Person
        {
            PersonTypeId = ptCliente.Id,
            Name = "Sofía", LastName = "Martínez",
            Phone = "3204567890", Email = "sofia@gmail.com",
            Address = "Calle 18 # 3-10, Pasto",
            RegisterDate = DateTime.UtcNow.AddMonths(-5), Active = true, PasswordHash = hash
        };
        var p4 = new Person
        {
            PersonTypeId = ptCliente.Id,
            Name = "Carlos", LastName = "Rodríguez",
            Phone = "3155551234", Email = "carlos@gmail.com",
            Address = "Avenida Centro # 22-08, Pasto",
            RegisterDate = DateTime.UtcNow.AddMonths(-3), Active = true, PasswordHash = hash
        };
        var p5 = new Person
        {
            PersonTypeId = ptProveedor.Id,
            Name = "Laura", LastName = "Vargas",
            Phone = "3012223344", Email = "laura@proveedores.com",
            Address = "Zona Industrial, Pasto",
            RegisterDate = DateTime.UtcNow.AddMonths(-10), Active = true, PasswordHash = hash
        };

        context.Person.AddRange(p1, p2, p3, p4, p5);
        await context.SaveChangesAsync();

        // ── 4. Products ───────────────────────────────────────────────────────
        var products = new[]
        {
            // Tortas
            new Product { Name = "Torta de chocolate",   Description = "Bizcocho húmedo con cobertura de cacao y chips crujientes.", Price = 28.50m, Stock = 10, CategoryId = catTortas.Id,    Active = true },
            new Product { Name = "Torta de vainilla",    Description = "Bizcocho clásico con crema chantilly y trozos de fruta.",    Price = 24.00m, Stock = 8,  CategoryId = catTortas.Id,    Active = true },
            new Product { Name = "Torta red velvet",     Description = "Bizcocho rojo con frosting de queso crema suave.",           Price = 30.00m, Stock = 6,  CategoryId = catTortas.Id,    Active = true },
            // Panadería
            new Product { Name = "Croissant de mantequilla", Description = "Hojaldre dorado con capas ligeras y aroma a vainilla.", Price = 4.20m,  Stock = 30, CategoryId = catPanaderia.Id, Active = true },
            new Product { Name = "Pan de canela",        Description = "Roll dulce con canela, azúcar morena y glaseado.",           Price = 4.00m,  Stock = 25, CategoryId = catPanaderia.Id, Active = true },
            new Product { Name = "Empanada de queso",    Description = "Hojaldre crujiente con relleno de queso fundido.",           Price = 3.50m,  Stock = 40, CategoryId = catPanaderia.Id, Active = true },
            // Postres
            new Product { Name = "Cheesecake frutos rojos", Description = "Base crocante, crema suave y mermelada artesanal.",      Price = 22.00m, Stock = 12, CategoryId = catPostres.Id,   Active = true },
            new Product { Name = "Tiramisú",             Description = "Capas de bizcocho con café, mascarpone y cacao.",            Price = 8.50m,  Stock = 15, CategoryId = catPostres.Id,   Active = true },
            new Product { Name = "Crème brûlée",         Description = "Crema de vainilla con costra de azúcar caramelizada.",       Price = 7.50m,  Stock = 20, CategoryId = catPostres.Id,   Active = true },
            // Bocados
            new Product { Name = "Brownie intenso",      Description = "Chocolate 70% cacao con nueces tostadas y sal marina.",      Price = 6.50m,  Stock = 50, CategoryId = catBocados.Id,   Active = true },
            new Product { Name = "Macaron de pistacho",  Description = "Tapas de almendra rellenas con ganache de pistacho.",        Price = 2.50m,  Stock = 60, CategoryId = catBocados.Id,   Active = true },
            new Product { Name = "Donut glaseado",       Description = "Esponjosa rosquilla con glaseado clásico.",                  Price = 2.80m,  Stock = 45, CategoryId = catBocados.Id,   Active = true },
            // Tartas
            new Product { Name = "Tarta de limón",       Description = "Crema cítrica balanceada con merengue suave y tostado.",     Price = 18.90m, Stock = 8,  CategoryId = catTartas.Id,    Active = true },
            new Product { Name = "Tarta de manzana",     Description = "Manzanas en lámina sobre masa quebrada y canela.",           Price = 17.50m, Stock = 10, CategoryId = catTartas.Id,    Active = true },
            // Galletas
            new Product { Name = "Galletas vainilla",    Description = "Clásicas, mantecosas y perfectas para acompañar café.",      Price = 3.80m,  Stock = 80, CategoryId = catGalletas.Id,  Active = true },
            new Product { Name = "Alfajores",            Description = "Dos galletas de maicena unidas con dulce de leche.",         Price = 4.50m,  Stock = 70, CategoryId = catGalletas.Id,  Active = true },
        };

        context.Product.AddRange(products);
        await context.SaveChangesAsync();

        // ── 5. Sales + Details + Participants ─────────────────────────────────
        // Helper: Subtotal = sum(qty * unitPrice), Total = Subtotal (sin impuestos en este modelo)
        var salesData = new[]
        {
            new
            {
                SaleDate = DateTime.UtcNow.AddDays(-30),
                State = "Completada",
                Observations = (string?)null,
                SellerId = p2.Id,
                BuyerId = p3.Id,
                Items = new[] { (products[0], 1), (products[9], 3) }   // Torta chocolate + Brownies
            },
            new
            {
                SaleDate = DateTime.UtcNow.AddDays(-20),
                State = "Completada",
                Observations = (string?)"Pedido para cumpleaños",
                SellerId = p2.Id,
                BuyerId = p4.Id,
                Items = new[] { (products[2], 1), (products[6], 1) }   // Red velvet + Cheesecake
            },
            new
            {
                SaleDate = DateTime.UtcNow.AddDays(-10),
                State = "Completada",
                Observations = (string?)null,
                SellerId = p1.Id,
                BuyerId = p3.Id,
                Items = new[] { (products[3], 4), (products[4], 2), (products[14], 2) } // Croissants + Pan canela + Galletas
            },
            new
            {
                SaleDate = DateTime.UtcNow.AddDays(-5),
                State = "Pendiente",
                Observations = (string?)"Cliente pasa a retirar el viernes",
                SellerId = p2.Id,
                BuyerId = p4.Id,
                Items = new[] { (products[1], 1), (products[12], 1) }  // Torta vainilla + Tarta limón
            },
            new
            {
                SaleDate = DateTime.UtcNow.AddDays(-2),
                State = "Pendiente",
                Observations = (string?)null,
                SellerId = p1.Id,
                BuyerId = p3.Id,
                Items = new[] { (products[7], 2), (products[10], 6) }  // Tiramisú + Macarons
            },
            new
            {
                SaleDate = DateTime.UtcNow.AddDays(-1),
                State = "Cancelada",
                Observations = (string?)"Cliente canceló por cambio de fecha",
                SellerId = p2.Id,
                BuyerId = p4.Id,
                Items = new[] { (products[0], 2) }                     // Torta chocolate x2
            },
        };

        foreach (var sd in salesData)
        {
            var subtotal = sd.Items.Sum(i => i.Item1.Price * i.Item2);

            var sale = new Sale
            {
                SaleDate     = sd.SaleDate,
                Subtotal     = subtotal,
                Total        = subtotal,
                State        = sd.State,
                Observations = sd.Observations,
            };

            context.Sale.Add(sale);
            await context.SaveChangesAsync(); // get sale.Id

            // Details
            foreach (var (product, qty) in sd.Items)
            {
                context.SaleDetail.Add(new SaleDetail
                {
                    SaleId    = sale.Id,
                    ProductId = product.Id,
                    Quantity  = qty,
                    UnitPrice = product.Price,
                });
            }

            // Participants
            context.SaleParticipant.AddRange(
                new SaleParticipant { SaleId = sale.Id, PersonId = sd.SellerId, Role = "Vendedor" },
                new SaleParticipant { SaleId = sale.Id, PersonId = sd.BuyerId,  Role = "Cliente"  }
            );

            await context.SaveChangesAsync();
        }
    }
}
