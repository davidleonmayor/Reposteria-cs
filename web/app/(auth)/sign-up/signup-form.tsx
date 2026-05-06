export function SignupForm() {
  return "form";
}
// "use client";

// import { useState } from "react";
// import { useRouter } from "next/navigation";
// import { z } from "zod";
// import { toast } from "sonner";
// import {
//   Form,
//   FormField,
//   FormControl,
//   FormLabel,
//   FormItem,
//   FormMessage,
// } from "@/components/ui/form";
// import { Input } from "@/components/ui/input";
// import { Button } from "@/components/ui/button";
// import {
//   Card,
//   CardContent,
//   CardHeader,
//   CardTitle,
//   CardDescription
// } from "@/components/ui/card";

// const signupSchema = z.object({
//   email: z.string().min(1, "El correo es obligatorio").email("Correo inválido"),
//   password: z.string().min(6, "Mínimo 6 caracteres"),
//   confirmPassword: z.string().min(6, "Confirmá tu contraseña"),
// }).refine((data) => data.password === data.confirmPassword, {
//   path: ["confirmPassword"],
//   message: "Las contraseñas no coinciden",
// });

// type SignupValues = z.infer<typeof signupSchema>;

// export function SignupForm() {
//   const router = useRouter();
//   const [loading, setLoading] = useState(false);

//   const [form, setForm] = useState<SignupValues>({ email: "", password: "", confirmPassword: "" });
//   const [errors, setErrors] = useState<Partial<SignupValues>>({});

//   const handleChange = (field: keyof SignupValues) => (e: React.ChangeEvent<HTMLInputElement>) => {
//     setForm({ ...form, [field]: e.target.value });
//     setErrors({ ...errors, [field]: undefined });
//   };

//   const handleSubmit = async (e: React.FormEvent) => {
//     e.preventDefault();
//     setLoading(true);
//     setErrors({});
//     const parsed = signupSchema.safeParse(form);
//     if (!parsed.success) {
//       const fieldErrors = parsed.error.flatten().fieldErrors;
//       setErrors({
//         email: fieldErrors.email?.[0],
//         password: fieldErrors.password?.[0],
//         confirmPassword: fieldErrors.confirmPassword?.[0],
//       });
//       setLoading(false);
//       toast.error("Revisá los campos e intentá de nuevo");
//       return;
//     }

//     await new Promise((res) => setTimeout(res, 800));
//     setLoading(false);

//     toast.success("¡Cuenta creada con éxito! Iniciá sesión.");
//     router.push("/login");
//   };

//   return (
//     <Card className="max-w-[400px] mx-auto w-full shadow-md">
//       <CardHeader>
//         <CardTitle className="text-neutral-600">Crear cuenta</CardTitle>
//         <CardDescription>Completá tus datos para registrarte</CardDescription>
//       </CardHeader>
//       <CardContent>
//         <Form>
//           <form onSubmit={handleSubmit} className="flex flex-col gap-y-4">
//             <FormField>
//               <FormItem>
//                 <FormLabel htmlFor="email">Correo electrónico</FormLabel>
//                 <FormControl>
//                   <Input
//                     id="email"
//                     type="email"
//                     autoComplete="email"
//                     placeholder="tu@correo.com"
//                     value={form.email}
//                     onChange={handleChange("email")}
//                     required
//                     disabled={loading}
//                   />
//                 </FormControl>
//                 {errors.email && <FormMessage>{errors.email}</FormMessage>}
//               </FormItem>
//             </FormField>
//             <FormField>
//               <FormItem>
//                 <FormLabel htmlFor="password">Contraseña</FormLabel>
//                 <FormControl>
//                   <Input
//                     id="password"
//                     type="password"
//                     autoComplete="new-password"
//                     placeholder="••••••••"
//                     value={form.password}
//                     onChange={handleChange("password")}
//                     required
//                     disabled={loading}
//                   />
//                 </FormControl>
//                 {errors.password && <FormMessage>{errors.password}</FormMessage>}
//               </FormItem>
//             </FormField>
//             <FormField>
//               <FormItem>
//                 <FormLabel htmlFor="confirmPassword">Confirmar contraseña</FormLabel>
//                 <FormControl>
//                   <Input
//                     id="confirmPassword"
//                     type="password"
//                     autoComplete="new-password"
//                     placeholder="Repetí la contraseña"
//                     value={form.confirmPassword}
//                     onChange={handleChange("confirmPassword")}
//                     required
//                     disabled={loading}
//                   />
//                 </FormControl>
//                 {errors.confirmPassword && <FormMessage>{errors.confirmPassword}</FormMessage>}
//               </FormItem>
//             </FormField>
//             <Button size="lg" variant="secondary" className="w-full mt-2" type="submit" disabled={loading}>
//               {loading ? "Creando..." : "Crear cuenta"}
//             </Button>
//           </form>
//         </Form>
//         <div className="flex flex-col gap-y-3 text-center text-sm mt-6">
//           <a href="/login" className="text-sky-500 hover:text-sky-600 font-semibold transition-colors">
//             ¿Ya tenés cuenta? Iniciá sesión
//           </a>
//         </div>
//       </CardContent>
//     </Card>
//   );
// }
