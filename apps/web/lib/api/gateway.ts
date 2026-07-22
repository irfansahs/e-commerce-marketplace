const internalBase =
  process.env.INTERNAL_API_URL?.replace(/\/$/, "") ?? "http://gateway-api:8080";

export function gatewayUrl(path: string): string {
  const p = path.startsWith("/") ? path : `/${path}`;
  return `${internalBase}${p}`;
}

export type ProblemDetails = {
  title?: string;
  status?: number;
  errorCode?: string;
  detail?: string;
};

export async function readProblem(response: Response): Promise<ProblemDetails> {
  try {
    return (await response.json()) as ProblemDetails;
  } catch {
    return { status: response.status, title: response.statusText };
  }
}

export function acceptLanguageFromLocale(locale: string): string {
  return locale === "en" ? "en" : "tr";
}
