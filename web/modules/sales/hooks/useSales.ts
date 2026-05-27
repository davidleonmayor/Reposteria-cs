'use client';

import { useState, useEffect, useCallback } from 'react';
import { getSalesRequest } from '../service';
import type { Sale } from '../types';

export function useSales() {
  const [sales, setSales] = useState<Sale[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(() => {
    setLoading(true);
    setError(null);
    getSalesRequest()
      .then(setSales)
      .catch((err) => setError(err instanceof Error ? err.message : 'Error al cargar ventas'))
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  return { sales, loading, error, reload: load };
}
