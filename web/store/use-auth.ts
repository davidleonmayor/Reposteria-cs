import { create } from 'zustand';
import type { AuthUser } from '@/modules/auth/types';

type AuthState = {
  user: AuthUser | null;
  isAuthenticated: boolean;
  setUser: (user: AuthUser | null) => void;
  logout: () => void;
};

// TODO: remove hardcoded user — for development only
const DEV_USER: AuthUser = {
  token: 'dev-token',
  expiresAt: '2099-01-01T00:00:00Z',
};

export const useAuthStore = create<AuthState>((set) => ({
  user: DEV_USER,
  isAuthenticated: true,
  setUser: (user) => set({ user, isAuthenticated: user !== null }),
  logout: () => set({ user: null, isAuthenticated: false }),
}));
