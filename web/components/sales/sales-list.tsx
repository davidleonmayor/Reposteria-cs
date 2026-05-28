"use client";

import { ShoppingBag } from "lucide-react";

import { SaleCard } from "@/components/sales/sale-card";
import { useSales } from "@/modules/sales/hooks/useSales";

export const SalesList = () => {
  const { sales, loading, error } = useSales();

  if (loading) {
    return (
      <div className="flex h-40 items-center justify-center">
        <p className="text-sm text-slate-500">Cargando ventas...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex h-40 items-center justify-center">
        <p className="text-sm text-red-500">{error}</p>
      </div>
    );
  }

  if (sales.length === 0) {
    return (
      <div className="flex h-40 flex-col items-center justify-center gap-2 rounded-xl border border-dashed border-slate-300 bg-slate-50">
        <ShoppingBag className="h-8 w-8 text-slate-300" />
        <p className="text-sm text-slate-500">No hay ventas registradas aún.</p>
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-3">
      <p className="text-sm text-slate-500">
        {sales.length} venta{sales.length !== 1 ? "s" : ""} registradas
      </p>
      {sales.map((sale) => (
        <SaleCard key={sale.id} sale={sale} />
      ))}
    </div>
  );
};
