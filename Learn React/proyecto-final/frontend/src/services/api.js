const API_BASE = import.meta.env.VITE_API_URL || "/api";

export async function apiRequest(endpoint, options = {}) {
  const token = localStorage.getItem("token");

  const config = {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...(token && { Authorization: `Bearer ${token}` }),
      ...options.headers,
    },
  };

  const response = await fetch(`${API_BASE}${endpoint}`, config);

  if (response.status === 401) {
    localStorage.removeItem("token");
    window.location.href = "/login";
    throw new Error("Sesión expirada");
  }

  if (!response.ok) {
    const error = await response.json().catch(() => ({}));
    throw new Error(error.message || `Error ${response.status}`);
  }

  return response.json();
}

export const api = {
  get: (endpoint) => apiRequest(endpoint),
  post: (endpoint, data) =>
    apiRequest(endpoint, {
      method: "POST",
      body: JSON.stringify(data),
    }),
  put: (endpoint, data) =>
    apiRequest(endpoint, {
      method: "PUT",
      body: JSON.stringify(data),
    }),
  delete: (endpoint) => apiRequest(endpoint, { method: "DELETE" }),
};
