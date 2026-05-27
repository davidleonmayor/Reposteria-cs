"use client";

import { useState } from "react";

import { toast } from "sonner";

import { Check, ShoppingBag, X } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";

import { useConfirmSaleModal } from "@/store/use-confirm-sale-modal";
import { useCart } from "@/store/use-cart";
import { createSaleRequest } from "@/modules/sales/service";
import { useAuthStore } from "@/store/use-auth";

export function ConfirmSaleModal() {
  const { isOpen, close } = useConfirmSaleModal();
  const items = useCart((state) => state.items);
  const clearCart = useCart((state) => state.clearCart);
  const user = useAuthStore((state) => state.user);

  const [observations, setObservations] = useState("");
  const [loading, setLoading] = useState(false);

  const subtotal = items.reduce(
    (sum, item) => sum + item.unitPrice * item.quantity,
    0,
  );

  const handleConfirm = async () => {
    if (items.length === 0) return;

    setLoading(true);
    try {
      await createSaleRequest({
        state: "Completada",
        observations: observations.trim() || undefined,
        participants: user ? [{ personId: user.personId, role: user.role }] : [],
        details: items.map((item) => ({
          productId: item.id,
          quantity: item.quantity,
          unitPrice: item.unitPrice,
        })),
      });

      toast.success("¡Venta confirmada correctamente!");
      clearCart();
      setObservations("");
      close();
    } catch (err) {
      toast.error(
        err instanceof Error ? err.message : "Error al confirmar la venta.",
      );
    } finally {
      setLoading(false);
    }
  };

  const handleClose = () => {
    if (!loading) {
      setObservations("");
      close();
    }
  };

  return (
    <Dialog open={isOpen} onOpenChange={(v) => !v && handleClose()}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <div className="flex items-center justify-center mb-2">
            <div className="flex h-14 w-14 items-center justify-center rounded-full bg-emerald-100">
              <ShoppingBag className="h-7 w-7 text-emerald-600" />
            </div>
          </div>
          <DialogTitle className="text-center text-xl font-bold">
            Confirmar venta
          </DialogTitle>
          <DialogDescription className="text-center">
            Revisá el resumen antes de confirmar.
          </DialogDescription>
        </DialogHeader>

        {/* Cart summary */}
        <div className="flex flex-col gap-2 rounded-xl border border-slate-100 bg-slate-50 px-4 py-3">
          {items.map((item) => (
            <div
              key={item.id}
              className="flex items-center justify-between text-sm"
            >
              <span className="text-slate-700">
                {item.name}{" "}
                <span className="text-slate-400">× {item.quantity}</span>
              </span>
              <span className="font-medium text-slate-900">
                ${(item.unitPrice * item.quantity).toFixed(2)}
              </span>
            </div>
          ))}
          <div className="mt-1 flex items-center justify-between border-t border-slate-200 pt-2">
            <span className="text-sm font-semibold text-slate-700">Total</span>
            <span className="text-lg font-bold text-slate-900">
              ${subtotal.toFixed(2)}
            </span>
          </div>
        </div>

        {/* Observations */}
        <div className="flex flex-col gap-2">
          <Label htmlFor="sale-obs">Observaciones (opcional)</Label>
          <Textarea
            id="sale-obs"
            value={observations}
            onChange={(e) => setObservations(e.target.value)}
            placeholder="Ej: Pedido para cumpleaños, entregar el viernes..."
            maxLength={1000}
            rows={3}
            disabled={loading}
          />
        </div>

        <DialogFooter className="flex flex-row gap-2">
          <Button
            type="button"
            variant="ghost"
            className="flex-1"
            onClick={handleClose}
            disabled={loading}
          >
            <X className="mr-1 h-4 w-4" />
            Cancelar
          </Button>
          <Button
            type="button"
            variant="secondary"
            className="flex-1"
            onClick={handleConfirm}
            disabled={loading || items.length === 0}
          >
            <Check className="mr-1 h-4 w-4" />
            {loading ? "Confirmando..." : "Confirmar venta"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
