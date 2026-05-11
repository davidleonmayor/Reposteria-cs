import Image from "next/image";

import { Minus, Plus, Trash2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";

type Props = {
  image: string;
  name: string;
  unitPrice: number;
  quantity: number;
  onIncrement?: () => void;
  onDecrement?: () => void;
  onRemove?: () => void;
};

export const CartItemCard = ({
  image,
  name,
  unitPrice,
  quantity,
  onIncrement,
  onDecrement,
  onRemove,
}: Props) => {
  const finalPrice = unitPrice * quantity;

  return (
    <Card
      size="sm"
      className="flex-row items-center gap-3 px-3 py-3"
    >
      <div className="relative h-16 w-16 shrink-0 overflow-hidden rounded-lg bg-slate-100">
        <Image
          src={image}
          alt={name}
          fill
          className="object-cover"
          sizes="64px"
        />
      </div>

      <div className="flex min-w-0 flex-1 flex-col gap-1.5">
        <div className="flex items-start justify-between gap-2">
          <h3 className="truncate text-sm font-semibold text-slate-900">
            {name}
          </h3>
          <Button
            variant="ghost"
            size="icon"
            className="h-7 w-7 text-rose-500 hover:bg-rose-50 hover:text-rose-600"
            onClick={onRemove}
            aria-label="Eliminar producto"
          >
            <Trash2 className="h-4 w-4" />
          </Button>
        </div>

        <p className="text-xs text-slate-500">
          Unitario: ${unitPrice.toFixed(2)}
        </p>

        <div className="flex items-center justify-between gap-2">
          <div className="flex items-center gap-1">
            <Button
              variant="ghost"
              size="icon"
              className="h-7 w-7 border border-slate-200"
              onClick={onDecrement}
              aria-label="Disminuir cantidad"
            >
              <Minus className="h-3.5 w-3.5" />
            </Button>
            <span className="min-w-[2ch] text-center text-sm font-semibold text-slate-900">
              {quantity}
            </span>
            <Button
              variant="ghost"
              size="icon"
              className="h-7 w-7 border border-slate-200"
              onClick={onIncrement}
              aria-label="Aumentar cantidad"
            >
              <Plus className="h-3.5 w-3.5" />
            </Button>
          </div>

          <span className="text-sm font-bold text-slate-900">
            ${finalPrice.toFixed(2)}
          </span>
        </div>
      </div>
    </Card>
  );
};
