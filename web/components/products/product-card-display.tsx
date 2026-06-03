import Image from "next/image";
import Link from "next/link";

import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";

type Product = {
  id: number;
  name: string;
  description: string;
  price: number;
  category: string;
  image: string | null;
};

type Props = {
  product: Product;
  priority?: boolean;
  href?: string;
};

export const ProductCardDisplay = ({
  product,
  priority = false,
  href,
}: Props) => {
  const cardContent = (
    <>
      <div className="relative h-44 w-full bg-slate-100">
        {product.image ? (
          <Image
            src={product.image}
            alt={product.name}
            fill
            className="object-cover"
            sizes="(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 33vw"
            priority={priority}
            unoptimized
          />
        ) : (
          <div className="flex h-full w-full items-center justify-center text-slate-300">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              className="h-12 w-12"
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
    </>
  );

  const cardClasses = "overflow-hidden transition hover:-translate-y-1 hover:shadow-lg h-full";
  const linkCardClasses = "cursor-pointer";

  if (href) {
    return (
      <Link href={href}>
        <Card className={`${cardClasses} ${linkCardClasses}`}>
          {cardContent}
        </Card>
      </Link>
    );
  }

  return (
    <Card className={cardClasses}>
      {cardContent}
    </Card>
  );
};