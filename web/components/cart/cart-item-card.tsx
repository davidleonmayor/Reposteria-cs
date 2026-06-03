import Image from "next/image";

import { Minus, Plus, Trash2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Input } from "@/components/ui/input";

type Props = {
  image: string | null;
  name: string;
  unitPrice: number;
  quantity: number;
  onIncrement?: () => void;
  onDecrement?: () => void;
  onRemove?: () => void;
  onQuantityChange?: (quantity: number) => void;
};

export const CartItemCard = ({
  image,
  name,
  unitPrice,
  quantity,
  onIncrement,
  onDecrement,
  onRemove,
  onQuantityChange,
}: Props) => {
  const finalPrice = unitPrice * quantity;

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (!onQuantityChange) return;
    const value = e.target.value;
    if (value === "") {
      onQuantityChange(0);
      return;
    }
    const parsed = Number.parseInt(value, 10);
    if (Number.isNaN(parsed) || parsed < 0) return;
    onQuantityChange(parsed);
  };

  return (
    <Card size="sm" className="flex-row items-center gap-3 px-3 py-3">
      <div className="relative h-16 w-16 shrink-0 overflow-hidden rounded-lg bg-slate-100">
        {image ? (
          <Image
            src={image}
            alt={name}
            fill
            className="object-cover"
            sizes="64px"
            unoptimized
          />
        ) : (
          <div className="flex h-full w-full items-center justify-center text-slate-300">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              className="h-7 w-7"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={1}
                d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"
              />
            </svg>
          </div>
        )}
      </div>

      <div className="flex min-w-0 flex-1 flex-col gap-1.5">
        <div className="flex items-start justify-between gap-2">
          <h3 className="truncate text-sm font-semibold text-slate-900">
            {name}
          </h3>
          {onRemove && (
            <Button
              variant="ghost"
              size="icon"
              className="h-7 w-7 text-rose-500 hover:bg-rose-50 hover:text-rose-600"
              onClick={onRemove}
              aria-label="Eliminar producto"
            >
              <Trash2 className="h-4 w-4" />
            </Button>
          )}
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
            <Input
              type="number"
              min={0}
              inputMode="numeric"
              value={quantity}
              onChange={handleInputChange}
              className="h-7 w-12 px-1 text-center text-sm font-semibold text-slate-900 [appearance:textfield] [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
              aria-label="Cantidad en el carrito"
            />
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
