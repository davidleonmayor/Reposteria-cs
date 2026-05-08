import Image from "next/image";

import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";

const products = [
  {
    id: 1,
    name: "Torta de chocolate",
    description: "Bizcocho húmedo con cobertura de cacao y chips crujientes.",
    price: 28.5,
    category: "Tortas",
    image: "/hero.png",
  },
  {
    id: 2,
    name: "Croissant de mantequilla",
    description: "Hojaldre dorado con capas ligeras y aroma a vainilla.",
    price: 4.2,
    category: "Panadería",
    image: "/mascot.svg",
  },
  {
    id: 3,
    name: "Cheesecake frutos rojos",
    description: "Base crocante, crema suave y mermelada artesanal.",
    price: 22.0,
    category: "Postres",
    image: "/window.svg",
  },
  {
    id: 4,
    name: "Brownie intenso",
    description: "Chocolate 70% cacao con nueces tostadas y sal marina.",
    price: 6.5,
    category: "Bocados",
    image: "/file.svg",
  },
  {
    id: 5,
    name: "Tarta de limón",
    description: "Crema cítrica balanceada con merengue suave y tostado.",
    price: 18.9,
    category: "Tartas",
    image: "/globe.svg",
  },
  {
    id: 6,
    name: "Galletas vainilla",
    description: "Clásicas, mantecosas y perfectas para acompañar café.",
    price: 3.8,
    category: "Galletas",
    image: "/vercel.svg",
  },
];

export default function Home() {
  return (
    <main className="min-h-screen w-full bg-[radial-gradient(circle_at_top,_#f5efe6,_#f8fafc_45%,_#eef2ff_100%)]">
      <section className="mx-auto flex w-full max-w-6xl flex-col gap-8 px-4 py-10 md:px-8">
        <header className="flex flex-col gap-3">
          <p className="text-xs font-semibold uppercase tracking-[0.3em] text-slate-500">
            Seleccion de hoy
          </p>
          <h1 className="text-3xl font-semibold text-slate-900 md:text-4xl">
            Productos destacados de la reposteria
          </h1>
          <p className="max-w-2xl text-base text-slate-500">
            Descubri combinaciones dulces y clasicos de temporada con
            ingredientes frescos, horneados cada manana.
          </p>
        </header>

        <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-3">
          {products.map((product) => (
            <Card
              key={product.id}
              className="overflow-hidden transition hover:-translate-y-1 hover:shadow-lg"
            >
              <div className="relative h-44 w-full">
                <Image
                  src={product.image}
                  alt={product.name}
                  fill
                  className="object-cover"
                  sizes="(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 33vw"
                  priority={product.id <= 2}
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
              <CardContent className="text-sm text-slate-500">
                Ingredientes seleccionados, textura equilibrada y empaque
                seguro.
              </CardContent>
              <CardFooter className="flex items-center justify-between">
                <span className="text-sm font-medium text-slate-600">
                  Precio
                </span>
                <span className="text-lg font-semibold text-slate-900">
                  ${product.price.toFixed(2)}
                </span>
              </CardFooter>
            </Card>
          ))}
        </div>
      </section>
    </main>
  );
}
