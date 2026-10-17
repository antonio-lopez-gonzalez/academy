import { api } from "./api";

export const taskService = {
  getByProject: (projectId) => api.get(`/projects/${projectId}/tasks`),
  getById: (id) => api.get(`/tasks/${id}`),
  create: (projectId, data) => api.post(`/projects/${projectId}/tasks`, data),
  update: (id, data) => api.put(`/tasks/${id}`, data),
  move: (id, status) => api.put(`/tasks/${id}/move`, { status }),
  delete: (id) => api.delete(`/tasks/${id}`),
  search: (projectId, params) => {
    const query = new URLSearchParams(params).toString();
    return api.get(`/projects/${projectId}/tasks/search?${query}`);
  },
};
