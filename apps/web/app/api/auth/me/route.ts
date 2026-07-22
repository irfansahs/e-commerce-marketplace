import { NextResponse } from "next/server";
import { cookies } from "next/headers";
import { AUTH_COOKIE_NAME } from "@/lib/auth/constants";
import { gatewayUrl, readProblem } from "@/lib/api/gateway";

export async function GET() {
  const cookieStore = await cookies();
  const token = cookieStore.get(AUTH_COOKIE_NAME)?.value;
  if (!token) {
    return NextResponse.json({ authenticated: false }, { status: 401 });
  }

  const upstream = await fetch(gatewayUrl("/api/identity/auth/me"), {
    headers: { Authorization: `Bearer ${token}` },
    cache: "no-store",
  });

  if (!upstream.ok) {
    if (upstream.status === 401) {
      cookieStore.delete(AUTH_COOKIE_NAME);
    }
    const problem = await readProblem(upstream);
    return NextResponse.json(problem, { status: upstream.status });
  }

  const me = await upstream.json();
  return NextResponse.json(me);
}
