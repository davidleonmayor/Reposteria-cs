'use client';

import { useState } from 'react';
import { deleteProductRequest } from '../service';

export function useDeleteProduct() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const deleteProduct = async (id: number): Promise<void> => {
    setLoading(true);
    setError(null);
    try {
      await deleteProductRequest(id);
    } catch (err) {
      const msg = err instanceof Error ? err.message : 'Error al eliminar el producto';
      setError(msg);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  return { deleteProduct, loading, error };
}
