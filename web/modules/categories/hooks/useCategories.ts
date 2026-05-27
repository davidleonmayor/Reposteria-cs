'use client';

import { useState, useEffect, useCallback } from 'react';
import { getCategoriesRequest } from '../service';
import type { Category } from '../types';

export function useCategories() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(() => {
    setLoading(true);
    setError(null);
    getCategoriesRequest()
      .then(setCategories)
      .catch((err) => setError(err instanceof Error ? err.message : 'Error al cargar categorías'))
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  return { categories, loading, error, reload: load };
}
