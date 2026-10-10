import { useState, useEffect, useCallback } from "react";
import { taskService } from "../services/taskService";
import { commentService } from "../services/commentService";
import { subtaskService } from "../services/subtaskService";

export function useTaskDetail(taskId) {
  const [task, setTask] = useState(null);
  const [comments, setComments] = useState([]);
  const [subtasks, setSubtasks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  async function load() {
    try {
      setLoading(true);
      const [taskData, commentsData, subtasksData] = await Promise.all([
        taskService.getById(taskId),
        commentService.getByTask(taskId),
        subtaskService.getByTask(taskId),
      ]);
      setTask(taskData);
      setComments(commentsData);
      setSubtasks(subtasksData);
      setError(null);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    load();
  }, [taskId]);

  const addComment = useCallback(
    async (content) => {
      const comment = await commentService.create(taskId, content);
      setComments((prev) => [...prev, comment]);
    },
    [taskId]
  );

  const addSubtask = useCallback(
    async (content) => {
      const subtask = await subtaskService.create(taskId, content);
      setSubtasks((prev) => [...prev, subtask]);
    },
    [taskId]
  );

  const toggleSubtask = useCallback(async (id, done) => {
    const updated = await subtaskService.toggle(id, done);
    setSubtasks((prev) => prev.map((s) => (s.id === id ? updated : s)));
  }, []);

  const deleteSubtask = useCallback(async (id) => {
    await subtaskService.delete(id);
    setSubtasks((prev) => prev.filter((s) => s.id !== id));
  }, []);

  const completedCount = subtasks.filter((s) => s.done).length;
  const progress =
    subtasks.length > 0
      ? Math.round((completedCount / subtasks.length) * 100)
      : 0;

  return {
    task,
    comments,
    subtasks,
    loading,
    error,
    progress,
    addComment,
    addSubtask,
    toggleSubtask,
    deleteSubtask,
    reload: load,
  };
}
