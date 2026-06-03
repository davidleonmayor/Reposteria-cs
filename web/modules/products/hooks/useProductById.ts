'use client';

import { useState, useEffect } from 'react';
import { getProductByIdRequest } from '../service';
import type { Product } from '../types';

export function useProductById(id: number) {
  const [product, setProduct] = useState<Product | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    setLoading(true);
    setError(null);
    getProductByIdRequest(id)
      .then(setProduct)
      .catch((err) => setError(err instanceof Error ? err.message : 'Error al cargar el producto'))
      .finally(() => setLoading(false));
  }, [id]);

  return { product, loading, error };
}
