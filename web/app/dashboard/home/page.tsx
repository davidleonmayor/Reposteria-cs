"use client";

import { BestSellersSection } from "@/components/dashboard/best-sellers-section";
import { DashboardHeader } from "@/components/dashboard/header";
import { ProductCard } from "@/components/products/product-card";
import { useProducts } from "@/modules/products/hooks/useProducts";

export default function Home() {
  const { products, loading, error } = useProducts();

  return (
    <>
      <DashboardHeader />
      <section className="mx-auto flex w-full max-w-6xl flex-col gap-10 px-4 py-10 md:px-8">
        {/* <BestSellersSection /> */}

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

          {loading && (
            <p className="text-sm text-slate-500">Cargando productos...</p>
          )}

          {error && <p className="text-sm text-red-500">{error}</p>}

          {!loading && !error && (
            <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-3">
              {products.map((product, index) => (
                <ProductCard
                  key={product.id}
                  product={{
                    id: product.id,
                    name: product.name,
                    description: product.description,
                    price: product.price,
                    category: product.category.name,
                    image: "/hero.png",
                  }}
                  priority={index < 2}
                />
              ))}
            </div>
          )}
        </div>
      </section>
    </>
  );
}
