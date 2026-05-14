"use client";

import { useState } from "react";
import Image from "next/image";
import { usePathname } from "next/navigation";

import { ChevronDown, ShoppingCart } from "lucide-react";

import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
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

import { PRODUCTS, PRODUCT_CATEGORIES } from "@/moks/constants";
import { useCart } from "@/store/use-cart";

type CategoryKey = string;

export const DashboardHeader = () => {
  const pathname = usePathname();
  const showSidebar = pathname === "/dashboard/home";

  const [open, setOpen] = useState(false);
  const [openCategory, setOpenCategory] = useState<CategoryKey | null>(null);

  const items = useCart((state) => state.items);
  const addItem = useCart((state) => state.addItem);
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
    setOpen(false);
  };

  return (
    <header
      className={cn(
        "fixed top-0 left-0 right-0 z-50 flex h-16 items-center justify-between border-b border-slate-200 bg-white px-4 lg:px-6",
        showSidebar && "hidden lg:flex lg:left-[256px]",
      )}
    >
      {/* Logo / Brand */}
      <div className="flex items-center gap-3">
        <div className="relative h-9 w-9 overflow-hidden rounded-lg">
          <Image
            src="/hero.png"
            alt="Repostería"
            fill
            className="object-cover"
          />
        </div>
        <span className="text-lg font-bold text-slate-900">Repostería</span>
      </div>

      {/* Categorías dropdown */}
      <nav className="hidden lg:flex items-center gap-1">
        {PRODUCT_CATEGORIES.map((category) => {
          const productsInCategory = PRODUCTS.filter(
            (p) => p.category === category,
          );
          const isOpen = openCategory === category;

          return (
            <div
              key={category}
              className="relative"
              onMouseEnter={() => setOpenCategory(category)}
              onMouseLeave={() => setOpenCategory(null)}
            >
              <button
                className={`flex h-10 items-center gap-1.5 rounded-md px-4 text-sm font-medium transition-colors ${
                  isOpen
                    ? "bg-emerald-50 text-emerald-700"
                    : "text-slate-600 hover:bg-slate-50 hover:text-slate-900"
                }`}
              >
                {category}
                <ChevronDown
                  className={`h-4 w-4 transition-transform ${isOpen ? "rotate-180" : ""}`}
                />
              </button>

              {/* Dropdown */}
              {isOpen && (
                <div className="absolute left-0 top-full mt-1 min-h-[200px] max-h-[calc(100vh-80px)] w-[420px] overflow-y-auto rounded-xl border border-slate-200 bg-white shadow-xl">
                  <div className="flex flex-col gap-3 p-4">
                    {productsInCategory.map((product) => {
                      const cartItem = items.find((it) => it.id === product.id);
                      const quantity = cartItem?.quantity ?? 0;
                      return (
                        <CartItemCard
                          key={product.id}
                          image={product.image}
                          name={product.name}
                          unitPrice={product.price}
                          quantity={quantity}
                          onIncrement={() =>
                            quantity === 0
                              ? addItem({
                                  id: product.id,
                                  name: product.name,
                                  image: product.image,
                                  unitPrice: product.price,
                                })
                              : incrementItem(product.id)
                          }
                          onDecrement={() => decrementItem(product.id)}
                          onQuantityChange={(q) =>
                            setQuantity(
                              {
                                id: product.id,
                                name: product.name,
                                image: product.image,
                                unitPrice: product.price,
                              },
                              q,
                            )
                          }
                        />
                      );
                    })}
                  </div>
                </div>
              )}
            </div>
          );
        })}
      </nav>

      {/* Carrito */}
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
                ? "Aún no has agregado productos."
                : `${itemsCount} ${itemsCount === 1 ? "producto" : "productos"} listos para comprar.`}
            </SheetDescription>
          </SheetHeader>

          <div className="flex-1 overflow-y-auto px-4 py-4">
            {itemsCount === 0 ? (
              <div className="flex h-full flex-col items-center justify-center gap-3 text-center">
                <ShoppingCart className="h-10 w-10 text-slate-300" />
                <p className="text-sm text-slate-500">El carrito está vacío</p>
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
                Limpiar
              </Button>
              <Button
                variant="secondary"
                className="flex-1"
                onClick={handleAcceptPurchase}
                disabled={itemsCount === 0}
              >
                Aceptar compra
              </Button>
            </div>
          </SheetFooter>
        </SheetContent>
      </Sheet>
    </header>
  );
};
