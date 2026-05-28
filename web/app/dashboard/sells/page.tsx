import { SalesList } from "@/components/sales/sales-list";

export default function Sells() {
  return (
    <main className="min-h-screen w-full">
      <section className="mx-auto flex w-full max-w-6xl flex-col gap-8 px-4 py-10 md:px-8">
        <header className="flex flex-col gap-2">
          <h1 className="text-3xl font-semibold text-slate-900 md:text-4xl">
            Ventas
          </h1>
          <p className="text-base text-slate-500">
            Historial de todas las ventas registradas. Expandí cada venta para
            ver los productos comprados.
          </p>
        </header>

        <SalesList />
      </section>
    </main>
  );
}
