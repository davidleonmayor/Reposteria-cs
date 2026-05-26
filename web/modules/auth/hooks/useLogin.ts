'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { loginRequest } from '../service';
import { useAuthStore } from '@/store/use-auth';
import type { LoginRequest } from '../types';

export function useLogin() {
  const router = useRouter();
  const setUser = useAuthStore((state) => state.setUser);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function login(credentials: LoginRequest) {
    setLoading(true);
    setError(null);
    try {
      const response = await loginRequest(credentials);
      setUser({ token: response.token, expiresAt: response.expiresAt });
      router.push('/dashboard/home');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error desconocido');
    } finally {
      setLoading(false);
    }
  }

  return { login, loading, error };
}
