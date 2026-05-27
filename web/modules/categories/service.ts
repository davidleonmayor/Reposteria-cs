import { customFetch } from '../../config/fetch';
import type { Category, CategoryCreateRequest, CategoryUpdateRequest } from './types';

export async function getCategoriesRequest(): Promise<Category[]> {
  return customFetch<Category[]>('/api/category');
}

export async function createCategoryRequest(data: CategoryCreateRequest): Promise<Category> {
  return customFetch<Category>('/api/category', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}

export async function updateCategoryRequest(id: number, data: CategoryUpdateRequest): Promise<void> {
  return customFetch<void>(`/api/category/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  });
}

export async function deleteCategoryRequest(id: number): Promise<void> {
  return customFetch<void>(`/api/category/${id}`, {
    method: 'DELETE',
  });
}
