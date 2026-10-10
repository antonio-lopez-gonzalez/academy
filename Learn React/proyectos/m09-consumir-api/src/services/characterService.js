import { apiGet } from "./api";

export const characterService = {
  getAll(params = {}) {
    return apiGet("/character", params);
  },

  getById(id) {
    return apiGet(`/character/${id}`);
  },

  search(query) {
    return apiGet("/character", { name: query });
  },
};
