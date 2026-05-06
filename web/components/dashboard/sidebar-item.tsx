"use client";
import Link from "next/link";
import Image from "next/image";
import { usePathname } from "next/navigation";

import { Button } from "@/components/ui/button";

type Props = {
  label: string;
  icon: React.ReactNode;
  href: string;
};

export const SidebarItem = ({ label, icon, href }: Props) => {
  const pathname = usePathname();
  const active = pathname === href;

  return (
    <Button
      variant={active ? "sidebarOutline" : "sidebar"}
      className="justify-start h-[52px]"
      asChild
    >
      <Link href={href}>
        <span className="mr-5 flex items-center">{icon}</span>
        {label}
      </Link>
    </Button>
  );
};
