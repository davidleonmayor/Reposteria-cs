export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  personId: number;
  role: string;
}

export interface AuthUser {
  token: string;
  expiresAt: string;
  personId: number;
  role: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  name: string;
  lastName: string;
  personTypeId: number;
  phone?: string;
  address?: string;
}

export type RegisterResponse = LoginResponse;
