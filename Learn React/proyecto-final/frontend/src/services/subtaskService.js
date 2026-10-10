import { api } from "./api";

export const subtaskService = {
  getByTask: (taskId) => api.get(`/tasks/${taskId}/subtasks`),
  create: (taskId, content) => api.post(`/tasks/${taskId}/subtasks`, { content }),
  toggle: (id, done) => api.put(`/subtasks/${id}`, { done }),
  delete: (id) => api.delete(`/subtasks/${id}`),
};
