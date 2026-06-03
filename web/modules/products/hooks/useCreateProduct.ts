'use client';

import { useState } from 'react';
import { createProductRequest, uploadProductImageRequest } from '../service';
import type { Product, ProductCreateData } from '../types';

export function useCreateProduct() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const createProduct = async (
    data: ProductCreateData,
    imageFile?: File | null,
  ): Promise<Product> => {
    setLoading(true);
    setError(null);
    try {
      const product = await createProductRequest(data);
      if (imageFile) {
        await uploadProductImageRequest(product.id, imageFile);
      }
      return product;
    } catch (err) {
      const msg = err instanceof Error ? err.message : 'Error al crear el producto';
      setError(msg);
      throw err;
    } finally {
      setLoading(false);
    }
  };

  return { createProduct, loading, error };
}
