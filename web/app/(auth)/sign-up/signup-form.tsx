"use client";

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
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  CardDescription,
} from "@/components/ui/card";
import { useRegister } from "@/modules/auth/hooks/useRegister";

const signupSchema = z
  .object({
    name: z.string().min(1, "El nombre es obligatorio"),
    lastName: z.string().min(1, "El apellido es obligatorio"),
    email: z.string().min(1, "El correo es obligatorio").email("Correo inválido"),
    password: z.string().min(6, "Mínimo 6 caracteres"),
    confirmPassword: z.string().min(6, "Confirmá tu contraseña"),
  })
  .refine((data) => data.password === data.confirmPassword, {
    path: ["confirmPassword"],
    message: "Las contraseñas no coinciden",
  });

type SignupValues = z.infer<typeof signupSchema>;

export function SignupForm() {
  const { register, loading } = useRegister();

  const form = useForm<SignupValues>({
    // zodResolver types don't account for Zod 4.4.x version.minor change — runtime is fine
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    resolver: zodResolver(signupSchema as any),
    defaultValues: {
      name: "",
      lastName: "",
      email: "",
      password: "",
      confirmPassword: "",
    },
  });

  const onSubmit = (values: SignupValues) => {
    register({
      name: values.name,
      lastName: values.lastName,
      email: values.email,
      password: values.password,
      personTypeId: 1,
    });
  };

  return (
    <Card className="max-w-[400px] mx-auto w-full shadow-md">
      <CardHeader>
        <CardTitle className="text-neutral-600">Crear cuenta</CardTitle>
        <CardDescription>Completá tus datos para registrarte</CardDescription>
      </CardHeader>
      <CardContent>
        <Form {...form}>
          <form
            onSubmit={form.handleSubmit(onSubmit)}
            className="flex flex-col gap-y-4"
          >
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Nombre</FormLabel>
                  <FormControl>
                    <Input
                      type="text"
                      autoComplete="given-name"
                      placeholder="Juan"
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
              name="lastName"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Apellido</FormLabel>
                  <FormControl>
                    <Input
                      type="text"
                      autoComplete="family-name"
                      placeholder="Pérez"
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
              {loading ? "Creando cuenta..." : "Crear cuenta"}
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
