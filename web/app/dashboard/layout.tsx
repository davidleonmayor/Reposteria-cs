'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { Sidebar } from "@/components/dashboard/sidebar";
import { MobileHeader } from "@/components/dashboard/mobile-header";
import { useAuthStore } from '@/store/use-auth';

type Props = {
  children: React.ReactNode;
};

export default function MainLayout({ children }: Props) {
  const router = useRouter();
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);

  useEffect(() => {
    if (!isAuthenticated) {
      router.replace('/login');
    }
  }, [isAuthenticated, router]);

  if (!isAuthenticated) {
    return null;
  }

  return (
    <>
      <MobileHeader />
      <Sidebar className="hidden lg:flex" />
      <main className="h-full pt-[50px] lg:pt-0 lg:pl-[256px]">
        <div className="max-w-[1056px] mx-auto pt-6 h-full">{children}</div>
      </main>
    </>
  );
}
