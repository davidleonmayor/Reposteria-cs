"use client";
import { useRouter } from "next/navigation";

import { SidebarItem } from "@/components/dashboard/sidebar-item";
import { LINKS } from "@/moks/constants";

function Illustration() {
  return (
    <div className="mb-12 flex justify-center">
      <div className="relative">
        <svg
          className="w-64 h-64 text-gray-300"
          fill="none"
          stroke="currentColor"
          viewBox="0 0 24 24"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth={1.5}
            d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
          />
        </svg>
        <div className="absolute inset-0 flex items-center justify-center">
          <span className="text-6xl">🔍</span>
        </div>
      </div>
    </div>
  );
}

function NavigationLinks() {
  return (
    <div className="space-y-4">
      <p className="text-sm font-semibold text-gray-700 mb-4">
        ¿A dónde quieres ir?
      </p>
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
        {LINKS.map((link) => (
          <SidebarItem
            key={link.href}
            label={link.label}
            href={link.href}
            icon={link.icon}
          />
        ))}
      </div>
    </div>
  );
}

export default function NotFound() {
  const router = useRouter();

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center p-4">
      <div className="max-w-2xl w-full text-center">
        {/* 404 Number */}
        <div className="mb-8">
          <h1 className="text-9xl font-bold text-primary-600 animate-pulse">
            404
          </h1>
        </div>

        {/* Message */}
        <div className="mb-12 space-y-4">
          <h2 className="text-3xl md:text-4xl font-bold text-gray-800">
            Página no encontrada
          </h2>
          <p className="text-lg text-gray-600 max-w-md mx-auto">
            Lo sentimos, la página que estás buscando no existe o ha sido
            movida.
          </p>
        </div>

        <Illustration />
        <NavigationLinks />

        {/* Back Button */}
        <div className="mt-8">
          <button
            onClick={() => router.back()}
            className="inline-flex items-center gap-2 px-6 py-3 bg-secondary-500  rounded-lg font-medium hover:bg-secondary-600 transition-all duration-300 shadow-lg hover:shadow-xl transform hover:scale-105"
          >
            <svg
              className="w-5 h-5"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M10 19l-7-7m0 0l7-7m-7 7h18"
              />
            </svg>
            Volver atrás
          </button>
        </div>

        {/* Decorative Elements */}
        <div className="absolute top-10 left-10 w-32 h-32 bg-primary-200 rounded-full opacity-20 blur-2xl animate-pulse"></div>
        <div className="absolute bottom-10 right-10 w-40 h-40 bg-blue-200 rounded-full opacity-20 blur-2xl animate-pulse delay-1000"></div>
      </div>
    </div>
  );
}
