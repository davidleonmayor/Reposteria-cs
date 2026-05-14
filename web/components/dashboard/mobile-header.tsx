import { CartSheet } from "@/components/cart/cart-sheet";
import { MobileSidebar } from "./mobile-sidebar";

export function MobileHeader() {
  return (
    <nav className="lg:hidden px-6 h-[50px] flex items-center justify-between bg-green-500 border-b fixed top-0 w-full z-50">
      <MobileSidebar />
      <CartSheet />
    </nav>
  );
}
