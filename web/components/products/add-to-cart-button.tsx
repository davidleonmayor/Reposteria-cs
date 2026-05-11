"use client";

import { Minus, Plus } from "lucide-react";

import { Button } from "@/components/ui/button";

import { useCart } from "@/store/use-cart";

type AddToCartButtonProps = {
  productId: number;
  productName: string;
  productImage: string;
  productPrice: number;
};

export const AddToCartButton = ({
  productId,
  productName,
  productImage,
  productPrice,
}: AddToCartButtonProps) => {
  const items = useCart((state) => state.items);
  const addItem = useCart((state) => state.addItem);
  const decrementItem = useCart((state) => state.decrementItem);

  const cartItem = items.find((item) => item.id === productId);
  const quantity = cartItem?.quantity ?? 0;

  return (
    <>
      <Button
        variant="ghost"
        size="icon"
        className="h-10 w-10 border border-rose-200 text-rose-500 hover:bg-rose-50 hover:text-rose-600 disabled:border-slate-200 disabled:text-slate-300"
        onClick={() => decrementItem(productId)}
        disabled={quantity === 0}
        aria-label="Quitar uno"
      >
        <Minus className="h-4 w-4" />
      </Button>

      <span className="text-base font-bold text-slate-900">
        {quantity === 0 ? "Agregar al carrito" : `${quantity} en el carrito`}
      </span>

      <Button
        variant="ghost"
        size="icon"
        className="h-10 w-10 border border-emerald-200 text-emerald-600 hover:bg-emerald-50 hover:text-emerald-700"
        onClick={() =>
          addItem({
            id: productId,
            name: productName,
            image: productImage,
            unitPrice: productPrice,
          })
        }
        aria-label="Agregar uno"
      >
        <Plus className="h-4 w-4" />
      </Button>
    </>
  );
};
