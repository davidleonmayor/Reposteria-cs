"use client";

import { useSales } from "@/modules/sales/hooks/useSales";
import { SaleRow } from "./sale-row";

export const SalesList = () => {
  const { sales, loading, error } = useSales();

  if (loading) {
    return <p className="text-sm text-slate-500">Cargando ventas...</p>;
  }

  if (error) {
    return <p className="text-sm text-red-500">{error}</p>;
  }

  return (
    <div className="flex flex-col gap-4">
      {/* Counter */}
      <p className="text-sm text-slate-500">
        {sales.length} venta{sales.length !== 1 ? "s" : ""}
      </p>

      {/* Empty state */}
      {sales.length === 0 && (
        <div className="flex h-40 items-center justify-center rounded-xl border border-dashed border-slate-300 bg-slate-50">
          <p className="text-sm text-slate-500">No hay ventas registradas aún.</p>
        </div>
      )}

      {/* Table header */}
      {sales.length > 0 && (
        <div className="flex flex-col rounded-xl border border-slate-200 bg-white">
          {/* Header row */}
          <div className="grid grid-cols-[1fr_1fr_1fr_auto] gap-4 border-b border-slate-100 px-4 py-2">
            <span className="text-xs font-semibold uppercase tracking-wide text-slate-400">ID</span>
            <span className="text-xs font-semibold uppercase tracking-wide text-slate-400">Fecha</span>
            <span className="text-xs font-semibold uppercase tracking-wide text-slate-400">Total</span>
            <span className="text-xs font-semibold uppercase tracking-wide text-slate-400">Estado</span>
          </div>
          {/* Data rows */}
          <div className="flex flex-col divide-y divide-slate-100">
            {sales.map((sale) => (
              <SaleRow key={sale.id} sale={sale} />
            ))}
          </div>
        </div>
      )}
    </div>
  );
};
