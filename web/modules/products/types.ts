export interface ProductCategory {
  id: number;
  name: string;
  description: string;
}

export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: number;
  active: boolean;
  hasImage: boolean;
  category: ProductCategory;
}

export interface ProductCreateData {
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: number;
  active: boolean;
}

export interface ProductUpdateData {
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: number;
  active: boolean;
}
