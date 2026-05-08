"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { z } from "zod";
import { toast } from "sonner";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import {
  Form,
  FormField,
  FormControl,
  FormLabel,
  FormItem,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardDescription,
} from "@/components/ui/card";

const signupSchema = z
  .object({
    email: z
      .string()
      .min(1, "El correo es obligatorio")
      .email("Correo inválido"),
    password: z.string().min(6, "Mínimo 6 caracteres"),
    confirmPassword: z.string().min(6, "Confirmá tu contraseña"),
  })
  .refine((data) => data.password === data.confirmPassword, {
    path: ["confirmPassword"],
    message: "Las contraseñas no coinciden",
  });

type SignupValues = z.infer<typeof signupSchema>;

export function SignupForm() {
  const router = useRouter();
  const [loading, setLoading] = useState(false);

  // 1. Inicializamos react-hook-form y lo conectamos con Zod
  const form = useForm<SignupValues>({
    resolver: zodResolver(signupSchema),
    defaultValues: {
      email: "",
      password: "",
      confirmPassword: "",
    },
  });

  // 2. Nueva función onSubmit. La validación ya la hizo Zod automáticamente.
  const onSubmit = async (values: SignupValues) => {
    setLoading(true);

    // Acá iría tu lógica real de registro (ej. supabase, clerk, fetch)
    await new Promise((res) => setTimeout(res, 800));

    setLoading(false);
    toast.success("¡Cuenta creada con éxito! Iniciá sesión.");
    router.push("/login");
  };

  return (
    <Card className="max-w-[400px] mx-auto w-full shadow-md">
      <CardHeader>
        <CardTitle className="text-neutral-600">Crear cuenta</CardTitle>
        <CardDescription>Completá tus datos para registrarte</CardDescription>
      </CardHeader>
      <CardContent>
        {/* 3. Le pasamos todas las propiedades del hook form al componente Form */}
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(onSubmit)}
            className="flex flex-col gap-y-4"
          >
            {/* 4. FormField ahora usa control y render (que inyecta field) */}
            <FormField
              control={form.control}
              name="email"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Correo electrónico</FormLabel>
                  <FormControl>
                    <Input
                      type="email"
                      autoComplete="email"
                      placeholder="tu@correo.com"
                      disabled={loading}
                      {...field} /* <-- Esto reemplaza el value y onChange manual */
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="password"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Contraseña</FormLabel>
                  <FormControl>
                    <Input
                      type="password"
                      autoComplete="new-password"
                      placeholder="••••••••"
                      disabled={loading}
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <FormField
              control={form.control}
              name="confirmPassword"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Confirmar contraseña</FormLabel>
                  <FormControl>
                    <Input
                      type="password"
                      autoComplete="new-password"
                      placeholder="Repetí la contraseña"
                      disabled={loading}
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <Button
              size="lg"
              variant="secondary"
              className="w-full mt-2"
              type="submit"
              disabled={loading}
            >
              {loading ? "Creando..." : "Crear cuenta"}
            </Button>
          </form>
        </Form>
        <div className="flex flex-col gap-y-3 text-center text-sm mt-6">
          <a
            href="/login"
            className="text-sky-500 hover:text-sky-600 font-semibold transition-colors"
          >
            ¿Ya tenés cuenta? Iniciá sesión
          </a>
        </div>
      </CardContent>
    </Card>
  );
}
