"use client";

import { useState } from "react";

import { toast } from "sonner";

import { Check, ShoppingCart, Trash2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetFooter,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet";

import { CartItemCard } from "@/components/cart/cart-item-card";
import { useCart } from "@/store/use-cart";

export const CartSheet = () => {
  const [open, setOpen] = useState(false);

  const items = useCart((state) => state.items);
  const incrementItem = useCart((state) => state.incrementItem);
  const decrementItem = useCart((state) => state.decrementItem);
  const removeItem = useCart((state) => state.removeItem);
  const setQuantity = useCart((state) => state.setQuantity);
  const clearCart = useCart((state) => state.clearCart);

  const itemsCount = items.length;
  const total = items.reduce(
    (sum, item) => sum + item.unitPrice * item.quantity,
    0,
  );

  const handleAcceptPurchase = () => {
    toast.success("Compra realizada.");
    clearCart();
    setOpen(false);
  };

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger asChild>
        <Button
          variant="ghost"
          size="icon"
          className="relative h-11 w-11 border border-slate-200 bg-white"
          aria-label="Abrir carrito"
        >
          <ShoppingCart className="h-5 w-5 text-slate-700" />
          {itemsCount > 0 && (
            <span className="absolute -right-1 -top-1 flex h-5 w-5 items-center justify-center rounded-full bg-rose-500 text-[10px] font-bold text-white">
              {itemsCount}
            </span>
          )}
        </Button>
      </SheetTrigger>

      <SheetContent
        side="right"
        className="flex w-full flex-col gap-0 bg-white sm:max-w-md"
      >
        <SheetHeader className="border-b border-slate-100 p-4">
          <SheetTitle className="flex items-center gap-2 text-lg">
            <ShoppingCart className="h-5 w-5 text-green-600" />
            Tu carrito
          </SheetTitle>
          <SheetDescription>
            {itemsCount === 0
              ? "Aun no has agregado productos."
              : `${itemsCount} ${itemsCount === 1 ? "producto" : "productos"} listos para comprar.`}
          </SheetDescription>
        </SheetHeader>

        <div className="flex-1 overflow-y-auto px-4 py-4">
          {itemsCount === 0 ? (
            <div className="flex h-full flex-col items-center justify-center gap-3 text-center">
              <ShoppingCart className="h-10 w-10 text-slate-300" />
              <p className="text-sm text-slate-500">El carrito esta vacio</p>
            </div>
          ) : (
            <div className="flex flex-col gap-3">
              {items.map((item) => (
                <CartItemCard
                  key={item.id}
                  image={item.image}
                  name={item.name}
                  unitPrice={item.unitPrice}
                  quantity={item.quantity}
                  onIncrement={() => incrementItem(item.id)}
                  onDecrement={() => decrementItem(item.id)}
                  onRemove={() => removeItem(item.id)}
                  onQuantityChange={(q) =>
                    setQuantity(
                      {
                        id: item.id,
                        name: item.name,
                        image: item.image,
                        unitPrice: item.unitPrice,
                      },
                      q,
                    )
                  }
                />
              ))}
            </div>
          )}
        </div>

        <SheetFooter className="border-t border-slate-100 p-4">
          <div className="flex items-center justify-between pb-3">
            <span className="text-sm font-medium text-slate-600">Total</span>
            <span className="text-xl font-bold text-slate-900">
              ${total.toFixed(2)}
            </span>
          </div>
          <div className="flex flex-row gap-2">
            <Button
              variant="dangerOutline"
              className="flex-1"
              onClick={clearCart}
              disabled={itemsCount === 0}
            >
              <Trash2 className="mr-2 h-4 w-4" />
              Limpiar
            </Button>
            <Button
              variant="secondary"
              className="flex-1"
              onClick={handleAcceptPurchase}
              disabled={itemsCount === 0}
            >
              <Check className="mr-2 h-4 w-4" />
              Aceptar compra
            </Button>
          </div>
        </SheetFooter>
      </SheetContent>
    </Sheet>
  );
};
