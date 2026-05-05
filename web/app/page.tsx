"use client";
import { toast } from "sonner";

import { Button } from "@/components/ui/button";

export default function Home() {
  function handleClick() {
    toast("Clicked!");
  }

  return (
    <div className="flex flex-col flex-1 items-center justify-center bg-zinc-50 font-sans dark:bg-black">
      <main className="flex flex-1 w-full max-w-3xl flex-col items-center justify-between py-32 px-16 bg-white dark:bg-black sm:items-start">
        <Button onClick={handleClick}>Shadcn-UI Button</Button>
      </main>
    </div>
  );
}
