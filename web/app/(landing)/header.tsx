//"use client";
import Image from "next/image";
import Link from "next/link";

import { Button } from "@/components/ui/button";

export const Header = () => {
  return (
    <header className="h-20 w-full border-b-2 border-slate-200 px-4">
      {/* lg:max-w-5xl */}
      <div className="lg:max-w-screen-lg mx-auto flex items-center justify-between h-ful">
        <div className="pt-8 pl-4 pb-7 flex items-center gap-3">
          <Image src="/mascot.svg" height={40} width={40} alt="Mascot" />
          <h1 className="text-2xl font-extrabold text-green-600 tracking-wide">
            Reposteria
          </h1>
        </div>
        <div className="flex">
          <Link href="/login">
            <Button variant="primaryOutline" size="lg">
              Iniciar sesión
            </Button>
          </Link>
          <Link href="/sing-up">
            <Button variant="primaryOutline" size="lg">
              Registrate
            </Button>
          </Link>
        </div>
      </div>
    </header>
  );
};
