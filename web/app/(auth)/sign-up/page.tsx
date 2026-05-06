"use client";

import { SignupForm } from "./signup-form";
import { Toaster } from "@/components/ui/sonner";

export default function SignUpPage() {
  return (
    <div className="flex items-center justify-center min-h-screen p-4 bg-background">
      <SignupForm />
      <Toaster richColors position="top-center" />
    </div>
  );
}
