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
  category: ProductCategory;
}
