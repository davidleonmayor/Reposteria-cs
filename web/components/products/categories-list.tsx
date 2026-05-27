"use client";

import { useState } from "react";

import { Pencil, Plus, Trash2 } from "lucide-react";
import { toast } from "sonner";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";

import { useCategories } from "@/modules/categories/hooks/useCategories";
import { createCategoryRequest, deleteCategoryRequest, updateCategoryRequest } from "@/modules/categories/service";
import type { Category } from "@/modules/categories/types";

// ─── Form dialog (create / edit) ───────────────────────────────────────────

type FormDialogProps = {
  open: boolean;
  category: Category | null;
  onClose: () => void;
  onSaved: () => void;
};

const FormDialog = ({ open, category, onClose, onSaved }: FormDialogProps) => {
  const isEditing = category !== null;
  const [name, setName] = useState(category?.name ?? "");
  const [description, setDescription] = useState(category?.description ?? "");
  const [saving, setSaving] = useState(false);

  const handleOpenChange = (value: boolean) => {
    if (!value) onClose();
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!name.trim() || !description.trim()) {
      toast.error("Completá todos los campos.");
      return;
    }
    setSaving(true);
    try {
      if (isEditing) {
        await updateCategoryRequest(category.id, { name: name.trim(), description: description.trim() });
        toast.success("Categoría actualizada.");
      } else {
        await createCategoryRequest({ name: name.trim(), description: description.trim() });
        toast.success("Categoría creada.");
      }
      onSaved();
      onClose();
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Error al guardar.");
    } finally {
      setSaving(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>
            {isEditing ? "Editar categoría" : "Nueva categoría"}
          </DialogTitle>
        </DialogHeader>

        <form onSubmit={handleSubmit} className="flex flex-col gap-4 py-2">
          <div className="flex flex-col gap-2">
            <Label htmlFor="cat-name">Nombre</Label>
            <Input
              id="cat-name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="Ej: Tortas"
              maxLength={100}
              disabled={saving}
            />
          </div>

          <div className="flex flex-col gap-2">
            <Label htmlFor="cat-desc">Descripción</Label>
            <Textarea
              id="cat-desc"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Descripción breve de la categoría"
              maxLength={500}
              rows={3}
              disabled={saving}
            />
          </div>

          <DialogFooter>
            <Button
              type="button"
              variant="ghost"
              onClick={onClose}
              disabled={saving}
            >
              Cancelar
            </Button>
            <Button type="submit" variant="secondary" disabled={saving}>
              {saving ? "Guardando..." : "Guardar"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
};

// ─── Delete confirm dialog ──────────────────────────────────────────────────

type DeleteDialogProps = {
  open: boolean;
  category: Category | null;
  onClose: () => void;
  onDeleted: () => void;
};

const DeleteDialog = ({
  open,
  category,
  onClose,
  onDeleted,
}: DeleteDialogProps) => {
  const [deleting, setDeleting] = useState(false);

  const handleConfirm = async () => {
    if (!category) return;
    setDeleting(true);
    try {
      await deleteCategoryRequest(category.id);
      toast.success("Categoría eliminada.");
      onDeleted();
      onClose();
    } catch (err) {
      toast.error(err instanceof Error ? err.message : "Error al eliminar.");
    } finally {
      setDeleting(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={(v) => !v && onClose()}>
      <DialogContent className="sm:max-w-sm">
        <DialogHeader>
          <DialogTitle>¿Eliminar categoría?</DialogTitle>
        </DialogHeader>

        <p className="text-sm text-slate-500">
          Estás por eliminar{" "}
          <span className="font-semibold text-slate-700">{category?.name}</span>
          . Esta acción no se puede deshacer.
        </p>

        <DialogFooter>
          <Button
            type="button"
            variant="ghost"
            onClick={onClose}
            disabled={deleting}
          >
            Cancelar
          </Button>
          <Button
            type="button"
            variant="danger"
            onClick={handleConfirm}
            disabled={deleting}
          >
            {deleting ? "Eliminando..." : "Eliminar"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

// ─── Main list ──────────────────────────────────────────────────────────────

export const CategoriesList = () => {
  const { categories, loading, error, reload } = useCategories();
  const [formTarget, setFormTarget] = useState<Category | null | undefined>(
    undefined,
  ); // undefined = closed, null = new, Category = edit
  const [deleteTarget, setDeleteTarget] = useState<Category | null>(null);

  if (loading) {
    return <p className="text-sm text-slate-500">Cargando categorías...</p>;
  }

  if (error) {
    return <p className="text-sm text-red-500">{error}</p>;
  }

  return (
    <div className="flex flex-col gap-4">
      {/* Header row */}
      <div className="flex items-center justify-between">
        <p className="text-sm text-slate-500">
          {categories.length} categoría{categories.length !== 1 ? "s" : ""}
        </p>
        <Button
          size="sm"
          variant="secondary"
          onClick={() => setFormTarget(null)}
        >
          <Plus className="mr-1 h-4 w-4" />
          Nueva categoría
        </Button>
      </div>

      {/* Empty state */}
      {categories.length === 0 && (
        <div className="flex h-40 items-center justify-center rounded-xl border border-dashed border-slate-300 bg-slate-50">
          <p className="text-sm text-slate-500">
            No hay categorías aún. ¡Creá la primera!
          </p>
        </div>
      )}

      {/* Category rows */}
      <div className="flex flex-col divide-y divide-slate-100 rounded-xl border border-slate-200 bg-white">
        {categories.map((cat) => (
          <div
            key={cat.id}
            className="flex items-center justify-between gap-4 px-4 py-3"
          >
            <div className="flex min-w-0 flex-col gap-0.5">
              <span className="truncate text-sm font-semibold text-slate-800">
                {cat.name}
              </span>
              <span className="truncate text-xs text-slate-500">
                {cat.description}
              </span>
            </div>

            <div className="flex shrink-0 items-center gap-2">
              <Button
                size="icon"
                variant="ghost"
                className="h-8 w-8 text-slate-500 hover:text-slate-800"
                onClick={() => setFormTarget(cat)}
                aria-label={`Editar ${cat.name}`}
              >
                <Pencil className="h-4 w-4" />
              </Button>
              <Button
                size="icon"
                variant="ghost"
                className="h-8 w-8 text-rose-400 hover:bg-rose-50 hover:text-rose-600"
                onClick={() => setDeleteTarget(cat)}
                aria-label={`Eliminar ${cat.name}`}
              >
                <Trash2 className="h-4 w-4" />
              </Button>
            </div>
          </div>
        ))}
      </div>

      {/* Dialogs */}
      <FormDialog
        open={formTarget !== undefined}
        category={formTarget ?? null}
        onClose={() => setFormTarget(undefined)}
        onSaved={reload}
      />

      <DeleteDialog
        open={deleteTarget !== null}
        category={deleteTarget}
        onClose={() => setDeleteTarget(null)}
        onDeleted={reload}
      />
    </div>
  );
};
