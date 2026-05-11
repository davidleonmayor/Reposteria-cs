import { Home, Package, ShoppingCart } from "lucide-react";

interface ILink {
  href: string;
  label: string;
  icon: React.ReactNode;
}

export const LINKS: ILink[] = [
  { href: "/dashboard/home", label: "Inicio", icon: <Home /> },
  { href: "/dashboard/products", label: "Productos", icon: <Package /> },
  { href: "/dashboard/sells", label: "Ventas", icon: <ShoppingCart /> },
];

export type Product = {
  id: number;
  name: string;
  description: string;
  price: number;
  category: string;
  image: string;
};

export const PRODUCTS: Product[] = [
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

export const PRODUCT_CATEGORIES = [
  "Tortas",
  "Panadería",
  "Postres",
  "Bocados",
  "Tartas",
  "Galletas",
];
