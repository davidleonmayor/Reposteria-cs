'use client';

import { useState } from 'react';
import { updateProductRequest, uploadProductImageRequest } from '../service';
import type { ProductUpdateData } from '../types';

export function useUpdateProduct() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const updateProduct = async (
    id: number,
    data: ProductUpdateData,
    imageFile?: File | null,
  ): Promise<void> => {
    setLoading(true);
    setError(null);
    try {
      await updateProductRequest(id, data);
      if (imageFile) {
        await uploadProductImageRequest(id, imageFile);
      }
    } catch (err) {
      const msg = err instanceof Error ? err.message : 'Error al actualizar el producto';
      setError(msg);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  return { updateProduct, loading, error };
}
