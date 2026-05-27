export interface Category {
  id: number;
  name: string;
  description: string;
}

export interface CategoryCreateRequest {
  name: string;
  description: string;
}

export type CategoryUpdateRequest = CategoryCreateRequest;
