"use client";

import { useLocale, useTranslations } from "next-intl";
import { usePathname, Link } from "@/i18n/routing";
import { routing } from "@/i18n/routing";
import { Button } from "@/components/ui/button";

export function LocaleSwitcher() {
  const locale = useLocale();
  const pathname = usePathname();
  const t = useTranslations("common");
  const other = routing.locales.find((l) => l !== locale) ?? "en";

  return (
    <Button variant="ghost" size="sm" asChild>
      <Link href={pathname} locale={other} aria-label={t("language")}>
        {other.toUpperCase()}
      </Link>
    </Button>
  );
}
