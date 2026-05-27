import { customFetch } from '../../config/fetch';
import type { Product } from './types';

export async function getProductsRequest(): Promise<Product[]> {
  return customFetch<Product[]>('/api/Product');
}
