'use client';

import { useState, useEffect } from 'react';
import { getProductsRequest } from '../service';
import type { Product } from '../types';

export function useProducts() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getProductsRequest()
      .then(setProducts)
      .catch((err) => setError(err instanceof Error ? err.message : 'Error al cargar productos'))
      .finally(() => setLoading(false));
  }, []);

  return { products, loading, error };
}
