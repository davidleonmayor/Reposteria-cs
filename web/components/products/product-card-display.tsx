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
  image: string;
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