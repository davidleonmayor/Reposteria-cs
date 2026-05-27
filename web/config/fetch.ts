import { useAuthStore } from '@/store/use-auth';

export class FetchError extends Error {
  constructor(
    public status: number,
    message: string,
    public data?: unknown,
  ) {
    super(message);
    this.name = "FetchError";
  }
}

export interface CustomRequestInit extends RequestInit {
  timeout?: number;
  params?: Record<string, string | number | boolean | undefined>;
}

const DEFAULT_TIMEOUT = 10000;
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "";

export async function customFetch<T = unknown>(
  endpoint: string,
  options: CustomRequestInit = {},
): Promise<T> {
  const {
    timeout = DEFAULT_TIMEOUT,
    params,
    headers,
    ...restOptions
  } = options;

  // 1. Construir URL base
  let url = endpoint.startsWith("http")
    ? endpoint
    : `${API_BASE_URL}${endpoint}`;

  // 2. Agregar Query Params si existen
  if (params) {
    const searchParams = new URLSearchParams();
    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined) searchParams.append(key, String(value));
    });

    const queryString = searchParams.toString();
    if (queryString) {
      url += (url.includes("?") ? "&" : "?") + queryString;
    }
  }

  // 3. Configurar Timeout
  const controller = new AbortController();
  const id = setTimeout(() => controller.abort(), timeout);

  // 3.5. Read auth token from store (works outside React — Zustand getState())
  const token = useAuthStore.getState().user?.token;

  // 4. Configurar opciones finales (mezclando headers por defecto con los recibidos)
  const finalOptions: RequestInit = {
    ...restOptions,
    headers: {
      "Content-Type": "application/json",
      Accept: "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...headers,
    },
    signal: controller.signal,
  };

  try {
    const response = await fetch(url, finalOptions);
    clearTimeout(id);

    // Validación estricta de errores HTTP
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new FetchError(
        response.status,
        errorData.detail ||
          errorData.message ||
          errorData.title ||
          response.statusText ||
          "Ocurrió un error en la petición.",
        errorData,
      );
    }

    // Retorno temprano para "No Content"
    if (response.status === 204) {
      return null as T;
    }

    // Parseo inteligente de la respuesta
    const contentType = response.headers.get("content-type");
    if (contentType?.includes("application/json")) {
      return (await response.json()) as T;
    }

    return (await response.text()) as unknown as T;
  } catch (error) {
    clearTimeout(id);

    // Validar específicamente el timeout
    if (error instanceof Error && error.name === "AbortError") {
      throw new Error(`La petición excedió el tiempo límite de ${timeout}ms`);
    }

    // Propagar el error original (FetchError u otros)
    throw error;
  }
}
