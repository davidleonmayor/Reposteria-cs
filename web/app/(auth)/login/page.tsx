"use client";

import Link from "next/link";
import { z } from "zod";
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
import { useLogin } from "@/modules/auth/hooks/useLogin";

const loginSchema = z.object({
  email: z.string().min(1, "El correo es obligatorio").email("Correo inválido"),
  password: z.string().min(1, "La contraseña es obligatoria"),
});

type LoginValues = z.infer<typeof loginSchema>;

export default function Login() {
  const { login, loading, error } = useLogin();

  const form = useForm<LoginValues>({
    // zodResolver types don't account for Zod 4.4.x version.minor change — runtime is fine
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    resolver: zodResolver(loginSchema as any),
    defaultValues: { email: "", password: "" },
  });

  const onSubmit = (values: LoginValues) => login(values);

  return (
    <div className="max-w-[988px] mx-auto flex-1 w-full flex items-center justify-center p-4">
      <div className="w-full max-w-[400px]">
        <div className="mb-8">
          <h1 className="text-3xl lg:text-4xl font-bold text-neutral-600 mb-2">
            Iniciar sesión
          </h1>
          <p className="text-slate-500">
            Ingresá tus credenciales para acceder a tu cuenta
          </p>
        </div>

        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(onSubmit)}
            className="flex flex-col gap-y-4 mb-6"
          >
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
                      {...field}
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
                      autoComplete="current-password"
                      placeholder="••••••••"
                      disabled={loading}
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            {error && (
              <p className="text-sm text-red-500 text-center">{error}</p>
            )}

            <Button
              size="lg"
              variant="secondary"
              className="w-full mt-2"
              type="submit"
              disabled={loading}
            >
              {loading ? "Iniciando sesión..." : "Iniciar sesión"}
            </Button>
          </form>
        </Form>

        <div className="flex flex-col gap-y-3 text-center text-sm">
          <div className="text-slate-500">
            ¿No tenés cuenta?{" "}
            <Link
              href="/sign-up"
              className="text-green-500 hover:text-green-600 font-semibold transition-colors"
            >
              Registrate
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}
