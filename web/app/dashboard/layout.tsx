import { Sidebar } from "@/components/dashboard/sidebar";
import { MobileHeader } from "@/components/dashboard/mobile-header";
import { DashboardHeader } from "@/components/dashboard/header";

type Props = {
  children: React.ReactNode;
};

const MainLayout = ({ children }: Props) => {
  return (
    <>
      <DashboardHeader />
      <MobileHeader className="lg:hidden" />
      <Sidebar className="hidden lg:flex" />
      <main className="lg:pl-[256px] h-full pt-16 lg:pt-0">
        <div className="max-w-[1056px] mx-auto pt-6 h-full">{children}</div>
      </main>
    </>
  );
};

export default MainLayout;
