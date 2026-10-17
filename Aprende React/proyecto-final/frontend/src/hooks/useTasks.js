import { useState, useEffect, useCallback } from "react";
import { taskService } from "../services/taskService";

export function useTasks(projectId) {
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  async function loadTasks() {
    try {
      setLoading(true);
      const data = await taskService.getByProject(projectId);
      setTasks(data);
      setError(null);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadTasks();
  }, [projectId]);

  const createTask = useCallback(
    async (taskData) => {
      const newTask = await taskService.create(projectId, taskData);
      setTasks((prev) => [...prev, newTask]);
      return newTask;
    },
    [projectId]
  );

  const updateTask = useCallback(async (id, updates) => {
    const updated = await taskService.update(id, updates);
    setTasks((prev) => prev.map((t) => (t.id === id ? updated : t)));
    return updated;
  }, []);

  const moveTask = useCallback(async (id, status) => {
    const updated = await taskService.move(id, status);
    setTasks((prev) => prev.map((t) => (t.id === id ? updated : t)));
    return updated;
  }, []);

  const deleteTask = useCallback(async (id) => {
    await taskService.delete(id);
    setTasks((prev) => prev.filter((t) => t.id !== id));
  }, []);

  return {
    tasks,
    setTasks,
    loading,
    error,
    createTask,
    updateTask,
    moveTask,
    deleteTask,
    reload: loadTasks,
  };
}
