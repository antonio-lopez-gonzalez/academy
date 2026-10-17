import { api } from "./api";

export const commentService = {
  getByTask: (taskId) => api.get(`/tasks/${taskId}/comments`),
  create: (taskId, content) => api.post(`/tasks/${taskId}/comments`, { content }),
  delete: (id) => api.delete(`/comments/${id}`),
};
