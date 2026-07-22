import { cookies } from "next/headers";
import { redirect } from "@/i18n/routing";
import { AUTH_COOKIE_NAME } from "@/lib/auth/constants";
import { gatewayUrl } from "@/lib/api/gateway";
import { AccountPanel } from "@/components/auth/account-panel";

type Props = { params: Promise<{ locale: string }> };

export default async function AccountPage({ params }: Props) {
  const { locale } = await params;
  const token = (await cookies()).get(AUTH_COOKIE_NAME)?.value;
  if (!token) {
    redirect({ href: "/login", locale });
  }

  const res = await fetch(gatewayUrl("/api/identity/auth/me"), {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  });

  if (!res.ok) {
    redirect({ href: "/login", locale });
  }

  const me = (await res.json()) as { userId: string; email: string; role: string };
  return <AccountPanel me={me} />;
}
