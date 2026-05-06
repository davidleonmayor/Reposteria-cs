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
