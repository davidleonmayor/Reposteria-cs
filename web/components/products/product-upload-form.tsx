"use client";

import { useState } from "react";
import Image from "next/image";

import { toast } from "sonner";
import { ImagePlus, Save } from "lucide-react";

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

import { PRODUCT_CATEGORIES } from "@/moks/constants";

export const ProductUploadForm = () => {
  const [imagePreview, setImagePreview] = useState<string | null>(null);

  const handleImageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      setImagePreview(URL.createObjectURL(file));
    }
  };

  const handleSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    toast.success("Producto guardado.");
  };

  const handleReset = () => {
    setImagePreview(null);
  };

  return (
    <form
      onSubmit={handleSubmit}
      onReset={handleReset}
      className="grid gap-6 md:grid-cols-[280px_1fr]"
    >
      <div className="flex flex-col gap-2">
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
              <span className="text-sm font-medium">
                Click para subir imagen
              </span>
              <span className="text-xs">PNG, JPG o WEBP</span>
            </div>
          )}
          <input
            id="product-image"
            name="image"
            type="file"
            accept="image/*"
            className="sr-only"
            onChange={handleImageChange}
          />
        </label>
      </div>

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
          <Label htmlFor="product-description">Descripcion</Label>
          <Textarea
            id="product-description"
            name="description"
            placeholder="Bizcocho humedo con cobertura de cacao..."
            rows={4}
          />
        </div>

        <div className="grid gap-4 sm:grid-cols-2">
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
            <Label htmlFor="product-category">Categoria</Label>
            <Select name="category">
              <SelectTrigger id="product-category">
                <SelectValue placeholder="Selecciona una categoria" />
              </SelectTrigger>
              <SelectContent>
                {PRODUCT_CATEGORIES.map((category) => (
                  <SelectItem key={category} value={category}>
                    {category}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </div>

        <div className="flex flex-col-reverse gap-2 pt-2 sm:flex-row sm:justify-end">
          <Button type="reset" variant="default">
            Cancelar
          </Button>
          <Button type="submit" variant="secondary">
            <Save className="mr-2 h-4 w-4" />
            Guardar producto
          </Button>
        </div>
      </div>
    </form>
  );
};
