import { customFetch } from '../../config/fetch';
import type { Sale, SaleCreateRequest } from './types';

export async function getSalesRequest(): Promise<Sale[]> {
  return customFetch<Sale[]>('/api/sales');
}

export async function getSaleByIdRequest(id: number): Promise<Sale> {
  return customFetch<Sale>(`/api/sales/${id}`);
}

export async function createSaleRequest(data: SaleCreateRequest): Promise<Sale> {
  return customFetch<Sale>('/api/sales', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}
