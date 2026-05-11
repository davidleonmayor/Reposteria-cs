import {
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from "@/components/ui/tabs";

import { ProductUploadForm } from "@/components/products/product-upload-form";
import { ProductsList } from "@/components/products/products-list";

export default function Products() {
  return (
    <main className="min-h-screen w-full">
      <section className="mx-auto flex w-full max-w-6xl flex-col gap-8 px-4 py-10 md:px-8">
        <header className="flex flex-col gap-2">
          <h1 className="text-3xl font-semibold text-slate-900 md:text-4xl">
            Productos
          </h1>
          <p className="text-base text-slate-500">
            Gestiona el catalogo de la reposteria: agrega nuevos productos o
            revisa los existentes.
          </p>
        </header>

        <Tabs defaultValue="todos" className="flex flex-col gap-6">
          <TabsList>
            <TabsTrigger value="carga">Carga</TabsTrigger>
            <TabsTrigger value="todos">Todos</TabsTrigger>
          </TabsList>

          <TabsContent value="carga">
            <div className="rounded-xl border border-slate-200 bg-white p-6">
              <ProductUploadForm />
            </div>
          </TabsContent>

          <TabsContent value="todos">
            <ProductsList />
          </TabsContent>
        </Tabs>
      </section>
    </main>
  );
}
