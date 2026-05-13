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
  image: string;
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
      <div className="relative w-full overflow-hidden rounded-none">
        <Image
          src={product.image}
          alt={product.name}
          fill
          className="object-cover"
          sizes="(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 33vw"
        />
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
          productImage={product.image}
          productPrice={product.price}
        />
      </div>
    </Card>
  );
};