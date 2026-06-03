"use client";

import Link from "next/link";
import Image from "next/image";
import { useRouter } from "next/navigation";

import { LogOut, User } from "lucide-react";

import { cn } from "@/lib/utils";
import { SidebarItem } from "./sidebar-item";
import { Button } from "@/components/ui/button";
import { LINKS } from "@/moks/constants";
import { useAuthStore } from "@/store/use-auth";

type Props = {
  className?: string;
};

export const Sidebar = ({ className }: Props) => {
  const router = useRouter();
  const { user, logout } = useAuthStore();

  const handleLogout = () => {
    logout();
    router.replace("/login");
  };

  return (
    <div
      className={cn(
        "flex h-full lg:w-[256px] lg:fixed left-0 top-0 px-4 border-r-2 flex-col",
        className,
      )}
    >
      <Link href="/dashboard/home">
        <div className="pt-8 pl-4 pb-7 flex items-center gap-3">
          <Image src="/mascot.svg" height={40} width={40} alt="Mascot" />
          <h1 className="text-2xl font-extrabold text-green-600 tracking-wide">
            Repostería
          </h1>
        </div>
      </Link>

      <div className="flex flex-col gap-y-2 flex-1">
        {LINKS.map((link) => (
          <SidebarItem
            key={link.href}
            label={link.label}
            href={link.href}
            icon={link.icon}
          />
        ))}
      </div>

      <div className="flex flex-col gap-2 p-4 border-t border-slate-100">
        {user && (
          <div className="flex items-center gap-2 px-2 py-1 text-sm text-slate-500">
            <User className="h-4 w-4 shrink-0" />
            <span className="truncate">{user.role}</span>
          </div>
        )}
        <Button variant="dangerOutline" onClick={handleLogout} className="w-full">
          <LogOut className="mr-2 h-4 w-4" />
          Cerrar sesión
        </Button>
      </div>
    </div>
  );
};
