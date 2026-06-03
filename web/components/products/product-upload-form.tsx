"use client";

import { useRef, useState } from "react";
import Image from "next/image";

import { toast } from "sonner";
import { ImagePlus, Loader2, Save, X } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { useCategories } from "@/modules/categories/hooks/useCategories";
import { useCreateProduct } from "@/modules/products/hooks/useCreateProduct";

export const ProductUploadForm = () => {
  const { categories } = useCategories();
  const { createProduct, loading } = useCreateProduct();

  const [imageFile, setImageFile] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string | null>(null);
  const [categoryId, setCategoryId] = useState<string>("");
  const formRef = useRef<HTMLFormElement>(null);

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

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const fd = new FormData(event.currentTarget);

    try {
      await createProduct(
        {
          name: fd.get("name") as string,
          description: (fd.get("description") as string) ?? "",
          price: Number(fd.get("price")),
          stock: Number(fd.get("stock") ?? 0),
          categoryId: Number(categoryId),
          active: true,
        },
        imageFile,
      );
      toast.success("Producto creado correctamente.");
      formRef.current?.reset();
      setCategoryId("");
      setImageFile(null);
      setImagePreview(null);
    } catch {
      toast.error("No se pudo crear el producto.");
    }
  };

  return (
    <form
      ref={formRef}
      onSubmit={handleSubmit}
      className="grid gap-6 md:grid-cols-[280px_1fr]"
    >
      {/* Imagen */}
      <div className="flex flex-col gap-2">
        <Label htmlFor="product-image">Imagen del producto</Label>
        <label
          htmlFor="product-image"
          className="relative flex aspect-square w-full cursor-pointer items-center justify-center overflow-hidden rounded-xl border-2 border-dashed border-slate-300 bg-slate-50 transition hover:border-emerald-500 hover:bg-emerald-50/40"
        >
          {imagePreview ? (
            <Image
              src={imagePreview}
              alt="Vista previa"
              fill
              className="object-cover"
              unoptimized
            />
          ) : (
            <div className="flex flex-col items-center gap-2 p-4 text-center text-slate-400">
              <ImagePlus className="h-10 w-10" />
              <span className="text-sm font-medium">Click para subir imagen</span>
              <span className="text-xs">PNG, JPG o WEBP · Máx 5 MB</span>
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
      </div>

      {/* Campos */}
      <div className="flex flex-col gap-4">
        <div className="flex flex-col gap-2">
          <Label htmlFor="product-name">Nombre</Label>
          <Input
            id="product-name"
            name="name"
            placeholder="Torta de chocolate"
            required
          />
        </div>

        <div className="flex flex-col gap-2">
          <Label htmlFor="product-description">Descripción</Label>
          <Textarea
            id="product-description"
            name="description"
            placeholder="Bizcocho húmedo con cobertura de cacao..."
            rows={3}
          />
        </div>

        <div className="grid gap-4 sm:grid-cols-3">
          <div className="flex flex-col gap-2">
            <Label htmlFor="product-price">Precio</Label>
            <Input
              id="product-price"
              name="price"
              type="number"
              step="0.01"
              min="0"
              placeholder="0.00"
              required
            />
          </div>

          <div className="flex flex-col gap-2">
            <Label htmlFor="product-stock">Stock</Label>
            <Input
              id="product-stock"
              name="stock"
              type="number"
              min="0"
              placeholder="0"
            />
          </div>

          <div className="flex flex-col gap-2">
            <Label htmlFor="product-category">Categoría</Label>
            <Select value={categoryId} onValueChange={setCategoryId} required>
              <SelectTrigger id="product-category">
                <SelectValue placeholder="Selecciona" />
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

        <div className="flex flex-col-reverse gap-2 pt-2 sm:flex-row sm:justify-end">
          <Button
            type="reset"
            variant="ghost"
            disabled={loading}
            onClick={() => {
              setCategoryId("");
              setImageFile(null);
              setImagePreview(null);
            }}
          >
            Cancelar
          </Button>
          <Button type="submit" disabled={loading}>
            {loading ? (
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
            ) : (
              <Save className="mr-2 h-4 w-4" />
            )}
            Guardar producto
          </Button>
        </div>
      </div>
    </form>
  );
};
