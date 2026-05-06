import type { Metadata } from "next";
import { Nunito } from "next/font/google";
import "./globals.css";

import { Toaster } from "@/components/ui/sonner";
import { ExitModal } from "@/components/modals/ExitModal";

const font = Nunito({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Gestor De Ventas Reposteria",
  description: "Registra las ventas de productos en Reposteria.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="es" suppressHydrationWarning data-lt-installed>
      <body className={`min-h-full flex flex-col ${font.className}`}>
        <Toaster />
        <ExitModal />
        {children}
      </body>
    </html>
  );
}
