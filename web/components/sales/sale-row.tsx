import { Badge } from "@/components/ui/badge";
import type { Sale } from "@/modules/sales/types";

type Props = {
  sale: Sale;
};

const STATE_STYLES: Record<string, string> = {
  Completada: "bg-emerald-100 text-emerald-700",
  Pendiente: "bg-amber-100 text-amber-700",
  Cancelada: "bg-rose-100 text-rose-700",
};

export const SaleRow = ({ sale }: Props) => {
  const stateClass = STATE_STYLES[sale.state] ?? "bg-slate-100 text-slate-700";
  const dateStr = new Date(sale.saleDate).toLocaleDateString("es-AR", {
    day: "2-digit",
    month: "short",
    year: "numeric",
  });

  return (
    <div className="grid grid-cols-[1fr_1fr_1fr_auto] items-center gap-4 px-4 py-3">
      <span className="text-sm font-medium text-slate-500">#{sale.id}</span>
      <span className="text-sm text-slate-700">{dateStr}</span>
      <span className="text-sm font-semibold text-slate-900">${sale.total.toFixed(2)}</span>
      <span className={`rounded-full px-3 py-1 text-xs font-semibold ${stateClass}`}>
        {sale.state}
      </span>
    </div>
  );
};
