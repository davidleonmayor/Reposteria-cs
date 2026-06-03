"use client";

import { useMemo, useState } from "react";

import { Loader2, Search } from "lucide-react";
import { toast } from "sonner";

import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";

import { ProductCardDisplay } from "@/components/products/product-card-display";
import { useProducts } from "@/modules/products/hooks/useProducts";
import { useDeleteProduct } from "@/modules/products/hooks/useDeleteProduct";

const ALL_CATEGORIES = "all";

export const ProductsList = () => {
  const { products, loading, error, reload } = useProducts();
  const { deleteProduct, loading: deleting } = useDeleteProduct();

  const [search, setSearch] = useState("");
  const [category, setCategory] = useState<string>(ALL_CATEGORIES);
  const [pendingDeleteId, setPendingDeleteId] = useState<number | null>(null);

  const categories = useMemo(
    () => [...new Set(products.map((p) => p.category.name))].sort(),
    [products],
  );

  const filtered = useMemo(() => {
    const query = search.trim().toLowerCase();
    return products.filter((product) => {
      const matchSearch =
        query === "" || product.name.toLowerCase().includes(query);
      const matchCategory =
        category === ALL_CATEGORIES || product.category.name === category;
      return matchSearch && matchCategory;
    });
  }, [products, search, category]);

  const handleDeleteRequest = (id: number) => {
    setPendingDeleteId(id);
  };

  const handleDeleteConfirm = async () => {
    if (pendingDeleteId === null) return;
    const id = pendingDeleteId;
    setPendingDeleteId(null);
    try {
      await deleteProduct(id);
      toast.success("Producto eliminado.");
      reload();
    } catch {
      toast.error("No se pudo eliminar el producto.");
    }
  };

  const handleDeleteCancel = () => {
    setPendingDeleteId(null);
  };

  if (loading) {
    return <p className="text-sm text-slate-500">Cargando productos...</p>;
  }

  if (error) {
    return <p className="text-sm text-red-500">{error}</p>;
  }

  const pendingProduct = products.find((p) => p.id === pendingDeleteId);

  return (
    <>
      <div className="flex flex-col gap-6">
        <div className="flex flex-col gap-3 rounded-xl border border-slate-200 bg-white p-3 sm:flex-row sm:items-center">
          <div className="relative flex-1">
            <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-400" />
            <Input
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Buscar por nombre..."
              className="h-10 pl-9"
            />
          </div>

          <div className="sm:w-56">
            <Select value={category} onValueChange={setCategory}>
              <SelectTrigger className="h-10">
                <SelectValue placeholder="Categoria" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value={ALL_CATEGORIES}>
                  Todas las categorias
                </SelectItem>
                {categories.map((cat) => (
                  <SelectItem key={cat} value={cat}>
                    {cat}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </div>

        {filtered.length === 0 ? (
          <div className="flex h-40 items-center justify-center rounded-xl border border-dashed border-slate-300 bg-slate-50">
            <p className="text-sm text-slate-500">
              No se encontraron productos con esos filtros.
            </p>
          </div>
        ) : (
          <div className="grid gap-5 sm:grid-cols-2 lg:grid-cols-3">
            {filtered.map((product) => (
              <ProductCardDisplay
                key={product.id}
                product={{
                  id: product.id,
                  name: product.name,
                  description: product.description,
                  price: product.price,
                  category: product.category.name,
                  image: product.hasImage
                    ? `/api/product/${product.id}/image`
                    : null,
                }}
                href={`/dashboard/products/${product.id}`}
                onDelete={handleDeleteRequest}
              />
            ))}
          </div>
        )}
      </div>

      <AlertDialog open={pendingDeleteId !== null}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>¿Eliminar producto?</AlertDialogTitle>
            <AlertDialogDescription>
              Vas a eliminar{" "}
              <span className="font-semibold text-slate-900">
                {pendingProduct?.name}
              </span>
              . Esta acción no se puede deshacer.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel onClick={handleDeleteCancel} disabled={deleting}>
              Cancelar
            </AlertDialogCancel>
            <AlertDialogAction
              onClick={handleDeleteConfirm}
              disabled={deleting}
              className="bg-rose-600 hover:bg-rose-700 focus:ring-rose-600"
            >
              {deleting ? (
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              ) : null}
              Eliminar
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
};
