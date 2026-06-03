"use client";

import { useState, useEffect, useCallback } from "react";
import { getProductsRequest } from "../service";
import type { Product } from "../types";

export function useProducts() {
  const [products, setProducts] = useState<Product[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(() => {
    setLoading(true);
    setError(null);
    getProductsRequest()
      .then(setProducts)
      .catch((err) =>
        setError(
          err instanceof Error ? err.message : "Error al cargar productos",
        ),
      )
      .finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  return { products, loading, error, reload: load };
}
