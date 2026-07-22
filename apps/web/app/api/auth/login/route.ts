import { NextRequest, NextResponse } from "next/server";
import { cookies } from "next/headers";
import { AUTH_COOKIE_NAME, cookieOptions } from "@/lib/auth/constants";
import { acceptLanguageFromLocale, gatewayUrl, readProblem } from "@/lib/api/gateway";

type LoginBody = { email: string; password: string; locale?: string };

export async function POST(request: NextRequest) {
  const body = (await request.json()) as LoginBody;
  const locale = body.locale ?? "tr";

  const upstream = await fetch(gatewayUrl("/api/identity/auth/login"), {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      "Accept-Language": acceptLanguageFromLocale(locale),
    },
    body: JSON.stringify({ email: body.email, password: body.password }),
  });

  if (!upstream.ok) {
    const problem = await readProblem(upstream);
    return NextResponse.json(problem, { status: upstream.status });
  }

  const data = (await upstream.json()) as { accessToken: string; expiresIn: number };
  const cookieStore = await cookies();
  cookieStore.set(AUTH_COOKIE_NAME, data.accessToken, cookieOptions(data.expiresIn));

  return NextResponse.json({ ok: true });
}
