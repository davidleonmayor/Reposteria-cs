'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { toast } from 'sonner';
import { registerRequest } from '../service';
import type { RegisterRequest } from '../types';

export function useRegister() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);

  async function register(data: RegisterRequest) {
    setLoading(true);
    try {
      await registerRequest(data);
      toast.success('¡Cuenta creada con éxito! Iniciá sesión.');
      router.push('/login');
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Error al crear la cuenta.');
    } finally {
      setLoading(false);
    }
  }

  return { register, loading };
}
