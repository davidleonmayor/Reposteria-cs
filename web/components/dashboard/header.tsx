"use client";

import { useMemo, useState } from "react";
import Image from "next/image";
import { usePathname } from "next/navigation";

import { ChevronDown } from "lucide-react";

import { cn } from "@/lib/utils";

import { CartItemCard } from "@/components/cart/cart-item-card";
import { CartSheet } from "@/components/cart/cart-sheet";

import { useProducts } from "@/modules/products/hooks/useProducts";
import { useCart } from "@/store/use-cart";

type CategoryKey = string;

export const DashboardHeader = () => {
  const pathname = usePathname();
  const showSidebar = pathname === "/dashboard/home";

  const [openCategory, setOpenCategory] = useState<CategoryKey | null>(null);

  const { products } = useProducts();

  const items = useCart((state) => state.items);
  const addItem = useCart((state) => state.addItem);
  const incrementItem = useCart((state) => state.incrementItem);
  const decrementItem = useCart((state) => state.decrementItem);
  const setQuantity = useCart((state) => state.setQuantity);

  const categoriesMap = useMemo(() => {
    const map = new Map<string, typeof products>();
    products.forEach((p) => {
      if (!map.has(p.category.name)) map.set(p.category.name, []);
      map.get(p.category.name)!.push(p);
    });
    return map;
  }, [products]);

  const categories = useMemo(() => [...categoriesMap.keys()], [categoriesMap]);

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
            sizes="36px"
            className="object-cover"
          />
        </div>
        <span className="text-lg font-bold text-slate-900">Repostería</span>
      </div>

      {/* Categorías dropdown */}
      <nav className="hidden lg:flex items-center gap-1">
        {categories.map((category) => {
          const productsInCategory = categoriesMap.get(category) ?? [];
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

              {isOpen && (
                <div className="absolute left-0 top-full mt-1 min-h-[200px] max-h-[calc(100vh-80px)] w-[420px] overflow-y-auto rounded-xl border border-slate-200 bg-white shadow-xl">
                  <div className="flex flex-col gap-3 p-4">
                    {productsInCategory.map((product) => {
                      const cartItem = items.find((it) => it.id === product.id);
                      const quantity = cartItem?.quantity ?? 0;
                      const image = product.hasImage
                        ? `/api/product/${product.id}/image`
                        : null;

                      return (
                        <CartItemCard
                          key={product.id}
                          image={image}
                          name={product.name}
                          unitPrice={product.price}
                          quantity={quantity}
                          onIncrement={() =>
                            quantity === 0
                              ? addItem({
                                  id: product.id,
                                  name: product.name,
                                  image,
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
                                image,
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

      <CartSheet />
    </header>
  );
};
