const DEFAULT_API_BASE = 'http://localhost:5168';

export const API_BASE = import.meta.env.VITE_API_BASE_URL ?? DEFAULT_API_BASE;

const withBase = (path) => `${API_BASE}${path.startsWith('/') ? path : `/${path}`}`;

async function parseJson(response) {
  const text = await response.text();
  if (!text) {
    return {};
  }

  try {
    return JSON.parse(text);
  } catch {
    return { success: false, message: text };
  }
}

export async function apiRequest(path, { method = 'GET', token, body, headers = {} } = {}) {
  const response = await fetch(withBase(path), {
    method,
    headers: {
      ...(body ? { 'Content-Type': 'application/json' } : {}),
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...headers
    },
    body: body ? JSON.stringify(body) : undefined
  });

  const payload = await parseJson(response);

  if (!response.ok || payload.success === false) {
    throw new Error(payload.message ?? `Request failed with status ${response.status}`);
  }

  return payload;
}

export function resolveProductImage(imagePath) {
  if (!imagePath) return '';
  if (imagePath.startsWith('http://') || imagePath.startsWith('https://')) return imagePath;
  return `${API_BASE}/${String(imagePath).replace(/^\//, '')}`;
}
