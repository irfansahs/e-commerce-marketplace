import { getTranslations } from "next-intl/server";
import { Link } from "@/i18n/routing";
import { Button } from "@/components/ui/button";

export default async function HomePage() {
  const t = await getTranslations("home");

  return (
    <section className="flex flex-col items-start gap-6 py-12">
      <h1 className="text-4xl font-bold tracking-tight">{t("title")}</h1>
      <p className="max-w-lg text-lg text-muted-foreground">{t("subtitle")}</p>
      <div className="flex gap-3">
        <Button asChild>
          <Link href="/login">{t("ctaLogin")}</Link>
        </Button>
        <Button variant="outline" asChild>
          <Link href="/register">{t("ctaRegister")}</Link>
        </Button>
      </div>
    </section>
  );
}
