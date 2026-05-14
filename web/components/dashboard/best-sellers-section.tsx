"use client";

import { useState } from "react";

import { ChevronDown, ChevronUp, TrendingUp } from "lucide-react";

import { Button } from "@/components/ui/button";

import { ProductCard } from "@/components/products/product-card";
import { PRODUCTS } from "@/moks/constants";

const BEST_SELLERS = PRODUCTS.slice(0, 20);

export const BestSellersSection = () => {
  const [open, setOpen] = useState(true);

  return (
    <section className="flex flex-col gap-4">
      <header className="flex items-center justify-between gap-4">
        <div className="flex items-center gap-3">
          <TrendingUp className="h-6 w-6 text-emerald-600" />
          <h2 className="text-2xl font-semibold text-slate-900 md:text-3xl">
            Más vendidos
          </h2>
          <span className="rounded-full bg-emerald-100 px-3 py-1 text-xs font-semibold text-emerald-700">
            Top 20
          </span>
        </div>
        <Button
          variant="ghost"
          size="icon"
          onClick={() => setOpen((prev) => !prev)}
          aria-label={open ? "Ocultar más vendidos" : "Mostrar más vendidos"}
        >
          {open ? (
            <ChevronUp className="h-5 w-5" />
          ) : (
            <ChevronDown className="h-5 w-5" />
          )}
        </Button>
      </header>

      {open && (
        <div className="overflow-x-auto py-2">
          <div className="flex gap-5">
            {BEST_SELLERS.map((product, index) => (
              <div key={product.id} className="w-[280px] shrink-0">
                <ProductCard product={product} priority={index < 4} />
              </div>
            ))}
          </div>
        </div>
      )}
    </section>
  );
};
