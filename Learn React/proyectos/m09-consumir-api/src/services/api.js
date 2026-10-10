const API_BASE = "https://rickandmortyapi.com/api";

export async function apiGet(endpoint, params = {}) {
  const searchParams = new URLSearchParams(params);
  const url = `${API_BASE}${endpoint}?${searchParams}`;

  const response = await fetch(url);
  if (!response.ok) {
    throw new Error(`Error ${response.status}: ${response.statusText}`);
  }
  return response.json();
}
