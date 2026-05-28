export interface Sale {
  id: number;
  saleDate: string;
  subtotal: number;
  total: number;
  state: string;
  observations?: string | null;
  participants: SaleParticipant[];
  details: SaleDetail[];
}

export interface SaleParticipant {
  id: number;
  personId: number;
  role: string;
  person?: PersonSummary | null;
}

export interface SaleDetail {
  id: number;
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
}

export interface PersonSummary {
  id: number;
  name: string;
  lastName: string;
  email: string;
}

export interface ProductSummary {
  id: number;
  name: string;
  price: number;
}

export interface SaleDetailCreateRequest {
  productId: number;
  quantity: number;
  unitPrice: number;
}

export interface SaleParticipantCreateRequest {
  personId: number;
  role: string;
}

export interface SaleCreateRequest {
  state: string;
  observations?: string;
  saleDate?: string;
  participants: SaleParticipantCreateRequest[];
  details: SaleDetailCreateRequest[];
}
