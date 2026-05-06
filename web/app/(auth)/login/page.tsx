"use client";

import { useState } from "react";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

export default function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Aquí iría la lógica de autenticación
    console.log({ email, password });
  };

  return (
    <div className="max-w-[988px] mx-auto flex-1 w-full flex items-center justify-center p-4">
      {/* Contenedor del formulario */}
      <div className="w-full max-w-[400px]">
        {/* Encabezado */}
        <div className="mb-8">
          <h1 className="text-3xl lg:text-4xl font-bold text-neutral-600 mb-2">
            Iniciar sesión
          </h1>
          <p className="text-slate-500">
            Ingresa tus credenciales para acceder a tu cuenta
          </p>
        </div>

        {/* Formulario */}
        <form onSubmit={handleSubmit} className="flex flex-col gap-y-4 mb-6">
          {/* Campo de email */}
          <div className="flex flex-col gap-y-2">
            <label htmlFor="email" className="text-sm font-semibold text-slate-600">
              Correo electrónico
            </label>
            <Input
              id="email"
              type="email"
              placeholder="tu@correo.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>

          {/* Campo de contraseña */}
          <div className="flex flex-col gap-y-2">
            <label htmlFor="password" className="text-sm font-semibold text-slate-600">
              Contraseña
            </label>
            <Input
              id="password"
              type="password"
              placeholder="••••••••"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>

          {/* Botón de login */}
          <Button size="lg" variant="secondary" className="w-full mt-2">
            Iniciar sesión
          </Button>
        </form>

        {/* Links de navegación */}
        <div className="flex flex-col gap-y-3 text-center text-sm">
          {/* Recuperar contraseña */}
          <Link
            href="/forgot-password"
            className="text-sky-500 hover:text-sky-600 font-semibold transition-colors"
          >
            ¿Olvidaste tu contraseña?
          </Link>

          {/* Registrarse */}
          <div className="text-slate-500">
            ¿No tienes cuenta?{" "}
            <Link
              href="/sing-up"
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
