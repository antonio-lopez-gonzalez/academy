import { api } from "./api";

const API_URL = "/auth";

export const authService = {
  async login(email, password) {
    return api.post(`${API_URL}/login`, { email, password });
  },

  async register(name, email, password) {
    return api.post(`${API_URL}/register`, { name, email, password });
  },

  async getMe() {
    return api.get(`${API_URL}/me`);
  },
};
