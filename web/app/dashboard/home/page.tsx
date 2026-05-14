import { BestSellersSection } from "@/components/dashboard/best-sellers-section";
import { DashboardHeader } from "@/components/dashboard/header";
import { ProductCard } from "@/components/products/product-card";
import { PRODUCTS } from "@/moks/constants";

export default function Home() {
  return (
    <>
      <DashboardHeader />
      <section className="mx-auto flex w-full max-w-6xl flex-col gap-10 px-4 py-10 md:px-8">
        <BestSellersSection />

        <div className="flex flex-col gap-6">
          <header className="flex items-start justify-between gap-4">
            <div className="flex flex-col gap-3">
              <h1 className="text-3xl font-semibold text-slate-900 md:text-4xl">
                Productos destacados de la reposteria
              </h1>
              <p className="text-xs font-semibold uppercase tracking-[0.3em] text-slate-500">
                Seleccion de hoy
              </p>
            </div>
          </header>

          <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-3">
            {PRODUCTS.map((product) => (
              <ProductCard
                key={product.id}
                product={product}
                priority={product.id <= 2}
              />
            ))}
          </div>
        </div>
      </section>
    </>
  );
}
