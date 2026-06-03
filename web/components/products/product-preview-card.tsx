import Image from "next/image";

import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";

import { AddToCartButton } from "./add-to-cart-button";

type Product = {
  id: number;
  name: string;
  description: string;
  price: number;
  category: string;
  image: string | null;
};

type ProductPreviewCardProps = {
  product: Product;
  compact?: boolean;
};

export const ProductPreviewCard = ({
  product,
  compact = false,
}: ProductPreviewCardProps) => {
  return (
    <Card
      className={`overflow-hidden transition hover:-translate-y-0.5 hover:shadow-md ${
        compact ? "h-full" : ""
      }`}
    >
      <div className="relative h-36 w-full overflow-hidden rounded-none bg-slate-100">
        {product.image ? (
          <Image
            src={product.image}
            alt={product.name}
            fill
            className="object-cover"
            sizes="(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 33vw"
            unoptimized
          />
        ) : (
          <div className="flex h-full w-full items-center justify-center text-slate-300">
            <svg xmlns="http://www.w3.org/2000/svg" className="h-10 w-10" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
            </svg>
          </div>
        )}
      </div>

      <CardHeader className="gap-1.5 p-3">
        <CardTitle className="text-sm text-slate-900 line-clamp-1">
          {product.name}
        </CardTitle>
        <CardContent className="flex items-center justify-between p-0">
          <span className="text-sm font-semibold text-emerald-600">
            ${product.price.toFixed(2)}
          </span>
        </CardContent>
      </CardHeader>

      <div className="px-3 pb-3">
        <AddToCartButton
          productId={product.id}
          productName={product.name}
          productImage={product.image ?? ""}
          productPrice={product.price}
        />
      </div>
    </Card>
  );
};