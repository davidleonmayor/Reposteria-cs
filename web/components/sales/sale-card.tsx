"use client";

import { useState } from "react";

import { ChevronDown, Package } from "lucide-react";

import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";

import type { Sale } from "@/modules/sales/types";

type Props = {
  sale: Sale;
};

const STATE_STYLES: Record<string, string> = {
  Completada: "bg-emerald-100 text-emerald-700 border-emerald-200",
  Pendiente: "bg-amber-100 text-amber-700 border-amber-200",
  Cancelada: "bg-rose-100 text-rose-700 border-rose-200",
};

export const SaleCard = ({ sale }: Props) => {
  const [expanded, setExpanded] = useState(false);

  const stateClass =
    STATE_STYLES[sale.state] ?? "bg-slate-100 text-slate-700 border-slate-200";

  const dateStr = new Date(sale.saleDate).toLocaleDateString("es-AR", {
    day: "2-digit",
    month: "short",
    year: "numeric",
  });

  return (
    <div className="rounded-xl border border-slate-200 bg-white overflow-hidden">
      {/* Header row */}
      <div className="flex items-center justify-between gap-4 px-4 py-3">
        <div className="flex items-center gap-4 flex-1 min-w-0">
          <span className="text-sm font-medium text-slate-400 shrink-0">
            #{sale.id}
          </span>
          <span className="text-sm text-slate-600 shrink-0">{dateStr}</span>
          <span className="text-sm font-semibold text-slate-900 shrink-0">
            ${sale.total.toFixed(2)}
          </span>
          {sale.observations && (
            <span className="text-xs text-slate-400 truncate hidden sm:block">
              {sale.observations}
            </span>
          )}
        </div>

        <div className="flex items-center gap-2 shrink-0">
          <span
            className={`rounded-full border px-3 py-1 text-xs font-semibold ${stateClass}`}
          >
            {sale.state}
          </span>

          {sale.details.length > 0 && (
            <Button
              type="button"
              variant="ghost"
              size="icon"
              className="h-8 w-8 text-slate-400 hover:text-slate-700"
              onClick={() => setExpanded((v) => !v)}
              aria-label={expanded ? "Ocultar productos" : "Ver productos"}
            >
              <ChevronDown
                className={`h-4 w-4 transition-transform duration-200 ${expanded ? "rotate-180" : ""}`}
              />
            </Button>
          )}
        </div>
      </div>

      {/* Expandable products */}
      {expanded && sale.details.length > 0 && (
        <div className="border-t border-slate-100 bg-slate-50 px-4 py-3">
          <div className="flex items-center gap-2 mb-3">
            <Package className="h-3.5 w-3.5 text-slate-400" />
            <span className="text-xs font-semibold uppercase tracking-wide text-slate-400">
              Productos ({sale.details.length})
            </span>
          </div>

          <div className="flex flex-col divide-y divide-slate-100 rounded-lg border border-slate-200 bg-white overflow-hidden">
            {/* Column headers */}
            <div className="grid grid-cols-[1fr_auto_auto_auto] gap-4 px-4 py-2">
              <span className="text-xs font-semibold uppercase tracking-wide text-slate-400">
                Producto
              </span>
              <span className="text-xs font-semibold uppercase tracking-wide text-slate-400 text-right">
                Cant.
              </span>
              <span className="text-xs font-semibold uppercase tracking-wide text-slate-400 text-right">
                P. Unit.
              </span>
              <span className="text-xs font-semibold uppercase tracking-wide text-slate-400 text-right">
                Subtotal
              </span>
            </div>

            {sale.details.map((detail) => (
              <div
                key={detail.id}
                className="grid grid-cols-[1fr_auto_auto_auto] gap-4 px-4 py-2.5"
              >
                <span className="text-sm text-slate-700">{detail.productName}</span>
                <span className="text-sm text-slate-600 text-right">
                  {detail.quantity}
                </span>
                <span className="text-sm text-slate-600 text-right">
                  ${detail.unitPrice.toFixed(2)}
                </span>
                <span className="text-sm font-medium text-slate-900 text-right">
                  ${(detail.quantity * detail.unitPrice).toFixed(2)}
                </span>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};
