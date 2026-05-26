import { customFetch, FetchError } from '../../config/fetch';
import type { LoginRequest, LoginResponse, RegisterRequest, RegisterResponse } from './types';

export async function loginRequest(credentials: LoginRequest): Promise<LoginResponse> {
  try {
    return await customFetch<LoginResponse>('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify(credentials),
    });
  } catch (error) {
    if (error instanceof FetchError) {
      if (error.status === 401 || error.status === 403) {
        throw new Error('Credenciales inválidas. Verificá tu correo y contraseña.');
      }
      throw new Error(error.message || 'Ocurrió un error en la autenticación.');
    }
    throw new Error('Error de conexión. Intentá de nuevo más tarde.');
  }
}

export async function registerRequest(data: RegisterRequest): Promise<RegisterResponse> {
  try {
    return await customFetch<RegisterResponse>('/api/auth/register', {
      method: 'POST',
      body: JSON.stringify(data),
    });
  } catch (error) {
    if (error instanceof FetchError) {
      if (error.status === 409) {
        throw new Error('El correo ya está registrado.');
      }
      throw new Error(error.message || 'Ocurrió un error al crear la cuenta.');
    }
    throw new Error('Error de conexión. Intentá de nuevo más tarde.');
  }
}
