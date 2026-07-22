"use client";

import { useTranslations } from "next-intl";
import { useRouter } from "@/i18n/routing";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

type MeResponse = {
  userId: string;
  email: string;
  role: string;
};

export function AccountPanel({ me }: { me: MeResponse }) {
  const t = useTranslations("account");
  const tCommon = useTranslations("common");
  const tAuth = useTranslations("auth");
  const router = useRouter();

  async function logout() {
    await fetch("/api/auth/logout", { method: "POST" });
    toast.success(tAuth("logoutSuccess"));
    router.push("/login");
    router.refresh();
  }

  return (
    <Card className="mx-auto max-w-md">
      <CardHeader>
        <CardTitle>{t("title")}</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        <div>
          <p className="text-sm text-muted-foreground">{t("emailLabel")}</p>
          <p className="font-medium">{me.email}</p>
        </div>
        <div>
          <p className="text-sm text-muted-foreground">{t("role")}</p>
          <p className="font-medium">{me.role}</p>
        </div>
        <Button variant="outline" onClick={logout}>
          {tCommon("logout")}
        </Button>
      </CardContent>
    </Card>
  );
}
