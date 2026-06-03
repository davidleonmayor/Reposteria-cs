"use client";

import Image from "next/image";
import { useParams, useRouter } from "next/navigation";
import { useEffect, useState } from "react";

import { toast } from "sonner";
import { ArrowLeft, ImagePlus, Loader2, Save, X } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";

import { useProductById } from "@/modules/products/hooks/useProductById";
import { useUpdateProduct } from "@/modules/products/hooks/useUpdateProduct";
import { useCategories } from "@/modules/categories/hooks/useCategories";

export default function ProductDetailPage() {
  const params = useParams();
  const router = useRouter();
  const productId = Number(params.id);

  const { product, loading: loadingProduct } = useProductById(productId);
  const { updateProduct, loading: saving } = useUpdateProduct();
  const { categories } = useCategories();

  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [price, setPrice] = useState(0);
  const [stock, setStock] = useState(0);
  const [categoryId, setCategoryId] = useState<string>("");
  const [active, setActive] = useState(true);

  const [imageFile, setImageFile] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string | null>(null);

  useEffect(() => {
    if (!product) return;
    setName(product.name);
    setDescription(product.description);
    setPrice(product.price);
    setStock(product.stock);
    setCategoryId(String(product.categoryId));
    setActive(product.active);
    setImagePreview(product.hasImage ? `/api/product/${product.id}/image` : null);
  }, [product]);

  const handleImageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;
    setImageFile(file);
    setImagePreview(URL.createObjectURL(file));
  };

  const handleRemoveImage = () => {
    setImageFile(null);
    setImagePreview(null);
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await updateProduct(
        productId,
        {
          name,
          description,
          price,
          stock,
          categoryId: Number(categoryId),
          active,
        },
        imageFile,
      );
      toast.success("Producto actualizado.");
      router.push("/dashboard/products");
    } catch {
      toast.error("No se pudo actualizar el producto.");
    }
  };

  if (loadingProduct) {
    return (
      <section className="mx-auto flex w-full max-w-6xl flex-col gap-8 px-4 py-10 md:px-8">
        <div className="flex items-center gap-2 text-slate-500">
          <Loader2 className="h-5 w-5 animate-spin" />
          <span className="text-sm">Cargando producto...</span>
        </div>
      </section>
    );
  }

  if (!product) {
    return (
      <section className="mx-auto flex w-full max-w-6xl flex-col gap-8 px-4 py-10 md:px-8">
        <div className="rounded-xl border border-rose-200 bg-rose-50 p-6">
          <p className="text-rose-600">Producto no encontrado.</p>
          <Button
            variant="ghost"
            onClick={() => router.push("/dashboard/products")}
            className="mt-4"
          >
            <ArrowLeft className="mr-2 h-4 w-4" />
            Volver a productos
          </Button>
        </div>
      </section>
    );
  }

  return (
    <section className="mx-auto flex w-full max-w-6xl flex-col gap-8 px-4 py-10 md:px-8">
      <header className="flex flex-col gap-2">
        <Button
          variant="ghost"
          onClick={() => router.push("/dashboard/products")}
          className="mb-2 w-fit"
        >
          <ArrowLeft className="mr-2 h-4 w-4" />
          Volver a productos
        </Button>
        <h1 className="text-3xl font-semibold text-slate-900 md:text-4xl">
          Editar Producto
        </h1>
        <p className="text-base text-slate-500">
          Modifica los datos del producto. Los cambios se guardarán al hacer clic en
          &quot;Guardar&quot;.
        </p>
      </header>

      <div className="rounded-xl border border-slate-200 bg-white p-6">
        <div className="grid gap-8 lg:grid-cols-[300px_1fr]">
          {/* Imagen */}
          <div className="flex flex-col gap-3">
            <Label htmlFor="product-image">Imagen del producto</Label>
            <label
              htmlFor="product-image"
              className="relative flex aspect-square w-full cursor-pointer items-center justify-center overflow-hidden rounded-xl border-2 border-dashed border-slate-300 bg-slate-50 transition hover:border-emerald-500 hover:bg-emerald-50/40"
            >
              {imagePreview ? (
                <Image
                  src={imagePreview}
                  alt="Vista previa del producto"
                  fill
                  className="object-cover"
                  unoptimized
                />
              ) : (
                <div className="flex flex-col items-center gap-2 p-4 text-center text-slate-400">
                  <ImagePlus className="h-10 w-10" />
                  <span className="text-sm font-medium">Click para subir imagen</span>
                  <span className="text-xs">PNG, JPG o WEBP</span>
                </div>
              )}
              {imagePreview && (
                <button
                  type="button"
                  onClick={(e) => {
                    e.preventDefault();
                    e.stopPropagation();
                    handleRemoveImage();
                  }}
                  className="absolute right-2 top-2 flex h-8 w-8 items-center justify-center rounded-full bg-rose-500 text-white shadow-md transition hover:bg-rose-600"
                  aria-label="Quitar imagen"
                >
                  <X className="h-4 w-4" />
                </button>
              )}
              <input
                id="product-image"
                type="file"
                accept="image/*"
                className="sr-only"
                onChange={handleImageChange}
              />
            </label>
            <p className="text-sm text-slate-500">PNG, JPG o WEBP · Máx 5 MB.</p>
          </div>

          {/* Formulario */}
          <form className="flex flex-col gap-5" onSubmit={handleSave}>
            <div className="grid gap-5 sm:grid-cols-2">
              <div className="flex flex-col gap-2">
                <Label htmlFor="name">Nombre</Label>
                <Input
                  id="name"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  placeholder="Nombre del producto"
                  required
                />
              </div>

              <div className="flex flex-col gap-2">
                <Label htmlFor="category">Categoría</Label>
                <Select value={categoryId} onValueChange={setCategoryId}>
                  <SelectTrigger id="category">
                    <SelectValue placeholder="Selecciona una categoría" />
                  </SelectTrigger>
                  <SelectContent>
                    {categories.map((cat) => (
                      <SelectItem key={cat.id} value={String(cat.id)}>
                        {cat.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>
            </div>

            <div className="flex flex-col gap-2">
              <Label htmlFor="description">Descripción</Label>
              <Textarea
                id="description"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                placeholder="Descripción del producto"
                rows={3}
              />
            </div>

            <div className="grid gap-5 sm:grid-cols-2">
              <div className="flex flex-col gap-2">
                <Label htmlFor="price">Precio</Label>
                <Input
                  id="price"
                  type="number"
                  step="0.01"
                  min="0"
                  value={price}
                  onChange={(e) => setPrice(Number(e.target.value))}
                  placeholder="0.00"
                />
              </div>

              <div className="flex flex-col gap-2">
                <Label htmlFor="stock">Stock</Label>
                <Input
                  id="stock"
                  type="number"
                  min="0"
                  value={stock}
                  onChange={(e) => setStock(Number(e.target.value))}
                  placeholder="0"
                />
              </div>
            </div>

            <div className="flex justify-end gap-3 pt-4">
              <Button
                type="button"
                variant="ghost"
                disabled={saving}
                onClick={() => router.push("/dashboard/products")}
              >
                Cancelar
              </Button>
              <Button type="submit" disabled={saving}>
                {saving ? (
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                ) : (
                  <Save className="mr-2 h-4 w-4" />
                )}
                Guardar
              </Button>
            </div>
          </form>
        </div>
      </div>
    </section>
  );
}
