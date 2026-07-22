"use client";

import { useTranslations } from "next-intl";
import { Link } from "@/i18n/routing";
import { ThemeToggle } from "@/components/layout/theme-toggle";
import { LocaleSwitcher } from "@/components/layout/locale-switcher";

export function SiteHeader() {
  const t = useTranslations("common");

  return (
    <header className="border-b">
      <div className="mx-auto flex h-14 max-w-5xl items-center justify-between px-4">
        <Link href="/" className="font-semibold tracking-tight">
          {t("marketplace")}
        </Link>
        <nav className="flex items-center gap-1">
          <Link href="/login" className="px-3 text-sm text-muted-foreground hover:text-foreground">
            {t("login")}
          </Link>
          <Link href="/register" className="px-3 text-sm text-muted-foreground hover:text-foreground">
            {t("register")}
          </Link>
          <Link href="/account" className="px-3 text-sm text-muted-foreground hover:text-foreground">
            {t("account")}
          </Link>
          <LocaleSwitcher />
          <ThemeToggle />
        </nav>
      </div>
    </header>
  );
}
