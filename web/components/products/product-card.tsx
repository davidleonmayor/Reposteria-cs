"use client";

import Image from "next/image";

import { Minus, Plus } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";

import { useCart } from "@/store/use-cart";

type Product = {
  id: number;
  name: string;
  description: string;
  price: number;
  category: string;
  image: string;
};

type Props = {
  product: Product;
  priority?: boolean;
};

export const ProductCard = ({ product, priority = false }: Props) => {
  const items = useCart((state) => state.items);
  const addItem = useCart((state) => state.addItem);
  const decrementItem = useCart((state) => state.decrementItem);
  const setQuantity = useCart((state) => state.setQuantity);

  const cartItem = items.find((item) => item.id === product.id);
  const quantity = cartItem?.quantity ?? 0;

  const handleQuantityChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    if (value === "") {
      setQuantity(
        {
          id: product.id,
          name: product.name,
          image: product.image,
          unitPrice: product.price,
        },
        0,
      );
      return;
    }
    const parsed = Number.parseInt(value, 10);
    if (Number.isNaN(parsed) || parsed < 0) return;
    setQuantity(
      {
        id: product.id,
        name: product.name,
        image: product.image,
        unitPrice: product.price,
      },
      parsed,
    );
  };

  return (
    <Card className="overflow-hidden transition hover:-translate-y-1 hover:shadow-lg">
      <div className="relative h-44 w-full">
        <Image
          src={product.image}
          alt={product.name}
          fill
          className="object-cover"
          sizes="(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 33vw"
          priority={priority}
        />
      </div>

      <CardHeader className="gap-2">
        <div className="flex items-center justify-between gap-3">
          <CardTitle className="text-lg text-slate-900">
            {product.name}
          </CardTitle>
          <span className="rounded-full bg-emerald-100 px-3 py-1 text-xs font-semibold text-emerald-700">
            {product.category}
          </span>
        </div>
        <CardDescription>{product.description}</CardDescription>
      </CardHeader>

      <CardContent className="flex items-center justify-between text-sm text-slate-500">
        <span className="text-sm font-medium text-slate-600">Precio</span>
        <span className="text-lg font-semibold text-slate-900">
          ${product.price.toFixed(2)}
        </span>
      </CardContent>

      <CardFooter className="flex items-center justify-between gap-2">
        <Button
          variant="ghost"
          size="icon"
          className="h-10 w-10 border border-rose-200 text-rose-500 hover:bg-rose-50 hover:text-rose-600 disabled:border-slate-200 disabled:text-slate-300"
          onClick={() => decrementItem(product.id)}
          disabled={quantity === 0}
          aria-label="Quitar uno"
        >
          <Minus className="h-4 w-4" />
        </Button>

        <Input
          type="number"
          min={0}
          inputMode="numeric"
          value={quantity}
          onChange={handleQuantityChange}
          className="h-10 w-20 text-center text-base font-bold text-slate-900 [appearance:textfield] [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
          aria-label="Cantidad en el carrito"
        />

        <Button
          variant="ghost"
          size="icon"
          className="h-10 w-10 border border-emerald-200 text-emerald-600 hover:bg-emerald-50 hover:text-emerald-700"
          onClick={() =>
            addItem({
              id: product.id,
              name: product.name,
              image: product.image,
              unitPrice: product.price,
            })
          }
          aria-label="Agregar uno"
        >
          <Plus className="h-4 w-4" />
        </Button>
      </CardFooter>
    </Card>
  );
};
