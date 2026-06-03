import { customFetch, uploadFile } from '../../config/fetch';
import type { Product, ProductCreateData, ProductUpdateData } from './types';

export async function getProductsRequest(): Promise<Product[]> {
  return customFetch<Product[]>('/api/Product');
}

export async function getProductByIdRequest(id: number): Promise<Product> {
  return customFetch<Product>(`/api/Product/${id}`);
}

export async function createProductRequest(data: ProductCreateData): Promise<Product> {
  return customFetch<Product>('/api/Product', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}

export async function updateProductRequest(id: number, data: ProductUpdateData): Promise<void> {
  return customFetch<void>(`/api/Product/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  });
}

export async function deleteProductRequest(id: number): Promise<void> {
  return customFetch<void>(`/api/Product/${id}`, {
    method: 'DELETE',
  });
}

export async function uploadProductImageRequest(id: number, file: File): Promise<void> {
  return uploadFile(`/api/Product/${id}/image`, file);
}
